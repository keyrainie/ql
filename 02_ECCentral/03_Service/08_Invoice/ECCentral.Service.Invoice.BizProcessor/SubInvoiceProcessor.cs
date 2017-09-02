using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(SubInvoiceProcessor))]
    public class SubInvoiceProcessor
    {
        private ISubInvoiceDA m_SubInvoiceDA = ObjectFactory<ISubInvoiceDA>.Instance;

        /// <summary>
        /// 创建SubInvoice
        /// </summary>
        /// <param name="entities">子发票列表</param>
        public virtual void Create(List<SubInvoiceInfo> entities)
        {
            TransactionOptions options = new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                if (entities.Count > 0)
                {
                    DeleteBySOSysNo(entities[0].SOSysNo.Value);

                    entities.ForEach(p =>
                    {
                        m_SubInvoiceDA.Create(p);
                    });
                }
                scope.Complete();
            }
        }

        /// <summary>
        /// 删除子发票
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public virtual void DeleteBySOSysNo(int soSysNo)
        {
            m_SubInvoiceDA.DeleteBySOSysNo(soSysNo);
        }

        public virtual List<SubInvoiceInfo> GetListByCriteria(SubInvoiceInfo query)
        {
            return m_SubInvoiceDA.GetListByCriteria(query);
        }
    }
}