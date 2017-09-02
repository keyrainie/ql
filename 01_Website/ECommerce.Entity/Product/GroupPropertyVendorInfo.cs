using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class GroupPropertyVendorInfo
    {
        public int ProductSysNo { get; set; }


        public VendorInfo VendorInfo { get; set; }


        public GroupPropertyInfo GroupPropertyInfo { get; set; }
    }
}
