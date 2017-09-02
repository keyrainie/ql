using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class ProductCostQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        //public string UserName { get; set; }

        //public int? UserSysNo { get; set; }

        /// <summary>
        ///   渠道编号
        /// </summary>
        public int? WebChannelSysNo { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? WarehouseSysNo { get; set; }

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        ///  商品SysNo
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 有可用库存
        /// </summary>
        public bool IsAvailableInventory { get; set; }
    }
}
