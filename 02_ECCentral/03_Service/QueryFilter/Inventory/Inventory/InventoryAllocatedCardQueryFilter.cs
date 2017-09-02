using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    /// <summary>
    /// 库存查询 -  已分配查询 - QueryFiler
    /// </summary>
    public class InventoryAllocatedCardQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
