using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    public class ProductListQueryFilter : QueryFilter
    {
        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
        public string Status { get; set; }
        public string SysNo { get; set; }
    }
}
