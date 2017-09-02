using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Inventory
{
    public class ProductCostOut
    {
        public int SysNo { get; set; }

        public int BillType { get; set; }

        public int BillSysNo { get; set; }

        public decimal Cost { get; set; }

        public int CostInSysNo { get; set; }

        public int Quantity { get; set; }

        public int ProductSysNo { get; set; }

        public int WarehouseNumber { get; set; }
    }
}
