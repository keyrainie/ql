using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.LoginReg
{
    public class CustomerRegisterRequestViewModel
    {
        public string CustomerID { get; set; }

        public string Password { get; set; }

        public string RePassword { get; set; }

        public string ValidatedCode { get; set; }

        public string FromLinkSource { get; set; }

        public string CellPhone { get; set; }

        public string Email { get; set; }
    }
}