using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Inventory
{
    public class InventoryItemCardQueryFilter : QueryFilter
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string StockSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductSysNo { get; set; }

        public string CompanyCode { get; set; }

        public DateTime RMAInventoryOnlineDate { get; set; }

        public string MerchantSysNo { get; set; }
    }
}
