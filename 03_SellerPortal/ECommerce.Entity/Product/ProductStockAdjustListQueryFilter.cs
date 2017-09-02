using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    public class ProductStockAdjustListQueryFilter : QueryFilter
    {
        public string SysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public ProductStockAdjustStatus? Status { get; set; }
        public int? StockSysNo { get; set; }
        public string CreateDateFrom { get; set; }
        public string CreateDateTo { get; set; }
        public string AuditDateFrom { get; set; }
        public string AuditDateTo { get; set; }
        public string ProductSysNo { get; set; }
    }
}
