using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{

    /// <summary>
    ///  预备采购篮商品信息(备货中心用)
    /// </summary>
    public class BasketItemsPrepareInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 订购价格
        /// </summary>
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 目标仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 是否中转
        /// </summary>
        public int? IsTransfer { get; set; }

        /// <summary>
        /// 供应商系统编号
        /// </summary>
        public int? LastVendorSysNo { get; set; }

        /// <summary>
        /// 建议备货数量
        /// </summary>
        public int? ReadyQuantity { get; set; }
    }

}
