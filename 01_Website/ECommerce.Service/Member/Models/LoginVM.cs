using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerce.Facade.Member.Models
{
    public class LoginVM
    {
        public string CustomerID { get; set; }

        public string Password { get; set; }

        public string ValidatedCode { get; set; }

        public bool RememberLogin { get; set; }

        public CustomerSourceType SourceType { get; set; }
    }
}