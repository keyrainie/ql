using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Inventory
{
    public class InventoryQueryFilter : QueryFilter
    {
        public string CompanyCode { get; set; }
        public string ProductSysNo { get; set; }
        public string AuthorizedPMsSysNumber { get; set; }
        public string StockSysNo { get; set; }
        public string MerchantSysNo { get; set; }
    }
}
