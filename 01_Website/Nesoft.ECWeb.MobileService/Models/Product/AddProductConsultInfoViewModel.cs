using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class AddProductConsultInfoViewModel
    {
        public int ProductSysNo { get; set; }
        public int CustomerSysNo { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
    }
}