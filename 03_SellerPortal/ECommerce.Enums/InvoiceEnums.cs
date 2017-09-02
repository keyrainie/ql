using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum SettleOrderType
    {
        [Description("代收结算单")]
        Collecting = 1,
        [Description("佣金结算单")]
        Commission = 2
    }

    public enum SettleOrderStatus
    {
        [Description("未出单")]
        ORG,
        [Description("已出单")]
        SET,
        [Description("已关闭")]
        CLS
    }

    public enum CommissionType
    {
        [Description("销售提成")]
        SAC,
        [Description("订单提成")]
        SOC,
        [Description("配送费用")]
        DEF
    }

    /// <summary>
    /// 财务销售收款单 单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum SOIncomeOrderType
    {
        /// <summary>
        /// 订单,VALUE=1
        /// </summary>
        SO = 1,
        /// <summary>
        /// 退款单,VALUE=2
        /// </summary>
        RO = 2,
        /// <summary>
        /// 财务负收款单,VALUE=3
        /// </summary>
        AO = 3,
        /// <summary>
        /// 退款调整单,VALUE=4
        /// </summary>
        RO_Balance = 4,
        /// <summary>
        /// 多付款退款单,VALUE=5
        /// </summary>
        OverPayment = 5,
        ///// <summary>
        ///// 补偿退款单,VALUE=6
        ///// </summary>
        //RO_Adjust = 6
        /// <summary>
        /// 虚拟团购收款
        /// </summary>
        Group = 10,
        /// <summary>
        /// 虚拟团购退款款
        /// </summary>
        GroupRefund = 11
    }

    /// <summary>
    /// 财务销售收款单 类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum SOIncomeOrderStyle
    {
        /// <summary>
        /// 货到付款
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 款到发货
        /// </summary>
        Advanced = 1,
        /// <summary>
        /// 已出库退款单
        /// </summary>
        RO = 2,
        /// <summary>
        /// 退款调整单
        /// </summary>
        RO_Balance = 3
        ///// <summary>
        ///// 补偿退款单
        ///// </summary>
        //RO_Adjust = 4

    }

    /// <summary>
    /// 财务销售收款单 状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum SOIncomeStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        [Description("已作废")]
        Abandon = -1,
        /// <summary>
        /// 待确认
        /// </summary>
        [Description("待确认")]
        Origin = 0,
        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        Confirmed = 1,
        /// <summary>
        /// 已拆分
        /// </summary>
        [Description("已拆分")]
        Splited = 2,
        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Processed = 3,
        /// <summary>
        /// 网关退款中
        /// </summary>
        [Description("退款中")]
        Processing = 4,
        /// <summary>
        /// 网关退款失败
        /// </summary>
        [Description("退款失败")]
        Failed = 5,
    }

    /// <summary>
    /// 整合的支付状态，将支付状态与应收状态整合。
    /// </summary>
    public enum ConsolidatedPaymentStatus
    {
        [Description("已支付未审核")]
        PaymentWaitApprove = -2,

        [Description("待确认")]
        SOIncomeOrigin = 0,

        [Description("已确认")]
        SOIncomeConfirmed = 1,

        [Description("已拆分")]
        [Obsolete]
        SOIncomeSplited = 2,

        [Description("已处理")]
        SOIncomeProcessed = 3,

        [Description("退款中")]
        SOIncomeProcessing = 4,

        [Description("退款失败")]
        SOIncomeFailed = 5,

        [Description("未支付")]
        PaymentNone = -3
    }
    /// <summary>
    /// 销售退款单 单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum RefundOrderType
    {
        /// <summary>
        /// 退款单
        /// </summary>
        RO = 2,
        /// <summary>
        /// 作废订单退款单
        /// </summary>
        AO = 3,
        /// <summary>
        /// 退款调整单
        /// </summary>
        RO_Balance = 4,
        /// <summary>
        /// 多付款退款单
        /// </summary>
        OverPayment = 5
        ///// <summary>
        ///// 补偿退款单
        ///// </summary>
        //RO_Adjsut = 6
    }
    /// <summary>
    /// 退款类型
    /// </summary>
    public enum RefundPayType
    {
        /// <summary>
        /// 现金退款
        /// </summary>
        [Obsolete("此字段已弃用")]
        [Description("现金退款")]
        CashRefund = 0,

        /// <summary>
        /// 转积分退款
        /// </summary>
        [Obsolete("此字段已弃用")]
        [Description("转积分退款")]
        TransferPointRefund = 1,

        /// <summary>
        /// 银行转账
        /// </summary>
        [Description("银行转账")]
        BankRefund = 3,

        /// <summary>
        /// 网关直接退款
        /// </summary>
        [Description("网关直接退款")]
        NetWorkRefund = 4,

        /// <summary>
        /// 退入余额帐户
        /// </summary>
        [Description("退入余额帐户")]
        PrepayRefund = 5,

        ///// <summary>
        ///// 礼品卡退款
        ///// </summary>
        [Obsolete("此字段已弃用")]
        [Description("礼品卡退款")]
        GiftCardRefund = 9

    }

    /// <summary>
    /// 销售退款单 状态
    /// </summary>
    public enum RefundStatus
    {
        /// <summary>
        /// 待CS审批
        /// </summary>
        [Description("待退款")]
        Origin = 0,
        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已退款")]
        Refunded = 1,
        ///// <summary>
        ///// 待RMA退款
        ///// </summary>
        //[Description("待RMA退款")]
        //WaitingRefund = 2,
        ///// <summary>
        ///// 待财务审批
        ///// </summary>
        //[Description("待财务审批")]
        //WaitingFinAudit = 3
        /// <summary>
        /// 网关退款中
        /// </summary>
        [Description("退款中")]
        Processing = 4,
        /// <summary>
        /// 网关退款失败
        /// </summary>
        [Description("退款失败")]
        Failed = 5,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("已作废")]
        Abandon = -1,
    }

    public class OrdersType
    {
        /// <summary>
        /// 销售订单
        /// </summary>
        public const string SO = "SO";
        /// <summary>
        /// 退货单
        /// </summary>
        public const string RMA = "RMA";
    }

    public enum CashOnDeliveryType
    {
        /// <summary>
        /// 银行卡
        /// </summary>
        [Description("银行卡")]
        PosBankCard = 1,
        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        PosCash = 2,
        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        PosPrePay = 3,
        /// <summary>
        /// 未收款
        /// </summary>
        [Description("未收款")]
        UncollectedReceivables = 4
    }

    /// <summary>
    /// 网上支付信息来源
    /// </summary>
    public enum NetPaySource
    {
        /// <summary>
        /// 网关到账
        /// </summary>
        Bank = 0,

        /// <summary>
        /// 手动添加
        /// </summary>
        Employee = 1,

        /// <summary>
        /// 系统添加
        /// </summary>
        NoNeedPay = 2,

        /// <summary>
        /// 分期添加
        /// </summary>
        Instalment = 3
    }

    /// <summary>
    /// 网上支付状态
    /// </summary>
    public enum NetPayStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,

        /// <summary>
        /// 待审核
        /// </summary>
        Origin = 0,

        /// <summary>
        /// 审核通过
        /// </summary>
        Approved = 1,

        /// <summary>
        /// 已拆分
        /// </summary>
        Splited = 2
    }
}
