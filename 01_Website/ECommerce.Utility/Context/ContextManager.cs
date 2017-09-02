using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;

namespace ECommerce.Utility
{
    public static class ContextManager
    {
        private static Type s_Type = null;
        private static Type GetContextType()
        {
            if (s_Type != null)
            {
                return s_Type;
            }
            string contextTypeName = ConfigurationManager.AppSettings["ContextType"];
            if (contextTypeName != null && contextTypeName.Trim().Length > 0)
            {
                if (contextTypeName.ToUpper() == "WCF")
                {
                    s_Type = Type.GetType("ECommerce.Utility.WCF.WCFServiceContext, ECommerce.Utility.WCF", true);
                }
                else if (contextTypeName.ToUpper() == "WEB")
                {
                    s_Type = Type.GetType("ECommerce.Utility.Web.WebAppContext, ECommerce.Utility.Web", true);
                }
                else
                {
                    s_Type = Type.GetType(contextTypeName, true);
                }
            }
            else // 默认使用WebAppContext
            {
                s_Type = Type.GetType("ECommerce.Utility.Web.WebAppContext, ECommerce.Utility.Web", true);
            }
            return s_Type;
        }

        public static IContext Current
        {
            get
            {
                return  (IContext)Invoker.CreateInstance(GetContextType());
            }
        }
    }
}
