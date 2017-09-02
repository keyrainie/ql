using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Inventory.Request
{
    public class ProductBatchQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public int? StockSysNo { get; set; }

    }
}
