using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.LoginReg
{
    public class LoginViewModel
    {
        public string CustomerID { get; set; }

        public string Password { get; set; }

        public string ValidatedCode { get; set; }

        public bool RememberLogin { get; set; }
    }
}