using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Inventory;

namespace ECommerce.Entity.Product
{
    public class ProductPurchaseInstockAdjustInventoryInfo
    {
        public int ReferenceSysNo { get; set; }
        public string SourceActionName { get; set; }
        public List<InventoryAdjustItemInfo> AdjustItemList { get; set; }
    }
}
