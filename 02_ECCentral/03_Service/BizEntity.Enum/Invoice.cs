using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Invoice
{
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum YNStatus
    {
        N = 0,
        Y = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum BitStatus
    {
        Valid = 0,
        InValid = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PostPayStatus
    {
        No = 0,
        Yes = 1,
        Splited = 2
    }

    /// <summary>
    /// 操作符号
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum OperationSignType
    {
        /// <summary>
        /// ≤
        /// </summary>
        LessThanOrEqual = -1,
        /// <summary>
        /// =
        /// </summary>
        Equal = 0,
        /// <summary>
        /// ≥
        /// </summary>
        MoreThanOrEqual = 1
    }

    /// <summary>
    /// 付款类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayItemStyle
    {
        /// <summary>
        /// 货到付款
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 预付款
        /// </summary>
        Advanced = 1,
    }

    /// <summary>
    /// 付款单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayItemStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,

        /// <summary>
        /// 待处理
        /// </summary>
        Origin = 0,

        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 1,

        /// <summary>
        /// 已锁定
        /// </summary>
        Locked = 2
    }

    /// <summary>
    /// 应付款单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayableOrderType
    {
        /// <summary>
        /// PO(0)
        /// </summary>
        PO = 0,

        /// <summary>
        /// VendorSettleOrder(1)
        /// </summary>
        VendorSettleOrder = 1,

        ///// <summary>
        ///// FinanceSettleOrder(不再使用)(2)
        ///// </summary>
        //FinanceSettleOrder = 2,

        ///// <summary>
        ///// BalanceOrder(不再使用)(3)
        ///// </summary>
        //BalanceOrder = 3,

        ///// <summary>
        ///// POTempConsign(不再使用)(4)
        ///// </summary>
        //POTempConsign = 4,

        /// <summary>
        /// POAdjust(5)
        /// </summary>
        POAdjust = 5,

        ///// <summary>
        ///// 现金(6)
        ///// </summary>
        //ReturnPointCashAdjust = 6,

        ///// <summary>
        ///// 票扣(7)
        ///// </summary>
        //SubInvoice = 7,

        ///// <summary>
        ///// 帐扣(8)
        ///// </summary>
        //SubAccount = 8,

        /// <summary>
        /// RMA供应商退款(9)
        /// </summary>
        RMAPOR = 9,

        /// <summary>
        /// 代收结算单(10)
        /// </summary>
        CollectionSettlement = 10,

        /// <summary>
        /// 佣金结算单
        /// </summary>
        Commission = 11,

        /// <summary>
        /// 代收代付结算单(12)
        /// </summary>
        CollectionPayment = 12,
        /// <summary>
        /// 租赁结算单,不开发票
        /// </summary>
        LeaseSettle = 13,
        /// <summary>
        /// 团购结算单
        /// </summary>
        GroupSettle = 14,
        /// <summary>
        /// 代销调整单
        /// </summary>
        ConsignAdjust = 15,
        /// <summary>
        /// 成本变价单
        /// </summary>
        CostChange = 16
    }

    /// <summary>
    /// 应付款状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayableStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,

        /// <summary>
        /// 未支付
        /// </summary>
        UnPay = 0,

        /// <summary>
        /// 部分支付
        /// </summary>
        PartlyPay = 1,

        /// <summary>
        /// 全部支付
        /// </summary>
        FullPay = 2
    }

    /// <summary>
    /// 应付款发票状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayableInvoiceStatus
    {
        /// <summary>
        /// 缺少
        /// </summary>
        Absent = 0,

        /// <summary>
        /// 不完整
        /// </summary>
        Incomplete = 1,

        /// <summary>
        /// 完整
        /// </summary>
        Complete = 2
    }

    /// <summary>
    /// 应付款发票实际状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayableInvoiceFactStatus
    {
        /// <summary>
        /// 正确
        /// </summary>
        Corrent = 0,

        /// <summary>
        /// 金额不符
        /// </summary>
        MoneyWrong = 1,

        /// <summary>
        /// 品名规格不符
        /// </summary>
        StandardWrong = 2,

        /// <summary>
        /// 其他错误
        /// </summary>
        Others = 3,
    }

    /// <summary>
    /// 付款单审核状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PayableAuditStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        NotAudit,   // = "O"
        /// <summary>
        /// 待财务审核
        /// </summary>
        WaitFNAudit,// = "F"
        /// <summary>
        /// 财务审核通过
        /// </summary>
        Audited     // = "A"
    }

    /// <summary>
    /// 网上支付信息来源
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
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
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
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

    /// <summary>
    /// 退款类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum RefundPayType
    {
        /// <summary>
        /// 现金退款
        /// </summary>
        [Obsolete("此字段已弃用")]
        CashRefund = 0,

        /// <summary>
        /// 转积分退款
        /// </summary>
        [Obsolete("此字段已弃用")]
        TransferPointRefund = 1,

        /// <summary>
        /// 邮政退款
        /// </summary>
        [Obsolete("此字段已弃用")]
        PostRefund = 2,

        /// <summary>
        /// 银行转账
        /// </summary>
        BankRefund = 3,

        /// <summary>
        /// 网关直接退款
        /// </summary>
        NetWorkRefund = 4,

        /// <summary>
        /// 退入余额帐户
        /// </summary>
        PrepayRefund = 5,

        ///// <summary>
        ///// 礼品卡退款
        ///// </summary>
        [Obsolete("此字段已弃用")]
        GiftCardRefund = 9



    }

    /// <summary>
    /// 仓储类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum StockType
    {
        /// <summary>
        /// 自有仓储
        /// </summary>
        SELF,

        /// <summary>
        /// 商家仓储
        /// </summary>
        MET,
        

        /// <summary>
        /// 自有+商家仓储
        /// </summary>
        NAM
    }

    /// <summary>
    /// 配送类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum DeliveryType
    {
        /// <summary>
        /// 自有配送
        /// </summary>
        SELF,

        /// <summary>
        /// 商家配送
        /// </summary>
        MET
    }

    /// <summary>
    /// 开票类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum InvoiceType
    {
        /// <summary>
        /// 自有开票
        /// </summary>
        SELF,

        /// <summary>
        /// 商家开票
        /// </summary>
        MET
    }

    /// <summary>
    /// 电汇-邮局收款单处理状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PostIncomeHandleStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        WaitingHandle = 0,

        /// <summary>
        /// 已处理
        /// </summary>
        Handled = 1,
    }

    /// <summary>
    /// 电汇-邮局收款单查询条件处理情况枚举
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PostIncomeHandleStatusUI
    {
        /// <summary>
        /// 待确认
        /// </summary>
        UnConfirmed = 0,
        /// <summary>
        /// 待处理
        /// </summary>
        UnHandled = 1,
        /// <summary>
        /// 已处理
        /// </summary>
        Handled = 2
    }

    public enum PostIncomeStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,

        /// <summary>
        /// 初始状态
        /// </summary>
        Origin = 0,

        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PostIncomeConfirmStatus
    {
        /// <summary>
        /// 已关联= "R"
        /// </summary>
        Related,
        /// <summary>
        /// 取消关联="C"
        /// </summary>
        Cancel,
        /// <summary>
        /// 已核收= "A"
        /// </summary>
        Audit,
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
        GroupRefund = 11,


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
        Abandon = -1,
        /// <summary>
        /// 待确认
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed = 1,
        /// <summary>
        /// 已拆分
        /// </summary>
        Splited = 2,
        /// <summary>
        /// 已处理
        /// </summary> 
        Processed = 3,
        /// <summary>
        /// 网关退款中
        /// </summary>
        Processing = 4,
        /// <summary>
        /// 网关退款失败
        /// </summary>
        ProcessingFailed = 5
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
    /// 销售退款单 状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum RefundStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 待CS审批
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已审批
        /// </summary>
        Audit = 1,
        /// <summary>
        /// 待RMA退款
        /// </summary>
        WaitingRefund = 2,
        /// <summary>
        /// 待财务审批
        /// </summary>
        WaitingFinAudit = 3
    }

    /// <summary>
    /// 统计类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum StatisticType
    {
        /// <summary>
        /// 总计
        /// </summary>
        Total,
        /// <summary>
        /// 本页小计
        /// </summary>
        Page
    }

    /// <summary>
    /// 退款结果
    /// PS：可能没有使用了，暂时先写在这里
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum WLTRefundStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Failure = -1,
        /// <summary>
        /// 处理中(为神州运通退预付卡专用状态)
        /// </summary>
        Processing = 3,
    }

    /// <summary>
    /// 单据添加方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum TrackingInfoStyle
    {
        /// <summary>
        /// 自动添加
        /// </summary>
        Auto = 0,

        /// <summary>
        /// 手动添加
        /// </summary>
        Manual = 1,
    }

    /// <summary>
    /// 单据处理进度
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum TrackingInfoStatus
    {
        /// <summary>
        /// 业务跟进
        /// </summary>
        Follow, // = 'A'
        /// <summary>
        /// 提交报损
        /// </summary>
        Submit, // = 'S'
        /// <summary>
        /// 核销完毕
        /// </summary>
        Confirm// = 'C'
    }

    /// <summary>
    /// 逾期未收款订单责任人状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum ResponseUserStatus
    {
        /// <summary>
        /// 有效, ='A'
        /// </summary>
        Active,
        /// <summary>
        /// 无效 = 'D'
        /// </summary>
        Dective
    }

    /// <summary>
    /// 责任人来源方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum ResponsibleSource
    {
        /// <summary>
        /// 网关
        /// </summary>
        NetPay,

        /// <summary>
        /// 人工
        /// </summary>
        Manual
    }

    /// <summary>
    /// 责任人负责的收款单类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum ResponseUserOrderStyle
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
        /// 退款异常
        /// </summary>
        RefundException = 2
    }

    /// <summary>
    /// POS支付方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum POSPayType
    {
        /// <summary>
        /// 刷卡='01'
        /// </summary>
        Card,
        /// <summary>
        /// 现金='02'
        /// </summary>
        Cash,
        /// <summary>
        /// 支票='03'
        /// </summary>
        Check
    }

    /// <summary>
    /// POS自动确认状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum AutoConfirmStatus
    {
        /// <summary>
        /// 成功='A'
        /// </summary>
        Success,
        /// <summary>
        /// 失败='D'
        /// </summary>
        Fault,
    }

    /// <summary>
    /// SAP导入结果
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum SapImportedStatus
    {
        /// <summary>
        /// 初始-A
        /// </summary>
        Origin,
        /// <summary>
        /// 成功-S
        /// </summary>
        Success,
        /// <summary>
        /// 失败-E
        /// </summary>
        Fault,
        /// <summary>
        /// 未完成-F
        /// </summary>
        Unfinished,
        /// <summary>
        /// 未处理
        /// </summary>
        UnHandle
    }

    /// <summary>
    /// 发票状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum InvoiceStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已核对
        /// </summary>
        Audited = 1
    }

    /// <summary>
    /// 账期类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum VendorPayPeriodType
    {
        /// <summary>
        /// 款到发货（预付）
        /// </summary>
        MoneyInItemOut = 1,
        /// <summary>
        /// 货到付款
        /// </summary>
        ItemInMoneyIn = 2,
        /// <summary>
        /// 货到后2天(*)
        /// </summary>
        ItemInAfter2 = 3,
        /// <sumary>
        /// 货到后每周一(*)
        /// </sumary>
        ItemInEveryMonday = 4,
        /// <summary>
        /// 货到当天，且票到
        /// </summary>
        ItemInPayIn = 5,
        /// <summary>
        /// 货到后3天，且票到
        /// </summary>
        ItemInAfter3PayIn = 6,
        /// <summary>
        /// 货到后7天，且票到
        /// </summary>
        ItemInAfter7PayIn = 7,
        /// <summary>
        /// 货到后14天，且票到
        /// </summary>
        ItemInAfter14PayIn = 8,
        /// <summary>
        /// 货到后30天，且票到
        /// </summary>
        ItemInAfter30PayIn = 9,
        /// <sumary>
        /// 每月25日，且票到(*)
        /// </sumary>
        ItemInEvery25PayIn = 10,
        /// <sumary>
        /// 每月10日及25日，且票到(*)
        /// </sumary>
        ItemInEvery1525PayIn = 11,
        /// <sumary>
        /// 手工结算(代销)
        /// </sumary>
        Consign = 12,
        /// <sumary>
        /// 货与票都到后15天(*)
        /// </sumary>
        ItemInPayInAfter15 = 13,
        /// <sumary>
        /// 货与票都到后30天(*)
        /// </sumary>
        ItemInPayInAfter30 = 14,
        /// <sumary>
        /// 货与票都到后20天(*)
        /// </sumary>
        ItemInPayInAfter20 = 15,
        /// <sumary>
        /// 货与票都到后7天(*)
        /// </sumary>
        ItemInPayInAfter7 = 16,
        /// <sumary>
        /// 货与票都到后45天(*)
        /// </sumary>
        ItemInPayInAfter45 = 17,
        /// <sumary>
        /// 货与票都到后当天(*)
        /// </sumary>
        ItemInPayInAfter0 = 18,
        /// <summary>
        /// 月结
        /// </summary>
        ItemInPayByMonth = 19,
        /// <summary>
        /// 货到后18天，且票到
        /// </summary>
        ItemInAfter18PayIn = 20,
        /// <summary>
        /// 货到后20天，且票到
        /// </summary>
        ItemInAfter20PayIn = 21,
        /// <summary>
        /// 货到后45天，且票到
        /// </summary>
        ItemInAfter45PayIn = 22,
        /// <summary>
        /// 货到后14天
        /// </summary>
        ItemInAfter14 = 23,
        /// <summary>
        /// 货到后20天
        /// </summary>
        ItemInAfter20 = 24,
        /// <summary>
        /// 货到后3天
        /// </summary>
        ItemInAfter3 = 25,
        /// <summary>
        /// 货到后7天
        /// </summary>
        ItemInAfter7 = 26,
        /// <summary>
        /// 货到后10天，且票到
        /// </summary>
        ItemInAfter10PayIn = 27,
        /// <summary>
        /// 货到后25天，且票到
        /// </summary>
        ItemInAfter25PayIn = 28,

        ///////////////////////////CRL17299////////////////////
        //货到后2天 且票到         41
        //货到后4天 且票到         42
        //货到后5天 且票到         43
        //货到后6天 且票到         44
        //货到后8天 且票到         45
        //货到后9天 且票到         46
        //货到后60天 且票到        47
        /// <summary>
        /// 货到后2天，且票到
        /// </summary>
        ItemInAfter2PayIn = 41,
        /// <summary>
        /// 货到后4天，且票到
        /// </summary>
        ItemInAfter4PayIn = 42,
        /// <summary>
        /// 货到后5天，且票到
        /// </summary>
        ItemInAfter5PayIn = 43,
        /// <summary>
        /// 货到后6天，且票到
        /// </summary>
        ItemInAfter6PayIn = 44,
        /// <summary>
        /// 货到后8天，且票到
        /// </summary>
        ItemInAfter8PayIn = 45,
        /// <summary>
        /// 货到后9天，且票到
        /// </summary>
        ItemInAfter9PayIn = 46,
        /// <summary>
        /// 货到后60天，且票到
        /// </summary>
        ItemInAfter60PayIn = 47,
        //半月结，每月10/25日，且票到（系统）31
        //月结，每月10日，且票到（系统）     30
        //月结，每月25日，且票到（系统）     29
        ItemInHalfMonth = 31,
        /// <sumary>
        /// 月结，每月10日，且票到（系统）
        /// </sumary>
        ItemInMonth10 = 30,
        /// <sumary>
        /// 月结，每月25日，且票到（系统）
        /// </sumary>
        ItemInMonth25 = 29,
        //款到发货（银行汇票60天）           51
        //款到发货（银行汇票90天）           52
        /// <summary>
        /// 款到发货（银行汇票60天）
        /// </summary>
        MoneyInItemOut60 = 51,
        /// <summary>
        /// 款到发货（银行汇票90天）
        /// </summary>
        MoneyInItemOut90 = 52,

        #region 经销改版专用

        /// <sumary>
        /// 票到后当天
        /// </sumary>
        InvoiceInNow = 53,
        /// <sumary>
        /// 票到后2天
        /// </sumary>
        InvoiceIn2 = 54,
        /// <sumary>
        /// 票到后3天
        /// </sumary>
        InvoiceIn3 = 55,
        /// <sumary>
        /// 票到后4天
        /// </sumary>
        InvoiceIn4 = 56,
        /// <sumary>
        /// 票到后5天
        /// </sumary>
        InvoiceIn5 = 57,
        /// <sumary>
        /// 票到后6天
        /// </sumary>
        InvoiceIn6 = 58,
        /// <sumary>
        /// 票到后7天
        /// </sumary>
        InvoiceIn7 = 59,
        /// <sumary>
        /// 票到后8天
        /// </sumary>
        InvoiceIn8 = 60,
        /// <sumary>
        /// 票到后9天
        /// </sumary>
        InvoiceIn9 = 61,
        /// <sumary>
        /// 票到后10天
        /// </sumary>
        InvoiceIn10 = 62,
        /// <sumary>
        /// 票到后14天
        /// </sumary>
        InvoiceIn14 = 63,
        /// <sumary>
        /// 票到后15天
        /// </sumary>
        InvoiceIn15 = 64,
        /// <sumary>
        /// 票到后18天
        /// </sumary>
        InvoiceIn18 = 65,
        /// <sumary>
        /// 票到后20天
        /// </sumary>
        InvoiceIn20 = 66,
        /// <sumary>
        /// 票到后25天
        /// </sumary>
        InvoiceIn25 = 67,
        /// <sumary>
        /// 票到后30天
        /// </sumary>
        InvoiceIn30 = 68,
        /// <sumary>
        /// 票到后45天
        /// </sumary>
        InvoiceIn45 = 69,
        /// <sumary>
        /// 票到后60天
        /// </sumary>
        InvoiceIn60 = 70,


        //运行商项目2012.11.21
        /// <summary>
        /// 手工结算
        /// </summary>
        Hand = 71,
        /// <summary>
        /// 月结，每月25日，且票到
        /// </summary>
        OneMonth25 = 72,

        /// <summary>
        /// 月结，每月10日，且票到
        /// </summary>
        OneMonth10 = 73,

        /// <summary>
        /// 半月结，每月10/25日，且票到
        /// </summary>
        HalfMonth = 74

        #endregion 经销改版专用
    }

    /// <summary>
    /// 发票匹配审核状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum APInvoiceMasterStatus
    {
        /// <summary>
        /// 被退回供应商
        /// </summary>
        VPCancel = -3,
        /// <summary>
        /// 拒绝审核
        /// </summary>
        Refuse = -2,
        /// <summary>
        /// 初始
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 提交审核
        /// </summary>
        NeedAudit = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        AuditPass = 2
    }

    /// <summary>
    /// 供应商发票Items状态
    /// </summary>
    public enum APInvoiceItemStatus
    {
        /// <summary>
        /// 有效='A'
        /// </summary>
        Active,
        /// <summary>
        /// 无效='D'
        /// </summary>
        Deactive,
    }

    /// <summary>
    /// 发票差异处理
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum InvoiceDiffType
    {
        /// <summary>
        /// 核对一致
        /// </summary>
        Coincident = 0,
        /// <summary>
        /// 少开发票含10元以内，费用化
        /// </summary>
        LessInvoicingAndExpensing = 1,
        /// <summary>
        /// 多开发票含10元以内，费用化
        /// </summary>
        MultiInvoicingAndExpensing = 2,
        /// <summary>
        /// 少开发票,税金差异挂账
        /// </summary>
        LessInvoicingAndTaxDiff = 3,
        /// <summary>
        /// 多开发票,税金差异挂账
        /// </summary>
        MultiInvoicingAndTaxDiff = 4
    }

    /// <summary>
    /// 客户余额退款状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum BalanceRefundStatus
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
        /// 财务审核通过
        /// </summary>
        FinConfirmed = 1,
        /// <summary>
        /// 客服审核通过
        /// </summary>
        CSConfirmed = 2
    }

    /// <summary>
    /// SAP状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum SAPStatus
    {
        /// <summary>
        /// 有效='A'
        /// </summary>
        Active,
        /// <summary>
        /// 无效='D'
        /// </summary>
        Deactive,
    }

    /// <summary>
    /// 余额账户预收明细类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum BalanceAccountDetailType
    {
        /// <summary>
        /// 收入
        /// </summary>
        PayedIn = 1,
        /// <summary>
        /// 支出
        /// </summary>
        PayedOut = -1
    }

    /// <summary>
    /// 余额账户预收单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum BalanceAccountPrepayType
    {
        /// <summary>
        /// 余额用于SO支付
        /// </summary>
        PayForSO = 1,
        /// <summary>
        /// RO
        /// </summary>
        RO = 2,
        /// <summary>
        /// AO
        /// </summary>
        AO = 3,
        /// <summary>
        /// RO_Balance
        /// </summary>
        RO_Balance = 4,
        /// <summary>
        /// 多付款退款单
        /// </summary>
        Overpayment = 5,
        /// <summary>
        /// 余额转银行帐户
        /// </summary>
        TransferBankAccount = 6
        ///// <summary>
        ///// 补偿退款单
        ///// </summary>
        //RO_Adjust = 7

    }

    public enum VendorPortalType
    {
        /// <summary>
        /// 已保存
        /// </summary>
        O,

        /// <summary>
        /// 已寄出
        /// </summary>
        E,

        /// <summary>
        /// 已核对
        /// </summary>
        A,

        /// <summary>
        /// 被拒绝
        /// </summary>
        D,

        /// <summary>
        /// 异常
        /// </summary>
        R,

        /// <summary>
        /// VendorPortal财务撤销
        /// </summary>
        C
    }

    /// <summary>
    /// 依旧换新状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum OldChangeNewStatus
    {
        /// <summary>
        /// 拒绝审核
        /// </summary>
        RefuseAudit = -2,
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 初始
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 提交审核
        /// </summary>
        SubmitAudit = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        Audited = 2,
        /// <summary>
        /// 退款
        /// </summary>
        Refund = 3,
        /// <summary>
        /// 关闭
        /// </summary>
        Close = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum GroupBuyingSettlementStatus
    {
        /// <summary>
        /// 部分团购失败
        /// </summary>
        PlanFail = -1, //P
        /// <summary>
        /// 已成团
        /// </summary>
        Success = 0,//S
        /// <summary>
        /// 处理失败
        /// </summary>
        Fail = 1, //F
        /// <summary>
        /// 未成团
        /// </summary>
        Null = -99,//Null未成团
    }

    public enum RefundPointStatus
    {
        /// <summary>
        /// 操作失败
        /// </summary>
        Failure = -1,
        /// <summary>
        /// 尚未操作
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 退款处理中(神州运通退预付卡专用状态)
        /// </summary>
        Processing = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum RequestPriceType
    {
        /// <summary>
        /// 0-	采购价
        /// </summary>
        PurchasePrice = 0,
        /// <summary>
        /// 1-	销售价格
        /// </summary>
        SalePrice = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum RequestPriceStatus
    {
        /// <summary>
        /// 0-	待审核
        /// </summary>
        Auditting = 0,
        /// <summary>
        /// 1-	待启动(审核通过)
        /// </summary>
        Audited = 1,
        /// <summary>
        /// 2-	运行中
        /// </summary>
        Running = 2,
        /// <summary>
        /// 3-	中止
        /// </summary>
        Aborted = 3,
        /// <summary>
        /// 4-	已完成
        /// </summary>
        Finished = 4,
        /// <summary>
        /// 已作废
        /// </summary>
        Void = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum CashOnDeliveryType
    {
        /// <summary>
        /// 银行卡
        /// </summary>
        PosBankCard = 1,
        /// <summary>
        /// 现金
        /// </summary>
        PosCash = 2,
        /// <summary>
        /// 预付款
        /// </summary>
        PosPrePay = 3,
        /// <summary>
        /// 未收款
        /// </summary>
        UncollectedReceivables = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum PriceChangeItemStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Enable = 1,
        /// <summary>
        /// 无效
        /// </summary>
        Disable = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum CheckTransactionType
    {
        /// <summary>
        /// 付款
        /// </summary>
        P = 0,
        /// <summary>
        /// 退款
        /// </summary>
        R = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum SaleCurrency
    {
        /// <summary>
        /// RMB
        /// </summary>
        RMB = 1,
        /// <summary>
        /// HKD
        /// </summary>
        HKD = 5
    }

    /// <summary>
    /// 待审核，已审核
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum CheckStatus
    {
        /// <summary>
        /// 待确认
        /// </summary>
        Pending = 0,
        /// <summary>
        /// 已确认
        /// </summary>
        Audited = 1
    }

    /// <summary>
    /// 待支出，已支出
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInvoiceEnum")]
    public enum RealFreightStatus
    {
        /// <summary>
        /// 待支出
        /// </summary>
        Pending = 0,
        /// <summary>
        /// 已支出
        /// </summary>
        Audited = 1
    }
}
