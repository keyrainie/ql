using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 采购单商品信息
    /// </summary>
    public class PurchaseOrderItemInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ItemSysNo { get; set; }

        /// <summary>
        /// 商品批次信息
        /// </summary>
        public string BatchInfo { get; set; }

        /// <summary>
        /// 采购单编号
        /// </summary>
        public int? POSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string BriefName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// 货币编码
        /// </summary>
        public int? CurrencyCode { get; set; }

        /// <summary>
        /// 货币符号
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// 摊销被取消
        /// </summary>
        public decimal? ApportionAddOn { get; set; }

        /// <summary>
        /// 采购成本
        /// </summary>
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// 当前成本
        /// </summary>
        public decimal? CurrentUnitCost { get; set; }

        /// <summary>
        /// 退货成本
        /// </summary>
        public decimal? ReturnCost { get; set; }

        /// <summary>
        /// 抵扣后总价
        /// </summary>
        public decimal? LineReturnedPointCost { get; set; }
        /// <summary>
        /// 同步采购价格
        /// </summary>
        public decimal? PurchasePrice { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? PurchaseQty { get; set; }

        /// <summary>
        /// 移仓在途库存
        /// </summary>
        public int? ShiftQty { get; set; }

        /// <summary>
        /// 上次采购价格
        /// </summary>
        public decimal? LastOrderPrice { get; set; }

        /// <summary>
        /// ExecptStatus
        /// </summary>
        public int? ExecptStatus { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductMode { get; set; }

        /// <summary>
        /// BM Code
        /// </summary>
        public string BMCode { get; set; }

        /// <summary>
        /// 去税成本
        /// </summary>
        public decimal? UnitCostWithoutTax { get; set; }

        /// <summary>
        /// 有效库存
        /// </summary>
        public int? AvailableQty { get; set; }

        /// <summary>
        /// 上月销售总量
        /// </summary>
        public int? M1 { get; set; }

        /// <summary>
        /// 上周销售总量
        /// </summary>
        public int? Week1SalesCount { get; set; }

        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 上一次调价价格
        /// </summary>
        public DateTime? LastAdjustPriceDate { get; set; }

        /// <summary>
        /// 上一次采购时间
        /// </summary>
        public DateTime? LastInTime { get; set; }

        /// <summary>
        /// 未激活或者已失效的库存
        /// </summary>
        public int? UnActivatyCount { get; set; }

        /// <summary>
        /// 正常采购价格
        /// </summary>
        public decimal? VirtualPrice { get; set; }

        /// <summary>
        /// 是否是虚库商品
        /// </summary>
        public bool? IsVirtualStockProduct { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal? TotalPrice
        {
            get { return (OrderPrice ?? 0m) * (PrePurchaseQty ?? 0); }
        }

        /// <summary>
        /// 实际总价 OrderPrice* Quantity
        /// </summary>
        public decimal? ActualPrice
        {
            get { return (OrderPrice ?? 0m) * (Quantity ?? 0); }
        }

        /// <summary>
        /// 计划报关数量
        /// </summary>
        public int? PrePurchaseQty { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public TradeType ProductTradeType { get; set; }
    }
}
