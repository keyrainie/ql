using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum RMARequestType
    {
        [Description("不确定")]
        Unsure = 0,
        [Description("申请返修")]
        Maintain = 1,
        [Description("申请退货")]
        Return = 2,
        [Description("拒绝申请")]
        Reject = 3,
        [Description("申请换货")]
        Exchange = 4
    }

    public enum RMARequestStatus
    {
        [Description("待处理")]
        Origin = 0,
        [Description("处理中")]
        Handling = 1,
        [Description("待审核")]
        WaitingAudit = 3,
        [Description("审核拒绝")]
        AuditRefuesed = -2,
        [Description("已完成")]
        Complete = 2,
        [Description("已作废")]
        Abandon = -1,
    }

    public enum RMARefundStatus
    {
        [Description("待退款")]
        WaitingRefund = 0,
        [Description("待审核")]
        WaitingAudit = 3,
        [Description("已退款")]
        Refunded = 2,
        [Description("已作废")]
        Abandon = -1,
    }

    /// <summary>
    /// 归属于
    /// </summary>
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

    public enum RMALocation
    {
        [Description("非处理状态")]
        Origin = 0,
        [Description("自己")]
        Self = 1,
        [Description("商家")]
        Vendor = 2,
    }

    public enum RefundPriceType
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
    /// 单件发还状态
    /// </summary>
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
}
