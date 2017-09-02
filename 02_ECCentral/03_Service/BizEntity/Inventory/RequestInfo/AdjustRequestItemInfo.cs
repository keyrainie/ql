using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 损益单所包含的商品损益信息
    /// </summary>
    public class AdjustRequestItemInfo : IIdentity
    {
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 损益商品
        /// </summary>
        public ProductInfo AdjustProduct { get; set; }

        /// <summary>
        /// 损益数量
        /// </summary>
        public int AdjustQuantity { get; set; }

        /// <summary>
        /// 损益成本
        /// </summary>
        public decimal AdjustCost { get; set; }

        /// <summary>
        /// 商品批次信息列表
        /// </summary>        
        public List<InventoryBatchDetailsInfo> BatchDetailsInfoList { get; set; }
    }
}
