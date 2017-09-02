using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ECommerce.Utility
{
    public static class TransactionManager
    {
        private const string NODE_NAME = "SupportTransaction";

        public static ITransaction Create()
        {
            string s = ConfigurationManager.AppSettings[NODE_NAME];
            if (s != null && s.Trim().Length > 0)
            {
                s = s.Trim().ToUpper();
                if (s == "1" || s == "Y" || s == "TRUE" || s == "YES")
                {
                    return new TransactionScopeWrapper();
                }
            }
            return new NotSupportTransaction();
        }

        public static ITransaction SuppressTransaction()
        {
            string s = ConfigurationManager.AppSettings[NODE_NAME];
            if (s != null && s.Trim().Length > 0)
            {
                s = s.Trim().ToUpper();
                if (s == "1" || s == "Y" || s == "TRUE" || s == "YES")
                {
                    return new TransactionScopeWrapper(TransactionScopeOption.Suppress);
                }
            }
            return new NotSupportTransaction();
        }

        private class TransactionScopeWrapper : ITransaction
        {
            private readonly TransactionScope m_Scope;

            public TransactionScopeWrapper()
            {
                m_Scope = TransactionScopeFactory.CreateTransactionScope();
            }

            public TransactionScopeWrapper(TransactionScopeOption tso)
            {
                m_Scope = TransactionScopeFactory.CreateTransactionScope(tso);
            }

            public void Complete()
            {
                m_Scope.Complete();
            }

            public void Dispose()
            {
                m_Scope.Dispose();
            }
        }

        private sealed class NotSupportTransaction : ITransaction
        {
            public void Complete() { }

            public void Dispose() { }
        }
    }
}
