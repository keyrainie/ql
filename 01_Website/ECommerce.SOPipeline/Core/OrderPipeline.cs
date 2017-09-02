using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Transactions;

namespace ECommerce.SOPipeline
{
    public class OrderPipeline
    {
        #region Static Member

        private static OrderPipelineElements FindElements(string piplelineID)
        {
            string key = "OrderPipeline_" + piplelineID;
            object item = MemoryCache.Default.Get(key);
            if (item != null && item is OrderPipelineElements)
            {
                return (OrderPipelineElements)item;
            }
            OrderPipelineElements els = OrderPipelineConfiguration.GetElements(piplelineID);
            if (els != null)
            {
                var policy = new CacheItemPolicy();
                policy.SlidingExpiration = TimeSpan.FromMinutes(OrderPipelineConfiguration.CacheSlidingExpirationMinutes);
                MemoryCache.Default.Set(new CacheItem(key, els), policy);
            }
            return els;
        }

        public static OrderPipeline Create(string piplelineID)
        {
            OrderPipelineElements els = FindElements(piplelineID);
            if (els == null)
            {
                throw new KeyNotFoundException("Invalid pipeline id '" + piplelineID + "'.");
            }
            return new OrderPipeline(els);
        }

        private static TransactionScope _CreateTransactionScope()
        {
            if (Transaction.Current != null)
            {
                IsolationLevel level = Transaction.Current.IsolationLevel;
                return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = level });
            }
            else
            {
                return new TransactionScope();
            }
        }

        #endregion

        #region Instance Private Member

        private OrderPipelineElements m_Elements;

        private OrderPipeline(OrderPipelineElements elements)
        {
            m_Elements = elements;
        }

        private void Initialize(OrderInfo order)
        {
            if (m_Elements.Initializers == null || m_Elements.Initializers.Count <= 0)
            {
                return;
            }
            foreach (var initializer in m_Elements.Initializers)
            {
                initializer.Initialize(ref order);
            }
        }

        private bool PreValidate(OrderInfo order, out List<string> errorList)
        {
            errorList = null;
            if (m_Elements.PreValidators == null || m_Elements.PreValidators.Count <= 0)
            {
                return true;
            }
            bool rst = true;
            foreach (var validator in m_Elements.PreValidators)
            {
                string error;
                if (validator.Validate((OrderInfo)order.Clone(), out error) == false)
                {
                    if (errorList == null)
                    {
                        errorList = new List<string>();
                    }
                    errorList.Add(error);
                    if (m_Elements.BreakOnceValidationError)
                    {
                        return false;
                    }
                    rst = false;
                }
            }
            return rst;
        }

        private void Calculate(OrderInfo order)
        {
            if (m_Elements.Calculators == null || m_Elements.Calculators.Count <= 0)
            {
                return;
            }
            foreach (var cal in m_Elements.Calculators)
            {
                cal.Calculate(ref order);
            }
        }

        private bool PostValidate(OrderInfo order, out List<string> errorList)
        {
            errorList = null;
            if (m_Elements.PostValidators == null || m_Elements.PostValidators.Count <= 0)
            {
                return true;
            }
            bool rst = true;
            foreach (var validator in m_Elements.PostValidators)
            {
                string error;
                if (validator.Validate((OrderInfo)order.Clone(), out error) == false)
                {
                    if (errorList == null)
                    {
                        errorList = new List<string>();
                    }
                    errorList.Add(error);
                    if (m_Elements.BreakOnceValidationError)
                    {
                        return false;
                    }
                    rst = false;
                }
            }
            return rst;
        }

        private void Persist(OrderInfo order)
        {
            if (m_Elements.Persisters == null || m_Elements.Persisters.Count <= 0)
            {
                return;
            }
            if (m_Elements.TransactionWithPersisters)
            {
                using (TransactionScope tran = CreateTransactionScope())
                {
                    foreach (var persister in m_Elements.Persisters)
                    {
                        persister.Persist(order);
                    }
                    tran.Complete();
                }
            }
            else
            {
                foreach (var persister in m_Elements.Persisters)
                {
                    persister.Persist(order);
                }
            }
        }

        private TransactionScope CreateTransactionScope()
        {
            if (Transaction.Current != null)
            {
                IsolationLevel level = Transaction.Current.IsolationLevel;
                return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = level });
            }
            else
            {
                return new TransactionScope();
            }
        }

        #endregion

        #region Event

        private event EventHandler<OrderPipelineEventArgs> m_InitializingHandler;
        public event EventHandler<OrderPipelineEventArgs> InitializingHandler
        {
            add { m_InitializingHandler += value; }
            remove { m_InitializingHandler -= value; }
        }
        private void OnInitializing(OrderInfo order)
        {
            var handler = m_InitializingHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineEventArgs> m_InitializedHandler;
        public event EventHandler<OrderPipelineEventArgs> InitializedHandler
        {
            add { m_InitializedHandler += value; }
            remove { m_InitializedHandler -= value; }
        }
        private void OnInitialized(OrderInfo order)
        {
            var handler = m_InitializedHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineEventArgs> m_PreValidatingHandler;
        public event EventHandler<OrderPipelineEventArgs> PreValidating
        {
            add { m_PreValidatingHandler += value; }
            remove { m_PreValidatingHandler -= value; }
        }
        private void OnPreValidating(OrderInfo order)
        {
            var handler = m_PreValidatingHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineValidationEventArgs> m_PreValidatedHandler;
        public event EventHandler<OrderPipelineValidationEventArgs> PreValidated
        {
            add { m_PreValidatedHandler += value; }
            remove { m_PreValidatedHandler -= value; }
        }
        private void OnPreValidated(OrderInfo order, ref bool succeed, ref List<string> errorMsgs)
        {
            OrderPipelineValidationEventArgs args = new OrderPipelineValidationEventArgs(order, succeed, errorMsgs);
            var handler = m_PreValidatedHandler;
            if (handler != null)
            {
                handler(this, args);
            }
            succeed = args.HasSucceed;
            errorMsgs = args.ErrorMessages;
        }

        private event EventHandler<OrderPipelineEventArgs> m_CalculatingHandler;
        public event EventHandler<OrderPipelineEventArgs> Calculating
        {
            add { m_CalculatingHandler += value; }
            remove { m_CalculatingHandler -= value; }
        }
        private void OnCalculating(OrderInfo order)
        {
            var handler = m_CalculatingHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineEventArgs> m_CalculatedHandler;
        public event EventHandler<OrderPipelineEventArgs> Calculated
        {
            add { m_CalculatedHandler += value; }
            remove { m_CalculatedHandler -= value; }
        }
        private void OnCalculated(OrderInfo order)
        {
            var handler = m_CalculatedHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineEventArgs> m_PostValidatingHandler;
        public event EventHandler<OrderPipelineEventArgs> PostValidating
        {
            add { m_PostValidatingHandler += value; }
            remove { m_PostValidatingHandler -= value; }
        }
        private void OnPostValidating(OrderInfo order)
        {
            var handler = m_PostValidatingHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineValidationEventArgs> m_PostValidatedHandler;
        public event EventHandler<OrderPipelineValidationEventArgs> PostValidated
        {
            add { m_PostValidatedHandler += value; }
            remove { m_PostValidatedHandler -= value; }
        }
        private void OnPostValidated(OrderInfo order, ref bool succeed, ref List<string> errorMsgs)
        {
            OrderPipelineValidationEventArgs args = new OrderPipelineValidationEventArgs(order, succeed, errorMsgs);
            var handler = m_PostValidatedHandler;
            if (handler != null)
            {
                handler(this, args);
            }
            succeed = args.HasSucceed;
            errorMsgs = args.ErrorMessages;
        }

        private event EventHandler<OrderPipelineEventArgs> m_PersistingHandler;
        public event EventHandler<OrderPipelineEventArgs> Persisting
        {
            add { m_PersistingHandler += value; }
            remove { m_PersistingHandler -= value; }
        }
        private void OnPersisting(OrderInfo order)
        {
            var handler = m_PersistingHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        private event EventHandler<OrderPipelineEventArgs> m_PersistedHandler;
        public event EventHandler<OrderPipelineEventArgs> Persisted
        {
            add { m_PersistedHandler += value; }
            remove { m_PersistedHandler -= value; }
        }
        private void OnPersisted(OrderInfo order)
        {
            var handler = m_PersistedHandler;
            if (handler != null)
            {
                handler(this, new OrderPipelineEventArgs(order));
            }
        }

        #endregion

        public OrderPipelineProcessResult Process(OrderInfo order)
        {
            OnInitializing(order);
            Initialize(order);
            OnInitialized(order);

            // 1. do pre checking
            OnPreValidating(order);
            List<string> errorList1;
            bool r1 = PreValidate(order, out errorList1);
            OnPreValidated(order, ref r1, ref errorList1);
            if (r1 == false)
            {
                return OrderPipelineProcessResult.Failed(errorList1, order);
            }

            // 2. do calculation
            OnCalculating(order);
            Calculate(order);
            OnCalculated(order);

            // 3. do post checking
            OnPostValidating(order);
            List<string> errorList2;
            bool r2 = PostValidate(order, out errorList2);
            OnPostValidated(order, ref r2, ref errorList2);
            if (r2 == false)
            {
                return OrderPipelineProcessResult.Failed(errorList2, order);
            }

            // 4. do persisting
            OnPersisting(order);
            using (TransactionScope tran = _CreateTransactionScope())
            {
                try
                {
                    Persist(order);
                    tran.Complete();
                }
                catch (Exception ex)
                {
                    ECommerce.Utility.Logger.WriteLog(ex.ToString(), "SOPipeline");
                    //当创建多个订单时，是否允许部分订单创建成功由调用端来决定 poseidon.y.tong
                    throw ex;
                    //return OrderPipelineProcessResult.Failed(new List<string>() { "Persist error" }, order);
                }
            }
            OnPersisted(order);

            // 5. return the result
            return OrderPipelineProcessResult.Succeed(order);
        }
    }

    public class OrderPipelineEventArgs : EventArgs
    {
        public OrderPipelineEventArgs(OrderInfo order)
        {
            Order = order;
        }

        public OrderInfo Order { get; private set; }
    }

    public class OrderPipelineValidationEventArgs : EventArgs
    {
        public OrderPipelineValidationEventArgs(OrderInfo order, bool succeed, List<string> errorMsgs)
        {
            Order = order;
            HasSucceed = succeed;
            ErrorMessages = errorMsgs;
        }

        public bool HasSucceed { get; set; }

        public List<string> ErrorMessages { get; set; }

        public OrderInfo Order { get; private set; }
    }
}
