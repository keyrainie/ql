using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 代收结算单结算商品信息
    /// </summary>
    public class GatherSettlementItemInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// 结算单系统编号
        /// </summary>
        public int? SettleSysNo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 单据编号 
        /// </summary>
        public int? InvoiceNumber { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int? OrderSysNo { get; set; }

        /// <summary>
        /// 结算单类型
        /// </summary>
        public GatherSettleType? SettleType { get; set; }
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        ///  商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int? ProductQuantity { get; set; }

        public int? Quantity { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal? SalePrice { get; set; }

        public decimal? OriginalPrice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal? TotalAmt { get; set; }

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public int? StockSysNo { get; set; }

        public int? WarehouseNumber { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        public DateTime? OutOrRefundDate { get; set; }

        public decimal? PromotionDiscount { get; set; }

        public int? SONumber { get; set; }

        public int? Point { get; set; }

        public string SettleStatus { get; set; } // --状态
        public int? SoItemSysno { get; set; }
        public int? SysNo { get; set; }
        public int? TransactionNumber { get; set; }
        public int? VendorSysno { get; set; }
    }
}
