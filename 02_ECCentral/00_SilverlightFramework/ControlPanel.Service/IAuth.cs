using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess;
using System.Configuration;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    public interface IAuth
    {
        void GetAuthData(string acct, string domain, out KeystoneAuthUserMsg userInfo,
            out List<RoleAttribute> roleAttributeList, out List<Role> roleList, out List<AuthFunctionMsg> functionList);

        bool Login(string acct, string password, ref string domain, bool isTrusted, out string identityToken);

        void Logout();
    }

    internal static class AuthFactory
    {
        private static IAuth s_Instance = null;
        private static object s_SyncObj = new object();

        public static IAuth GetInstance()
        {
            if (s_Instance == null)
            {
                lock (s_SyncObj)
                {
                    if (s_Instance == null)
                    {
                        string typeStr = ConfigurationManager.AppSettings["AuthType"];
                        if (!string.IsNullOrWhiteSpace(typeStr))
                        {
                            s_Instance = (IAuth)Activator.CreateInstance(Type.GetType(typeStr.Trim(), true));
                        }
                        else
                        {
                            s_Instance = new KeyStoneBiz();
                        }
                    }
                }
            }
            return s_Instance;
        }
    }
}
