using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Customer
{
    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum YNStatus
    {
        N = 0,

        Y = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum YNStatusThree
    {
        /// <summary>
        /// 不确定
        /// </summary>
        Uncertain = -1,
        /// <summary>
        /// 是
        /// </summary>
        Yes = 1,
        /// <summary>
        /// 否
        /// </summary>
        No = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerStatus
    {
        Valid,
        InValid
    }
    
    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerType
    {
        /// <summary>
        /// 个人客户
        /// </summary>
        Personal = 0,

        /// <summary>
        /// 企业客户
        /// </summary>
        Enterprise = 1,

        /// <summary>
        /// 校园客户
        /// </summary>
        Campus = 2,

        /// <summary>
        /// 媒客体户
        /// </summary>
        Media = 3,
        /// <summary>
        /// 内部用户
        /// </summary>
        Internal = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum Gender
    {
        /// <summary>
        /// 男
        /// </summary>
        Male = 1,

        /// <summary>
        /// 女
        /// </summary>
        Female = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum VIPRank
    {
        /// <summary>
        /// 普通(自动)
        /// </summary>
        NormalAuto = 1,

        /// <summary>
        /// VIP(自动)
        /// </summary>
        VIPAuto = 2,

        /// <summary>
        /// 普通(手动)
        /// </summary>
        NormalManual = 3,

        /// <summary>
        /// VIP(手动)
        /// </summary>
        VIPManual = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum AuctionRank
    {
        /// <summary>
        /// 铁蛋
        /// </summary>
        IronEgger,
        /// <summary>
        /// 铜蛋
        /// </summary>
        CopperEgger,
        /// <summary>
        /// 银蛋
        /// </summary>
        SilverEgger,
        /// <summary>
        /// 金蛋
        /// </summary>
        GoldEgger,
        /// <summary>
        /// 臭蛋
        /// </summary>
        SmellyEgger
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum AgentType
    {
        /// <summary>
        /// 个人代理
        /// </summary>
        Personal = 2,

        /// <summary>
        /// 校园代理
        /// </summary>
        Campus = 0,

        /// <summary>
        /// 企业代理
        /// </summary>
        Enterprise = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerRank
    {
        /// <summary>
        /// 初级会员；铁牌会员
        /// </summary>
        [Description("一钻会员")]
        Ferrum = 1,
        /// <summary>
        /// 铜牌会员
        /// </summary>
        [Description("二钻会员")]
        Copper = 2,
        /// <summary>
        /// 银牌会员
        /// </summary>
        [Description("三钻会员")]
        Silver = 3,
        /// <summary>
        /// 金牌会员
        /// </summary>
        [Description("四钻会员")]
        Golden = 4,
        /// <summary>
        /// 钻石会员
        /// </summary>
        [Description("五钻会员")]
        Diamond = 5,
        /// <summary>
        /// 皇冠会员
        /// </summary>
        [Description("六钻会员")]
        Crown = 6,
        /// <summary>
        /// 至尊会员
        /// </summary>
        [Description("七钻会员")]
        Supremacy = 7,

        [Description("八钻会员")]
        Eight=8
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerPointsAddRequestStatus
    {
        /// <summary>
        /// 审核拒绝
        /// </summary>
        AuditDenied = -1,
        /// <summary>
        /// 等待审核
        /// </summary>
        AuditWaiting = 0,
        /// <summary>
        /// 审核通过
        /// </summary>
        AuditPassed = 1
    }



    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum OperationSignType
    {
        /// <summary>
        /// =
        /// </summary>
        Equal = 0,

        /// <summary>
        /// ≥
        /// </summary>
        MoreThanOrEqual = 1,

        /// <summary>
        /// ≤
        /// </summary>
        LessThanOrEqual = 2,

        /// <summary>
        /// >
        /// </summary>
        MoreThan = 3,

        /// <summary>
        /// <
        /// </summary>
        LessThan = 4,

        /// <summary>
        /// ≠
        /// </summary>
        NotEqual = 5
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum VisitDealStatus
    {
        /// <summary>
        /// 需要跟进
        /// </summary>
        FollowUp = 0,
        /// <summary>
        /// 处理完成
        /// </summary>
        Complete = 1,
        /// <summary>
        /// 处理失败
        /// </summary>
        Failed = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum VisitSeachType
    {
        /// <summary>
        /// 回访统计
        /// </summary>
        Visited,
        /// <summary>
        /// 待回访
        /// </summary>
        WaitingVisit,
        /// <summary>
        /// 需跟进回访
        /// </summary>
        FollowUpVisit,
        /// <summary>
        /// 待维护
        /// </summary>
        Maintenance,
        /// <summary>
        /// 需跟进维护
        /// </summary>
        FollowUpMaintenance
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum VisitCallResult
    {
        /// <summary>
        /// 已接通
        /// </summary>
        Connected,
        /// <summary>
        /// 未接通
        /// </summary>
        NotConnected,
        /// <summary>
        /// 客户拒绝
        /// </summary>
        CustomerReject,
        /// <summary>
        /// 非本人接听
        /// </summary>
        NotSelf,
        /// <summary>
        /// 号码错误
        /// </summary>
        NumberIsError,
        /// <summary>
        /// 另约时间
        /// </summary>
        OtherTime,
    }

    /// <summary>
    /// 顾客奖品信息单据状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerGiftStatus
    {
        /// <summary>
        /// 原始的
        /// </summary>
        Origin = 1,
        /// <summary>
        /// 已领取
        /// </summary>
        Assigned = 0,
        /// <summary>
        /// 已作废
        /// </summary>
        Voided = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum AvtarShowStatus
    {
        /// <summary>
        /// 显示
        /// </summary>
        Show,

        /// <summary>
        /// 不显示
        /// </summary>
        NotShow,

        /// <summary>
        /// 未设置
        /// </summary>
        NotSet
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerVipOnly
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 1,

        /// <summary>
        /// 贵宾
        /// </summary>
        VIP = 2
    }

    /// <summary>
    /// 顾客经验值变更原因的类型,（订单送经验值这种情况是没有记录日志的）
    /// </summary>
    public enum ExperienceLogType
    {
        /// <summary>
        /// 引荐的新顾客第一笔订单成功后为引荐人增加经验值
        /// </summary>
        Recommend = 1,

        /// <summary>
        /// 商家订单出库，区别于普通订单
        /// </summary>
        MerchantSOOutbound = 4,
        /// <summary>
        /// 手工添加顾客经验值
        /// </summary>
        ManualSetTotalSOMoney = 2,
        /// <summary>
        /// 参加活动增加
        /// </summary>
        Promotion = 3,
        /// <summary>
        /// 物流拒收
        /// </summary>
        LogisticsRejected = 9

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CouponValidStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        A = 0,

        /// <summary>
        /// 无效
        /// </summary>
        F = -1
    }

    #region CS

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CSRole
    {
        /// <summary>
        /// 普通客服
        /// </summary>
        CS = 1,
        /// <summary>
        /// 小组长
        /// </summary>
        Leader = 2,
        /// <summary>
        /// 经理
        /// </summary>
        Manager = 3
    }

    #endregion CS

    #region BatchImportCustomers

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum TemplateType
    {
        AstraZeneca = 1,
        Ricoh = 2,
        VIP = 3
    }

    #endregion BatchImportCustomers

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum ShipTypeSMSStatus
    {
        A = 0,

        F = -1
    }

    /// <summary>
    /// 来电事件状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CallsEventsStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandoned = -1,

        /// <summary>
        /// 处理中
        /// </summary>
        Replied = 1,

        /// <summary>
        /// 处理完毕
        /// </summary>
        Handled = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CustomerCallReason
    {
        /// <summary>
        /// 投诉
        /// </summary>
        Complain = 1,
        /// <summary>
        /// 咨询
        /// </summary>
        Consult = 2,
        /// <summary>
        /// RMA
        /// </summary>
        RMA = 3,
        /// <summary>
        /// 订单
        /// </summary>
        Order = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CallingReferenceType
    {
        /// <summary>
        /// 投诉
        /// </summary>
        Complain = 1,

        /// <summary>
        /// RMA
        /// </summary>
        RMA = 2
    }



    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum CallingRMAStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandoned = -1,

        /// <summary>
        /// 待处理
        /// </summary>
        Origin = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 1,

        /// <summary>
        /// 处理完毕
        /// </summary>
        Handled = 2,

        /// <summary>
        /// 复核处理
        /// </summary>
        CheckProcessing = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum FPCheckStatus
    {
        A = 0,

        F = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum FPCheckItemStatus
    {
        Invalid,
        Valid
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum OrderCheckStatus
    {
        Invalid = 1,
        Valid = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum RefundRequestType
    {
        /// <summary>
        /// 订单退款
        /// </summary>
        SO,

        /// <summary>
        /// 余额取现
        /// </summary>
        Balance
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum RefundRequestStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        O,
        /// <summary>
        /// 审核通过
        /// </summary>
        A,
        /// <summary>
        /// 审核拒绝
        /// </summary>
        R
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum PrepayStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Valid,
        /// <summary>
        /// 无效
        /// </summary>
        InValid
    }

    /// <summary>
    /// 调积分类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum AdjustPointType
    {
        /// <summary>
        /// 生成订单
        /// </summary>
        CreateOrder = 3,

        /// <summary>
        /// 作废订单
        /// </summary>
        AbandonSO = 5,

        /// <summary>
        /// 订单修改
        /// </summary>
        UpdateSO = 12,

        /// <summary>
        /// 商品评论送积分
        /// </summary>
        Remark = 16,

        /// <summary>
        /// 系统帐号增加积分
        /// </summary>
        AddPointToSysAccounts = 20,

        /// <summary>
        /// 促销活动送积分
        /// </summary>
        SalesGivePoints = 25,

        /// <summary>
        /// 退货产生积分收支额
        /// </summary>
        ReturnProductPoint = 34,

        /// <summary>
        /// 销售折扣与折让
        /// </summary>
        SalesDiscountPoint = 35,

        /// <summary>
        /// 客户多付款－产品调价
        /// </summary>
        ProductPriceAdjustPoint = 37,

        /// <summary>
        /// RMA邮资手动转积分
        /// </summary>
        RMAPostageManuToPoints = 42,

        /// <summary>
        /// 退货现金转积分
        /// </summary>
        RefundCashToPoints = 44
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum AdjustPointOperationType
    {
        /// <summary>
        /// 添加积分，扣减积分操作
        /// </summary>
        AddOrReduce = 0,
        /// <summary>
        /// 作废SO,撤销积分使用
        /// </summary>
        Abandon = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum PointStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Valid,
        /// <summary>
        /// 已过期
        /// </summary>
        InValid
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum PrepayType
    {
        /// <summary>
        /// 订单相关 生成订单时使用或者Void订单时退还
        /// </summary>
        SOPay = 1,
        /// <summary>
        /// RO退款
        /// </summary>
        ROReturn = 2,
        /// <summary>
        /// AO退款
        /// </summary>
        BOReturn = 3,
        /// <summary>
        /// RO Balance
        /// </summary>
        RO_BalanceReturn = 4,
        /// <summary>
        /// 多汇款退款
        /// </summary>
        RemitReturn = 5,
        /// <summary>
        /// 余额转银行账户
        /// </summary>
        BalanceReturn = 6
        ///// <summary>
        ///// 补偿退款单
        ///// </summary>
        //RO_AdjustReturn = 7
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum SMSSendStatus
    {
        /// <summary>
        /// 等待发送
        /// </summary>
        Waiting,
        /// <summary>
        /// 已发送
        /// </summary>
        Sended
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum EmailSendStatus
    {
        /// <summary>
        /// 等待发送
        /// </summary>
        Waiting,
        /// <summary>
        /// 已发送
        /// </summary>
        Sended
    }

    public enum SMSType
    {
        /// <summary>
        /// 订单审核
        /// </summary>
        OrderAudit,
        /// <summary>
        /// 订单出库
        /// </summary>
        OrderOutBound,
        /// <summary>
        /// 订单到达
        /// </summary>
        OrderArived,
        /// <summary>
        /// 订单出库
        /// </summary>
        OrderAbandon
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    [Obsolete("此字段已弃用", true)]
    public enum CompanyCustomer
    {
        /// <summary>
        /// 泰隆优选网(默认)
        /// </summary>
        Newegg = 0,
        /// <summary>
        /// AstraZeneca用户
        /// </summary>
        [Obsolete("此字段已弃用",true)]
        AstraZeneca = 1
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResCustomerEnum")]
    public enum IDCardType
    {
        /// <summary>
        /// 身份证
        /// </summary>
        Certificate = 0,
        /// <summary>
        /// 护照
        /// </summary>
        Passport = 1,

        /// <summary>
        /// 其他
        /// </summary>
        Other = 2
    }
}