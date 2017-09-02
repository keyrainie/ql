using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.WebFramework
{
    public class DBAuth : IAuth
    {
        public bool Login(string userName, string pwd, string verifyCode)
        {
            return true;
        }

        public bool CheckLogin(string cookie)
        {
            return true;
        }
        
        public bool ValidateAuth()
        {
            return true;
        }       
    }
}
