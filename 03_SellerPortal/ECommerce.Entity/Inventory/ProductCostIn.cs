using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Inventory
{
    public class ProductCostIn
    {
        public int SysNo { get; set; }

        public int BillType { get; set; }

        public int BillSysNo { get; set; }

        public decimal Cost { get; set; }

        public int LeftQuantity { get; set; }

        public int LockQuantity { get; set; }

        public int ProductSysNo { get; set; }

        public object Quantity { get; set; }

        public int WarehouseNumber { get; set; }
    }
}
