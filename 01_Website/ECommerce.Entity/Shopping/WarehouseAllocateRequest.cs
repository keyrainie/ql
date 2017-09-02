using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shopping
{
    public class WarehouseAllocateRequest
    {
        public int ShippingAreaID { get; set; }

        public List<AllocateItemInfo> ProductList { get; set; }
    }
}
