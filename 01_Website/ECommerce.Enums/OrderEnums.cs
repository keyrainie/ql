using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ECommerce.Enums
{

    [Serializable]
    public enum SOType
    {
        [Description("普通订单")]
        [XmlEnum("0")]
        Normal = 0,
        [Description("团购订单")]
        [XmlEnum("7")]
        GroupBuy = 7,
        [Description("虚拟团购订单")]
        [XmlEnum("71")]
        VirualGroupBuy = 71,
        [Description("礼品卡订单")]
        [XmlEnum("5")]
        PhysicalCard = 5,
    }

    [Serializable]
    public enum SOPaymentStatus
    {
        [Description("所有的状态")]
        [XmlEnum("0")]
        All = 0,
        [Description("未付款")]
        [XmlEnum("1")]
        NoPay = 1,
        [Description("已付款")]
        [XmlEnum("2")]
        HasPay = 2,
    }


    public enum SOItemType
    {
        /// <summary>
        /// 正常商品
        /// </summary>
        [Description("For Sale")]
        [XmlEnum("0")]
        ForSale = 0,
        /// <summary>
        /// 厂商赠品
        /// </summary>
        [Description("Gift")]
        [XmlEnum("1")]
        Gift = 1,
        /// <summary>
        /// 奖品
        /// </summary>
        [Description("Customer Gift")]
        [XmlEnum("2")]
        AwardItem = 2,
        /// <summary>
        /// 优惠券
        /// </summary>
        [Description("Promotion")]
        [XmlEnum("3")]
        Promotion = 3,
        /// <summary>
        /// 延保
        /// </summary>
        [Description("Warranty")]
        [XmlEnum("4")]
        Warranty = 4,
        /// <summary>
        /// 附件
        /// </summary>
        [Description("Accessory")]
        [XmlEnum("5")]
        HiddenGift = 5,
        /// <summary>
        /// 活动赠品
        /// </summary>
        [Description("NeweggGift")]
        [XmlEnum("6")]
        ActivityGift = 6,
        /// <summary>
        /// 虚拟商品
        /// </summary>
        [Description("VirtualProduct")]
        [XmlEnum("7")]
        VirtualProduct = 7,
    }

    public enum SOStatus
    {
        [Description("已拆分")]
        OrderSplited = -6,

        [Description("未交清的订单")]
        BackOrder = -5,

        [Description("系统自动作废")]
        SystemCancel = -4,

        [Description("已作废")]
        ManagerCancel = -3,

        [Description("已作废")]
        CustomerCancel = -2,

        [Description("已作废")]
        EmployeeCancel = -1,

        [Description("待审核")]
        Original = 0,

        [Description("待出库")]
        WaitingOutStock = 1,

        [Description("待海关申报")]
        [Obsolete("该状态已过时")]
        WaitingPay = 2,

        [Description("待海关申报")]
        [Obsolete("该状态已过时")]
        WaitingManagerAudit = 3,
        /// <summary>
        /// 已出库待申报
        /// </summary>
        [Description("已出库")]
        OutStock = 4,

        /// <summary>
        /// 申报成功
        /// </summary>
        [Description("订单完成")]
        Complete = 5,

        [Description("海关申报")] 
        Waiting = 40,

        [Description("海关通关")]
        Reported = 41,

        [Description("发往顾客")]
        CustomsPass = 45,

        [Description("通关失败订单作废")]
        CustomsReject = 65,

        [Description("申报失败订单作废")]
        Reject = 6,

        [Description("订单拒收")]
        ShippingReject = 7
    }

    public enum SOSearchType
    {
        [Description("None")]
        None = 10,
        [Description("Last week")]
        LastWeek = 11,
        [Description("Last month")]
        LastMonth = 12,
        [Description("Status")]
        Status = 13,
        [Description("ALL")]
        ALL = 14,
        [Description("Last three months")]
        LastThreeMonths = 15,
        [Description("A month ago ")]
        LastMonthBofore = 16,
        [Description("VirtualGroup Orders ")]
        VirtualGroup = 17,
        [Description("Three months Before")]
        ThreeMonthsBefore = 18
    }

    /// <summary>
    /// 支付状态
    /// </summary>
    public enum EPaymentStatus
    {
        [Description("支付")]
        Pay = 0,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Fail = 2
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

    public enum AuditingStatus
    {

        [Description("未处理")]
        NoProcessing,

        [Description("已阅读")]
        Peruse,

        [Description("审核通过")]
        Pass,


        [Description("审核不通")]
        NoPass
    }

    /// <summary>
    /// 订单来源
    /// </summary>
    public enum SOSource
    {
        /// <summary>
        /// 非移动设备
        /// </summary>
        None = 0,
        /// <summary>
        /// 电话系统链接过来创建的订单
        /// </summary>
        Phone = 1,
        /// <summary>
        /// 微信下单
        /// </summary>
        Wechat = 2,
        /// <summary>
        /// IPhone 设备下单
        /// </summary>
        IPhone = 3,
        /// <summary>
        /// Android 设备下单
        /// </summary>
        Android = 4

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

}
