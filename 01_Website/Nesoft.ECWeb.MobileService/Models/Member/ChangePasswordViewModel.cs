using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
    }
}