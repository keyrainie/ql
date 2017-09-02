using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Enums
{
    public enum ItemStockStatusType
    {
        /// <summary>
        /// 库存充足
        /// </summary>
        [EnumMember]
        InStock = 0,

        /// <summary>
        /// 可接受预售
        /// </summary>
        [EnumMember]
        PreStock = 1,

        /// <summary>
        /// 库存不足
        /// </summary>
        [EnumMember]
        OutOfStock = 2
    }

    public enum CouponDiscountType
    {
        /// <summary>
        /// 订单折扣百分比:P
        /// </summary>
        OrderAmountPercentage,
        /// <summary>
        /// 订单折扣金额:D
        /// </summary>
        OrderAmountDiscount,
        /// <summary>
        /// 单个商品直减金额:Z
        /// </summary>
        ProductPriceDiscount,
        /// <summary>
        /// 单个商品最终售价:F
        /// </summary>
        ProductPriceFinal
    }

    public enum CouponSaleRuleType
    {
        RelProduct,

        RelCategory,

        RelBrand,

        RelCustomerRank,

        RelArea,

        RelSeller,
    }

    public enum SOIncomeStatus
    {
        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed = 1,
        /// <summary>
        /// 网关退款中
        /// </summary>
        Processing = 4,
        /// <summary>
        /// 网关退款失败
        /// </summary>
        ProcessingFailed = 5
    }
}
