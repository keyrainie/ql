using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ECommerce.WebFramework;

namespace ECommerce.Web
{
    public class UserAuth:IAuth
    {
        public bool ValidateAuth()
        {
            return UserAuthHelper.HasLogin();
        }
    }
}