using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class ValidateCellphoneViewModel
    {
        public string Cellphone { get; set; }
        public string CustomerID { get; set; }
        public string SMSCode { get; set; }
    }
}