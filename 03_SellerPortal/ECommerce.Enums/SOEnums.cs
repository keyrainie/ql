using System;
using System.ComponentModel;

namespace ECommerce.Enums
{

    [Serializable]
    public enum SOPaymentStatus
    {
        [Description("所有的状态")]
        All = 0,
        [Description("未付款")]
        NoPay = 1,
        [Description("已付款")]
        HasPay = 2,
    }

    public enum SOStatus
    {
        //[Description("已拆分")]
        //OrderSplited = -6,

        //[Description("未交清的订单")]
        //BackOrder = -5,

        [Description("系统作废")]
        SystemCancel = -4,

        //[Description("已作废")]
        //ManagerCancel = -3,

        //[Description("已作废")]
        //CustomerCancel = -2,

        [Description("已作废")]
        Abandon = -1,

        [Description("待审核")]
        Original = 0,

        [Description("待出库")]
        WaitingOutStock = 1,

        //[Description("待主管审")]
        //WaitingManagerAudit = 3,

        /// <summary>
        /// 已出库
        /// </summary>
        [Description("已出库")]
        OutStock = 4,
        /// <summary>
        /// 已申报待通关
        /// </summary>
        [Description("已申报待通关")]
        Reported = 41,
        /// <summary>
        /// 已通关发往顾客
        /// </summary>
        [Description("已通关发往顾客")]
        CustomsPass = 45,
        /// <summary>
        /// 订单完成
        /// </summary>
        [Description("订单完成")]
        Complete = 5,
        /// <summary>
        /// 申报失败订单作废
        /// </summary>
        [Description("申报失败订单作废")]
        Reject = 6,
        /// <summary>
        /// 通关失败订单作废
        /// </summary>
        [Description("通关失败订单作废")]
        CustomsReject = 65,
        /// <summary>
        /// 订单拒收
        /// </summary>
        [Description("订单拒收")]
        ShippingReject = 7
    }

    /// <summary>
    /// NetPay状态
    /// </summary>
    public enum NetPayStatusType
    {
        [Description("已作废")]
        Abandon = -1,
        [Description("支付成功")]
        Origin = 0,
        [Description("审核通过")]
        Verified = 1,
    }

    /// <summary>
    /// 支付类型
    /// </summary>
    public enum NetPayType
    {
        [Description("银行在线支付")]
        BankPayType = 1,

        [Description("支付平台支付")]
        PlatformPayType = 2,

        [Description("存储卡支付")]
        DebitCartPayType = 3
    }


    public enum SOItemType
    {
        /// <summary>
        /// 正常商品,IPP3:ForSale
        /// </summary>
        Product = 0,
        /// <summary>
        /// 赠品
        /// </summary>
        Gift = 1,
        /// <summary>
        /// 奖品,IPP3:CustomerGift
        /// </summary>
        Award = 2,
        /// <summary>
        /// 优惠券
        /// </summary>
        Coupon = 3,
        /// <summary>
        /// 延保
        /// </summary>
        ExtendWarranty = 4,
        /// <summary>
        /// 附件,配件
        /// </summary>
        Accessory = 5,
        /// <summary>
        /// 礼品, IPP3:NewEggGift
        /// </summary>
        SelfGift = 6
    }

    /// <summary>
    /// 订单操作者类型
    /// </summary>
    public enum SOOperatorType
    {
        /// <summary>
        /// 客户操作
        /// </summary>
        Customer,
        /// <summary>
        /// 客服人员
        /// </summary>
        User,
        /// <summary>
        /// 系统
        /// </summary>
        System
    }

    /// <summary>
    /// 快递类型
    /// </summary>
    public enum ExpressType
    {
        /// <summary>
        /// 顺丰
        /// </summary>
        [Description("顺丰")]
        SF = 1,
        /// <summary>
        /// 圆通
        /// </summary>
        [Description("圆通")]
        YT = 2,
        [Description("快递100")]
        KD100 = 100
    }
    
    /// <summary>
    /// 订单价格信息状态
    /// </summary>
    public enum SOPriceStatus
    {
        /// <summary>
        /// 原始的
        /// </summary>
        Original,
        /// <summary>
        /// 无效的
        /// </summary>
        Deactivate
    }
}
