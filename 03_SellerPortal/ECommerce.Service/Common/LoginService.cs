using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ECommerce.DataAccess.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Utility;

namespace ECommerce.Service.Common
{
    public class LoginService
    {
        public static UserInfo UserLogin(string userName, string userPassword)
        {
            return LoginDA.Login(userName, userPassword);
        }
    }
}
