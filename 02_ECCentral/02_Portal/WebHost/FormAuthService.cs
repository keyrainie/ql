using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service;

namespace ECCentral.Portal.WebHost
{
    public class FormAuthService : IAuth
    {
        public void GetAuthData(string acct, string domain, out KeystoneAuthUserMsg userInfo,
                   out List<RoleAttribute> roleAttributeList, out List<Role> roleList, out List<AuthFunctionMsg> functionList)
        {
            throw new NotImplementedException();
        }

        public bool Login(string acct, string password, ref string domain, bool isTrusted, out string identityToken)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}