using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ECommerce.Enums
{
    /// <summary>
    /// 前台展示方式
    /// </summary>
    public enum UIModeType
    {
        [Description("实物类")]
        Physical = 0,
        [Description("票务类")]
        Service = 1,
        [Description("酒店类")]
        Ticket = 2
    }
    /// <summary>
    /// 页面链接模式
    /// </summary>
    public enum FPCLinkUrlModeType
    {
        [Description("标准模式")]
        Standard = 0,
        [Description("促销活动页")]
        Promotion = 1,
        [Description("自定义Url")]
        Define = 999
    }
    public enum ProductStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        [EnumMember]
        Abandon = -1,
        /// <summary>
        /// 仅展示
        /// </summary>
        [EnumMember]
        OnlyShow = 0,
        /// <summary>
        /// 上架
        /// </summary>
        [EnumMember]
        Show = 1,
        /// <summary>
        /// 不展示
        /// </summary>
        [EnumMember]
        NotShow = 2
    }

    public enum ProductType
    {
        Normal = 0,
        SecondHand = 1,
        Bad = 2,
        Other = 3
    }

    public enum SourceCompany
    {
        /// <summary>
        /// 日本
        /// </summary>
        JP = 0,
        /// <summary>
        /// 香港
        /// </summary>
        HK = 1
    }

    public enum ProductStoreType
    {
        /// <summary>
        /// 常温
        /// </summary>
        Narmal = 0,
        /// <summary>
        /// 冷藏
        /// </summary>
        Cold = 1,
        /// <summary>
        /// 冷冻
        /// </summary>
        Frozen = 2
    }

    public enum EntryBizType
    {
        /// <summary>
        /// 一般进口
        /// </summary>
        NormalImport = 0,

        /// <summary>
        /// 保税进口
        /// </summary>
        BondedImport = 1
    }

    /// <summary>
    /// 商品品论
    /// </summary>
    public enum ReviewScoreType
    {
        [Description("全部评论")]
        None = 0,

        [Description("很好")]
        ScoreType11 = 11,

        [Description("较好")]
        ScoreType12 = 12,

        [Description("一般")]
        ScoreType13 = 13,

        [Description("较差")]
        ScoreType14 = 14,


        [Description("较差")]
        ScoreType15 = 15,
    }

    public enum ReviewSortType
    {

        [Description("默认排序")]
        DefaultSort = 113,


        [Description("时间排序")]
        DateSort = 112
    }

    public enum ReviewType
    {
        [Description("普通评论")]
        Common = 0,

        [Description("晒单")]
        OrderShow = 1,

    }

    public enum ReviewQueryType
    {
        ForProduct = 0,
        ByCustomer = 1,
        ByProductCode = 2,
        ByNone = 3,
    }
    public enum ProductImageType
    {
        Image,
        Video
    }

    public enum ProductContentType
    {
        [Description("商品详情")]
        Detail,
        [Description("规格参数")]
        Performance,
        [Description("售后服务")]
        Warranty,
        [Description("购买须知")]
        Attention
    }

    public enum FeedbackReplyType
    {
        [Description("商家回复:")]
        Manufacturer,

        [Description("泰隆优选回复:")]
        Newegg,

        [Description("网友回复:")]
        Web,
    }

    /// <summary>
    /// 商品积分兑换类型
    /// </summary>
    [Serializable]
    [DataContract]
    public enum ProductPayType
    {
        [EnumMember]
        MoneyPayOnly = 0,
        [EnumMember]
        BothSupported = 1,
        [EnumMember]
        PointPayOnly = 2,
    }

    /// <summary>
    /// 贸易类型
    /// </summary>
    [Serializable]
    [DataContract]
    public enum TradeType
    {
        /// <summary>
        /// 直邮
        /// </summary>
        [Description("直邮")]
        DirectMail = 0,
        /// <summary>
        /// 自贸
        /// </summary>
        [Description("自贸")]
        FTA = 1,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 2
    }
}
