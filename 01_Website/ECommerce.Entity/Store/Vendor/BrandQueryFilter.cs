using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    public class BrandQueryFilter : QueryFilter
    {
        public string BrandName { get; set; }

        public ValidStatus? Status { get; set; }

        public string ManufacturerName { get; set; }
    }
}
