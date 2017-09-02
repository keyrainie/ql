using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class PurchaseOrderItemProductInfo
    {
        public int SysNo { get; set; }

        public string ProductID { get; set; }

        public int PrePurchaseQty { get; set; }

        public decimal PurchasePrice { get; set; }
    }
}
