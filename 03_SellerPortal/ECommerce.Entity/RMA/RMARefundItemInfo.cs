using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.RMA
{
    public class RMARefundItemInfo : EntityBase
    {
        /// <summary>
        ///  系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 退款单编号
        /// </summary>
        public int? RefundSysNo { get; set; }

        /// <summary>
        /// 单件编号
        /// </summary>
        public int? RegisterSysNo { get; set; }

        /// <summary>
        /// 商品原始价格
        /// </summary>
        public decimal? OrgPrice { get; set; }

        /// <summary>
        /// 商品价值 = 商品原价（商品售价 - 优惠券分摊） - 单位折扣
        /// </summary>
        public decimal? ProductValue { get; set; }

        /// <summary>
        /// 退款价值
        /// </summary>
        public decimal? RefundPrice { get; set; }

        /// <summary>
        /// 商品单位折扣
        /// </summary>
        public decimal? UnitDiscount { get; set; }

        /// <summary>
        /// 商品原始礼品卡扣减金额
        /// </summary>
        public decimal? OrgGiftCardAmt { get; set; }

        /// <summary>
        ///  商品原始获得积分
        /// </summary>
        public int? OrgPoint { get; set; }

        public int? PointType { get; set; }

        /// <summary>
        /// 退还现金
        /// </summary>
        public decimal? RefundCash { get; set; }

        /// <summary>
        /// 退还积分
        /// </summary>
        public int? RefundPoint { get; set; }

        /// <summary>
        /// 商品成本
        /// </summary>
        public decimal? RefundCost { get; set; }

        /// <summary>
        /// 商品去税成本
        /// </summary>
        public decimal? RefundCostWithoutTax { get; set; }

        /// <summary>
        /// 退款积分成品，对应于SOItem的Point
        /// </summary>
        public int? RefundCostPoint { get; set; }

        /// <summary>
        /// 商品金额退还方式
        /// </summary>
        public RefundPriceType? RefundPriceType { get; set; }
    }
}
