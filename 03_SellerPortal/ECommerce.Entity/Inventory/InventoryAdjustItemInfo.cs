using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Inventory
{
    public class InventoryAdjustItemInfo
    {
        public int ProductSysNo { get; set; }
        public int StockSysNo { get; set; }
        public int AdjustQuantity { get; set; }
        public decimal AdjustUnitCost { get; set; }
        public int? AdjustItemBizFlag { get; set; }

        public int CurrencySysNo { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
