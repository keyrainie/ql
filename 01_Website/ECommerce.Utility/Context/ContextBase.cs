using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Utility
{
    public abstract class ContextBase<T> : IContext
    {
        private T m_RealContext;
        private System.Globalization.CultureInfo m_Culture;
        public ContextBase()
        {
            m_RealContext = RealContext;
            m_Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        protected abstract T RealContext { get; }
        protected abstract string GetValueFromRealContext(T context, string key);
        protected abstract string GetUserSysNo(T context);
        protected abstract string GetUserID(T context);
        protected abstract string GetUserDisplayName(T context);
        protected abstract string GetClientIP(T context);

        public void Attach(IContext owner)
        {
            ContextBase<T> c = owner as ContextBase<T>;
            if (c == null)
            {
                return;
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = c.m_Culture;
            m_RealContext = c.m_RealContext;
        }

        public int UserSysNo
        {
            get 
            {
                int i;
                if (int.TryParse(GetUserSysNo(m_RealContext), out i))
                {
                    return i;
                }
                return -1; 
            }
        }

        public string UserID
        {
            get { return GetUserID(m_RealContext); }
        }

        public string UserDisplayName
        {
            get { return GetUserDisplayName(m_RealContext); }
        }

        public string ClientIP
        {
            get { return GetClientIP(m_RealContext); }
        }

        public string this[string key]
        {
            get { return GetValueFromRealContext(m_RealContext, key); }
        }
    }
}
