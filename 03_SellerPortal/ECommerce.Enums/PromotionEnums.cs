using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{

    public enum SaleGiftDiscountBelongType
    {
        BelongGiftItem,
        BelongMasterItem
    }
    public enum SaleGiftType
    {
        /// <summary>
        /// 单品买赠
        /// </summary>
        [Description("单品买赠")]
        Single,
        /// <summary>
        /// 同时购买
        /// </summary>
        [Description("同时购买")]
        Multiple,
        /// <summary>
        /// 买满即送
        /// </summary>
        [Description("买满即送")]
        Full,
        ///// <summary>
        ///// 厂商赠品
        ///// </summary>
        //[Description("厂商赠品")]
        //Vendor,
        //[Description("满额加购")]
        //Additional
    }

    public enum SaleGiftStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Description("初始态")]
        Origin,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitingAudit,
        /// <summary>
        /// 就绪
        /// </summary>
        [Description("就绪")]
        Ready,
        /// <summary>
        /// 运行
        /// </summary>
        [Description("运行")]
        Run,
        /// <summary>
        /// 终止
        /// </summary>
        [Description("终止")]
        Stoped,
        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Void


    }

    public enum ProductRangeType
    {
        [Description("所有商品")]
        All,
        [Description("限定商品")]
        Limit
    }

    public enum RelationType
    {
        [Description("指定商品")]
        Y,
        [Description("排除商品")]
        N
    }
    /// <summary>
    /// 主商品规则组合类型
    /// </summary>
    public enum AndOrType
    {
        And,     //与
        Not     //非：Exclude
    }

    public enum CouponStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Description("初始化")]
        Init,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitingAudit,
        /// <summary>
        /// 就绪
        /// </summary>
        [Description("就绪")]
        Ready,
        /// <summary>
        /// 运行
        /// </summary>
        [Description("运行")]
        Run,
        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Finish,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Void,
        /// <summary>
        /// 终止
        /// </summary>
        [Description("终止")]
        Stoped

        //<option value="O">初始态</option>
        //<option value="W">待审核</option>
        //<option value="R">就绪</option>
        //<option value="A">运行</option>
        //<option value="D">作废</option>
        //<option value="F">完成</option>
    }

    public enum CouponDiscountRuleType
    {
        /// <summary>
        /// 折扣金额
        /// </summary>
        [Description("折扣金额")]
        Discount,

        /// <summary>
        /// 折扣百分比
        /// </summary>
        [Description("折扣百分比")]
        Percentage
    }

    public enum CouponsBindConditionType
    {
        [Description("不限")]
        None,
        //泰隆暂时不提供此活动
        [Description("购物")]
        SO,
        [Description("领取")]
        Get

    }

    public enum CouponValidPeriodType
    {
        [Description("不限")]
        All,
        [Description("自发放日起一周")]
        PublishDayToOneWeek,
        [Description("自发放日起一个月")]
        PublishDayToOneMonth,
        [Description("自发放日起两个月")]
        PublishDayToTwoMonths,
        [Description("自发放日起三个月")]
        PublishDayToThreeMonths,
        [Description("自发放日起六个月")]
        PublishDayToSixMonths,
        [Description("自定义")]
        CustomPeriod
    }

    public enum CouponCustomerRangeType
    {
        [Description("所有顾客")]
        All,
        [Description("指定顾客")]
        Limit
    }

    public enum CouponCodeUsedStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        [Description("有效")]
        Active,
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效")]
        Deactive
    }

    public enum CouponCodeType
    {
        [Description("通用型")]
        Common,
        [Description("绑定型")]
        ThrowIn
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum PSOperationType
    {
        View,
        Create,
        Edit,
        SubmitAudit,
        CancelAudit,
        AuditApprove,
        AuditRefuse,
        Stop,
        Void
    }

    public enum SaleGiftGiftItemType
    {
        GiftPool,   //O 赠品池
        AssignGift  //A 非赠品池
    }
}
