using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// RMA申请单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMARequestStatus
    {
        
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit = 3,
        /// <summary>
        /// 审核拒绝
        /// </summary>
        AuditRefuesed = -2,
        /// <summary>
        /// 初始，待处理
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 处理
        /// </summary>
        Handling = 1,
        /// <summary>
        /// 完成
        /// </summary>
        Complete = 2
    }
    /// <summary>
    /// RMA申请类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMARequestType
    {
        [Obsolete("此字段已弃用")]
        [Description("不确定")]
        Unsure = 0,

        [Obsolete("此字段已弃用")]
        [Description("申请返修")]
        Maintain = 1,

        [Description("申请退货")]
        Return = 2,

        [Obsolete("此字段已弃用")]
        [Description("拒绝申请")]
        Reject = 3,

        [Description("申请换货")]
        Exchange = 4
    }

    /// <summary>
    /// 新品类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMANewProductStatus
    {
        /// <summary>
        /// 非换货
        /// </summary>
        [Description("非换货")]
        Origin = 0,
        /// <summary>
        /// 调新品
        /// </summary>
        [Description("调新品")]
        NewProduct = 1,
        /// <summary>
        /// 调二手
        /// </summary>
        [Description("调二手")]
        SecondHand = 2,
        /// <summary>
        /// 非当前Case产品
        /// </summary>
        [Description("非当前Case产品")]
        OtherProduct = 3,
    }
    /// <summary>
    /// 单件发还状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMARevertStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Abandon = -1,

        /// <summary>
        /// 待发还
        /// </summary>
        [Description("待发还")]
        WaitingRevert = 0,

        /// <summary>
        /// 已发还
        /// </summary>
        [Description("已发还")]
        Reverted = 1,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitingAudit = 2,
    }

    /// <summary>
    /// 送修状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMAOutBoundStatus
    {
        [Description("已作废")]
        Abandon = -1,

        [Description("待送修")]
        Origin = 0,

        [Description("已送修")]
        SendAlready = 1,

        [Description("部分返还")]
        PartlyResponsed = 2,

        [Description("全部返还")]
        Responsed = 3,

        [Description("供应商已退款")]
        VendorRefund = 4,
    }

    /// <summary>
    /// 入库状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMAReturnStatus
    {
        [Description("作废")]
        Abandon = -1,

        [Description("待退正式库")]
        WaitingReturn = 0,

        [Description("已退正式库")]
        Returned = 1,
    }

    /// <summary>
    /// 退款单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMARefundStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,

        /// <summary>
        /// 待退款
        /// </summary>
        WaitingRefund = 0,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded = 2,

        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit = 3
    }

    ///// <summary>
    ///// RMA原因
    ///// </summary>
    //[Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    //public enum RMAReason
    //{
    //    [Description("质量问题")]
    //    QualityReason = 0,

    //    [Description("兼容性")]
    //    Compatibility = 1,

    //    [Description("不满意")]
    //    Discontented = 2,

    //    [Description("运输中损坏")]
    //    AttaintInTransport = 3,

    //    [Description("其他")]
    //    OtherReason = 4
    //}

    /// <summary>
    /// 归属于
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMAOwnBy
    {
        /// <summary>
        /// 非处理状态
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 客户
        /// </summary>
        Customer = 1,
        /// <summary>
        /// 自有
        /// </summary>
        Self = 2,
    }

    /// <summary>
    /// 下一个处理者
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMANextHandler
    {
        /// <summary>
        /// 电话中心
        /// </summary>
        CC = 0,

        /// <summary>
        /// RMA
        /// </summary>
        RMA = 1,
        /// <summary>
        /// 登记
        /// </summary>
        Register = 2,
        /// <summary>
        /// 检测
        /// </summary>
        Check = 3,
        /// <summary>
        /// 催讨
        /// </summary>
        Dun = 4,
        /// <summary>
        /// 发还
        /// </summary>
        GiveBack = 5,
        /// <summary>
        /// 退货入库
        /// </summary>
        ReturnInstock = 6,
        /// <summary>
        /// Executive CC
        /// </summary>
        ReturnStock = 7
    }

    /// <summary>
    /// 退款调整单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RefundBalanceStatus
    {
        /// <summary>
        /// 待退款
        /// </summary>
        WaitingRefund = 0,

        /// <summary>
        /// 已退款
        /// </summary>
        Refunded = 1,

        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1
    }
    /// <summary>
    /// 补偿退款单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RefundAdjustStatus
    {
        /// <summary>
        /// 已创建
        /// </summary>
        Initial = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit = 1,
        /// <summary>
        /// 审核拒绝
        /// </summary>
        AuditRefuesed = 2,
        /// <summary>
        /// 审核通过
        /// </summary>
        Audited = 3,
        /// <summary>
        /// 已退款
        /// </summary>
        Refunded = 4,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1
    }

    /// <summary>
    /// 退款调整单类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RefundBalanceType
    {
        /// <summary>
        /// 销售单
        /// </summary>
        SO = 0,
        /// <summary>
        /// 退款单
        /// </summary>
        RO = 1,
    }

    /// <summary>
    /// 补偿退款单类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RefundAdjustType
    {
        /// <summary>
        /// 运费补偿
        /// </summary>
        ShippingAdjust = 1,
        /// <summary>
        /// 节能补贴
        /// </summary>
        EnergySubsidy = 3,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 2
    }

    /// <summary>
    /// 是否涉及现金
    /// </summary>
    public enum CashFlagStatus
    {
        /// <summary>
        /// 不涉及现金
        /// </summary>
        No = -1,

        /// <summary>
        /// 涉及现金
        /// </summary>
        Yes = 0
    }


    /// <summary>
    /// 退款金额计算方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum ReturnPriceType
    {
        /// <summary>
        /// 扣除 10%，按商品原始价格的 90% 退款
        /// </summary>
        TenPercentsOff = 0,

        /// <summary>
        /// 商品原始价格，按商品原始价格退款
        /// </summary>
        OriginPrice = 1,

        /// <summary>
        /// 输入价格，按录入金额退款
        /// </summary>
        InputPrice = 2
    }

    /// <summary>
    /// RMA跟踪日志状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum InternalMemoStatus
    {
        /// <summary>
        /// 处理完毕
        /// </summary>
        Close = 0,

        /// <summary>
        /// 需要跟进
        /// </summary>
        Open = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMALocation
    {
        [Description("非处理状态")]
        Origin = 0,

        [Description("自己")]
        Self = 1,

        [Description("Vendor")]
        Vendor = 2,
    }

    public enum TriStatus
    {
        [Description("Abandon")]
        Abandon = -1,
        [Description("Origin")]
        Origin = 0,
        [Description("Handled")]
        Handled = 1,
    }
    /// <summary>
    /// RMA库存查询方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum RMAInventorySearchType
    {
        Product = 0,
        RMA = 1,
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum CompareSymbolType
    {
        /// <summary>
        /// =
        /// </summary>
        Equal = 0,
        /// <summary>
        /// >
        /// </summary>
        MoreThan = 1,

        /// <summary>
        /// <
        /// </summary>
        LessThan = 2,
    }

    ///// <summary>
    ///// 收款银行单状态
    ///// </summary>
    //[Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    //public enum IncomeBankInfoStatus
    //{
    //    /// <summary>
    //    /// 拒绝退款
    //    /// </summary>
    //    Abandon = -1,

    //    /// <summary>
    //    /// 待审核
    //    /// </summary>
    //    Apply = 0,

    //    /// <summary>
    //    /// 审核通过
    //    /// </summary>
    //    VerifyPass = 1,

    //    /// <summary>
    //    /// 待RMA退款
    //    /// </summary>
    //    WaitingRefund = 2
    //}

    ///// <summary>
    ///// 退款类型
    ///// </summary>
    //[Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    //public enum RefundPayType
    //{
    //    /// <summary>
    //    /// 现金退款
    //    /// </summary>
    //    CashRefund = 0,

    //    /// <summary>
    //    /// 转积分退款
    //    /// </summary>
    //    TransferPointRefund = 1,

    //    /// <summary>
    //    /// 邮政退款
    //    /// </summary>
    //    PostRefund = 2,

    //    /// <summary>
    //    /// 银行转账
    //    /// </summary>
    //    BankRefund = 3,

    //    /// <summary>
    //    /// 网关直接退款
    //    /// </summary>
    //    NetWorkRefund = 4,

    //    /// <summary>
    //    /// 退入余额帐户
    //    /// </summary>
    //    PrepayRefund = 5,

    //    /// <summary>
    //    /// 中智积分支付
    //    /// </summary>
    //    ZhongZhiPointPay = 6,

    //    /// <summary>
    //    /// 平安万里通积分支付
    //    /// </summary>
    //    SafeWanLiTongPointPay = 7,

    //    /// <summary>
    //    /// 退入支付宝帐户
    //    /// </summary>
    //    AliPay = 8,

    //    /// <summary>
    //    /// 礼品卡退款
    //    /// </summary>
    //    GiftCardRefund = 9
    //}

    /// <summary>
    /// 处理类型
    /// </summary>
    public enum ProcessType
    {
        UnKnown = -1,
        MET = 0,
        NEG = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum SellersType
    {
        [Description("泰隆优选")]
        Self = 1,
        [Description("商家")]
        Seller = 0
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResRMAEnum")]
    public enum ERPReturnType
    {      
        UnReturn = 0,
        Return = 1
    }
}
