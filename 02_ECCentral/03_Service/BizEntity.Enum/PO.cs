using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.PO
{
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum YNStatus
    {
        Yes = 1,
        NO = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ValidStatus
    {
        A = 0,
        D = -1
    }

    /// <summary>
    /// 供应商等级
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorRank
    {
        /// <summary>
        /// A
        /// </summary>
        A,
        /// <summary>
        /// B
        /// </summary>
        B,
        /// <summary>
        /// C
        /// </summary>
        C
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PayPeriodType
    {
        [Description("A")]
        A,
        [Description("B")]
        B,
        [Description("C")]
        C
    }

    /// <summary>
    /// 供应商属性
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorConsignFlag
    {
        /// <summary>
        /// 经销
        /// </summary>
        Sell = 0,
        /// <summary>
        /// 代销
        /// </summary>
        //[Obsolete("此字段已弃用")]
        Consign = 1,
        /// <summary>
        /// 代收
        /// </summary>
        [Obsolete("此字段已弃用")]
        Gather = 3,
        /// <summary>
        /// 代收代付
        /// </summary>
        [Obsolete("此字段已弃用")]
        GatherPay = 4,
        /// <summary>
        /// 团购
        /// </summary>
        [Obsolete("此字段已弃用")]
        GroupBuying = 5
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PaySettleCompany
    {
        /// <summary>
        /// 上海
        /// </summary>
        SH = 3270,
        /// <summary>
        /// 北京
        /// </summary>
        BJ = 3271
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PaySettleITCompany
    {
        /// <summary>
        /// 经上海中转到
        /// </summary>
        SH,
        /// <summary>
        /// 经北京中转到
        /// </summary>
        BJ
    }

    /// <summary>
    /// 代销结算公司
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum TransferType
    {
        /// <summary>
        /// 直送
        /// </summary>
        Direct = 0,
        /// <summary>
        /// 中转
        /// </summary>
        Indirect = 1
    }

    /// <summary>
    ///  结算单Check逻辑状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum SettlementVerifyType
    {
        /// <summary>
        /// 新建结算单
        /// </summary>
        CREATE,
        /// <summary>
        /// 结算结算单
        /// </summary>
        SETTLE,
        /// <summary>
        /// 更新结算单
        /// </summary>
        UPDATE
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignToAccountType
    {
        /// <summary>
        /// 销售单
        /// </summary>
        SO = 0,
        /// <summary>
        /// 物流拒收
        /// </summary>
        //ShippingRefuse = 1,
        /// <summary>
        /// Manual
        /// </summary>
        Manual = 1,
        /// <summary>
        /// 损益单
        /// </summary>
        Adjust = 2,
        /// <summary>
        /// RMA发货
        /// </summary>
        RMA = 3
        /// <summary>
        /// 正常品入库
        /// </summary>
        //Normal = 4

        /// <summary>
        /// 补偿退款单
        /// </summary>
        //RO_Adjust = 6
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum GatherSettleType
    {
        /// <summary>
        /// 销售单
        /// </summary>
        SO,
        /// <summary>
        /// RMA发货
        /// </summary>
        RMA
        ///// <summary>
        ///// 补偿退款单
        ///// </summary>
        //RO_Adjust
    }

    /// <summary>
    /// 采购单类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 负采购
        /// </summary>
        Negative = 1,
        ///// <summary>
        ///// 历史负采购
        ///// </summary>
        //HistoryNegative = 2,
        /// <summary>
        /// 调价单
        /// </summary>
        Adjust = 3
    }

    /// <summary>
    /// 采购单代销类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderConsignFlag
    {
        /// <summary>
        /// 经销
        /// </summary>
        UnConsign = 0,

        /// <summary>
        /// 代销
        /// </summary>
        Consign = 1,

        ///// <summary>
        ///// 临时代销(仅仅为和老数据兼容)
        ///// </summary>
        //TempConsign = 2,

        /// <summary>
        /// 代收代付 2012.11.20
        /// </summary>
        GatherPay = 4


    }

    /// <summary>
    /// PO单操作权限
    /// </summary>
    public enum PurchaseOrderPrivilege
    {
        /// <summary>
        /// 审核采购单(最高权限)
        /// </summary>
        CanAuditAll = 0,

        /// <summary>
        /// 审核采购单(中级权限)
        /// </summary>
        CanAuditGeneric = 1,

        /// <summary>
        /// 审核采购单(一般权限)
        /// </summary>
        CanAuditLow = 2,

        /// <summary>
        /// 具有审核负采购单的权限
        /// </summary>
        CanAuditNegativeStock = 3,

        /// <summary>
        /// 具有对滞收发票PM的权限审核
        /// </summary>
        CanAuditLagInvoice = 4,

        /// <summary>
        /// 具有对采购单审核-发票超期权限
        /// </summary>
        CanAuditInvoiceAbsent = 5,
        /// <summary>
        /// 审核滞销收货PO权限
        /// </summary>
        CanAuditLagGoods = 6
    }

    /// <summary>
    /// PO检查类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderActionType
    {
        /// <summary>
        /// 检查
        /// </summary>
        Check,

        /// <summary>
        /// 审批
        /// </summary>
        Audit
    }

    /// <summary>
    /// 采购单操作类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderOperationType
    {
        /// <summary>
        /// 创建
        /// </summary>
        Create = 0,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 1,
        /// <summary>
        /// 更新CS备注
        /// </summary>
        UpDateCSMemo = 2,
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = 3,
    }

    /// <summary>
    /// 采购单 - 是否分摊
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderIsApportionType
    {
        /// <summary>
        /// 是
        /// </summary>
        No = 0,
        /// <summary>
        /// 否
        /// </summary>
        Yes = 1
    }

    /// <summary>
    /// 采购单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderStatus
    {
        /// <summary>
        /// 初始化=-3
        /// </summary>
        Origin = -3,
        /// <summary>
        /// 已退回=-2
        /// </summary>
        Returned = -2,
        /// <summary>
        /// 自动作废=-1
        /// </summary>
        AutoAbandoned = -1,
        /// <summary>
        /// 已作废=0
        /// </summary>
        Abandoned = 0,
        /// <summary>
        /// 已创建=1
        /// </summary>
        Created = 1,

        /// <summary>
        /// 待审核=5
        /// </summary>
        WaitingAudit = 5,
        /// <summary>
        /// 待申报
        /// </summary>
        //[Obsolete("此字段已弃用")]
        //WaitingReport = 11,
        /// <summary>
        /// 已申报
        /// </summary>
        //[Obsolete("此字段已弃用")]
        //Request = 12,
        ///// <summary>
        ///// 待分摊=2
        ///// </summary>
        //WaitingApportion = 2,
        /// <summary>
        /// 待入库=3
        /// </summary>
        WaitingInStock = 3,
        /// <summary>
        /// 已入库=4
        /// </summary>
        InStocked = 4,
        /// <summary>
        /// 部分入库=6
        /// </summary>
        PartlyInStocked = 6,
        /// <summary>
        /// 手动关闭=7
        /// </summary>
        ManualClosed = 7,
        /// <summary>
        /// 系统关闭=8
        /// </summary>
        SystemClosed = 8,
        /// <summary>
        /// 供应商关闭=9
        /// </summary>
        VendorClosed = 9,
        /// <summary>
        /// 待PM确认=10
        /// </summary>
        WaitingPMConfirm = 10
    }

    /// <summary>
    /// POItem的检查状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrdeItemCheckStatus
    {
        /// <summary>
        /// 未检查
        /// </summary>
        UnCheck = -1,

        /// <summary>
        ///已检查(符合)
        /// </summary>
        Accordant = 0,

        /// <summary>
        /// 已检查(不符合)
        /// </summary>
        UnAccordant = 1
    }

    /// <summary>
    /// 虚库采购单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VirtualPurchaseOrderStatus
    {
        /// <summary>
        /// 初始
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 关闭
        /// </summary>
        Close = 1,
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1
    }

    /// <summary>
    /// 虚库采购单 - 单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VirtualPurchaseInStockOrderType
    {
        /// <summary>
        /// 采购单
        /// </summary>
        PO = 0,
        /// <summary>
        /// 移仓单
        /// </summary>
        Shift = 1,
        /// <summary>
        /// 转换单
        /// </summary>
        Convert = 2
    }

    /// <summary>
    /// 待审核状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderVerifyStatus
    {
        /// <summary>
        /// 待TL审核
        /// </summary>
        WaitingTLAudit = 1,
        /// <summary>
        /// 待PMD审核
        /// </summary>
        WaitingPMDAudit = 2
    }

    /// <summary>
    /// PO SSB消息类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderSSBMsgType
    {
        /// <summary>
        /// 创建
        /// </summary>
        I,

        /// <summary>
        /// 更新
        /// </summary>
        U,

        /// <summary>
        /// 关闭
        /// </summary>
        C
    }

    /// <summary>
    /// AccountLog状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignToAccountLogStatus
    {
        /// <summary>
        /// 待结算
        /// </summary>
        Origin,
        Finance,
        /// <summary>
        /// 已结算
        /// </summary>
        Settled,
        /// <summary>
        /// 系统已建
        /// </summary>
        SystemCreated,
        /// <summary>
        /// 人工已建
        /// </summary>
        ManualCreated
    }

    /// <summary>
    /// AccountLog状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum POCollectionPaymentSettleStatus
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
        /// 已审核
        /// </summary>
        Audited = 1,

        /// <summary>
        /// 已结算
        /// </summary>
        Settled = 2
    }

    /// <summary>
    /// 财务审核状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorFinanceRequestStatus
    {
        /// <summary>
        /// 审核通过
        /// </summary>
        AuditPassed = 1,
        /// <summary>
        /// 申请状态
        /// </summary>
        AuditWaiting = 0,
        /// <summary>
        /// 审核未通过
        /// </summary>
        AuditDenied = -1,
        /// <summary>
        /// 取消审核
        /// </summary>
        AuditCanceled = -2,
    }

    /// <summary>
    /// 供应商状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorStatus
    {
        /// <summary>
        /// 不可用
        /// </summary>
        UnAvailable = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitApprove = -2,
        /// <summary>
        /// 审核通过，可用
        /// </summary>
        Available = 0
    }

    /// <summary>
    /// 供应商 - 是否合作
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorIsCooperate
    {
        /// <summary>
        /// 是
        /// </summary>
        Yes = 1,
        /// <summary>
        /// 否
        /// </summary>
        No = 0
    }

    /// <summary>
    /// 供应商 - 是否转租赁
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorIsToLease
    {
        /// <summary>
        /// 是
        /// </summary>
        Lease = 1,
        /// <summary>
        /// 否
        /// </summary>
        UnLease = 0
    }

    /// <summary>
    /// 供应商等级状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorRankStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        AuditWaiting = 0
    }

    /// <summary>
    /// 商家类型（原:供应商开票方式）
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorInvoiceType
    {
        /// <summary>
        /// 平台经销供应商（原:泰隆优选开票）
        /// </summary>
        NEG,
        /// <summary>
        /// 平台代销供应商（原:商家开票）
        /// </summary>
        MET,
        /// <summary>
        /// 导购模式
        /// </summary>
        [Obsolete("此字段已弃用")]
        GUD
    }

    /// <summary>
    /// 供应商仓储方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorStockType
    {
        /// <summary>
        /// 自贸仓（原：泰隆仓储）
        /// </summary>
        NEG,
        /// <summary>
        /// 直邮（原：商家仓储）
        /// </summary>
        MET,
        /// <summary>
        /// 自贸仓+直邮
        /// </summary>
        [Obsolete("此字段已弃用")]
        NAM
    }

    /// <summary>
    /// 供应商配送方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorShippingType
    {
        /// <summary>
        /// 泰隆优选配送
        /// </summary>
        //[Obsolete("此字段已弃用")]
        NEG,
        /// <summary>
        /// 商家配送
        /// </summary>
        MET
    }

    /// <summary>
    /// 供应商合同信息
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorContractType
    {
        BeingSold = 1,
        Consign = 2,
        AfterSold = 3
    }

    /// <summary>
    /// 生产商状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ManufacturerStatus
    {
        Available = 0,
        UnAvailable = -1
    }

    /// <summary>
    /// 默认送货分仓
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorDefaultShippingStock
    {
        SH_BaoShan = 99
    }

    /// <summary>
    /// 佣金单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorCommissionMasterStatus
    {
        /// <summary>
        /// 已关闭
        /// </summary>
        CLS = 3,
        /// <summary>
        /// 已出单
        /// </summary>
        SET = 2,
        /// <summary>
        /// 待出单
        /// </summary>
        ORG = 1
    }

    /// <summary>
    /// 佣金状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorCommissionSettleStatus
    {
        /// <summary>
        /// 已支付
        /// </summary>
        Pay = 2,
        /// <summary>
        /// 未支付
        /// </summary>
        UnPay = 0,
        /// <summary>
        /// 部分支付
        /// </summary>
        PartPay = 1,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1
    }

    /// <summary>
    /// 佣金分类类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorCommissionItemType
    {
        /// <summary>
        /// 销售提成
        /// </summary>
        SAC,
        /// <summary>
        /// 订单提成
        /// </summary>
        SOC,
        /// <summary>
        /// 配送费用
        /// </summary>
        DEF
    }

    /// <summary>
    /// 供应商退款单状态:
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorRefundStatus
    {
        /// <summary>
        /// 废弃
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 初始状态
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已提交审核
        /// </summary>
        Verify = 1,
        /// <summary>
        /// PM已审核
        /// </summary>
        PMVerify = 2,
        /// <summary>
        /// PMD已审核
        /// </summary>
        PMDVerify = 3,
        /// <summary>
        /// 待PMCC审核
        /// </summary>
        [Obsolete("此字段已弃用")]
        PMCCToVerify = 4,
        /// <summary>
        /// PMCC已审核
        /// </summary>
        [Obsolete("此字段已弃用")]
        PMCCVerify = 5
    }

    /// <summary>
    /// 供应商付款方式(供应商退款单)
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorRefundPayType
    {
        /// <summary>
        /// 现金支付
        /// </summary>
        CashPay = 1,
        /// <summary>
        /// 应收款抵扣
        /// </summary>
        Deduction = 2
    }

    /// <summary>
    /// 结算类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum SettleType
    {
        /// <summary>
        /// 传统结算
        /// </summary>
        O,
        /// <summary>
        /// 佣金百分比结算
        /// </summary>
        P
    }

    /// <summary>
    /// 结算单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum SettleStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit = 1,
        /// <summary>
        /// 已审核
        /// </summary>
        AuditPassed = 2,
        /// <summary>
        /// 已结算
        /// </summary>
        SettlePassed = 3
    }

    /// <summary>
    /// 代收结算单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum GatherSettleStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        ABD = 1,
        /// <summary>
        /// 待审核
        /// </summary>
        ORG = 2,
        /// <summary>
        /// 已审核
        /// </summary>
        AUD = 3,
        /// <summary>
        /// 已结算
        /// </summary>
        SET = 4
    }

    /// <summary>
    /// 自动结算状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum AutoSettleStatus
    {
        /// <summary>
        /// 自动
        /// </summary>
        Auto,
        /// <summary>
        /// 手动
        /// </summary>
        Hand
    }

    /// <summary>
    /// 供应商审核状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorModifyRequestStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        Apply = 0,
        /// <summary>
        /// 审核通过
        /// </summary>
        VerifyPass = 1,
        /// <summary>
        /// 审核未通过
        /// </summary>
        VerifyUnPass = -1,
        /// <summary>
        /// 取消审核
        /// </summary>
        CancelVerify = -2,
        /// <summary>
        /// 普通（日志记录）
        /// </summary>
        Common = -3,
        /// <summary>
        /// 历史遗留状态(无实际意义)
        /// </summary>
        HistoryStatus = 2
    }

    /// <summary>
    /// 供应商审核申请类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorModifyRequestType
    {
        /// <summary>
        /// 财务信息
        /// </summary>
        Finance = 0,
        /// <summary>
        /// 基本信息
        /// </summary>
        Vendor = 1,
        /// <summary>
        /// 代理信息
        /// </summary>
        Manufacturer = 2,
        /// <summary>
        /// 售后信息
        /// </summary>
        AfterSale = 3,
        /// <summary>
        /// 下单日期
        /// </summary>
        BuyWeekDay = 4,
        ///手工添加
        Manual = 5
    }

    /// <summary>
    ///  供应商请求 - 操作类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorModifyActionType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal,
        //新增
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 其它
        /// </summary>
        Other
    }

    /// <summary>
    /// 供应商货币编号
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorCurrencyCode
    {
        CNY
    }

    public enum VendorRowState
    {
        Unchanged,

        Added,

        Deleted,

        Modified
    }

    /// <summary>
    /// 增值税率类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderTaxRate
    {
        /// <summary>
        /// 0%
        /// </summary>
        Percent000 = 0,
        /// <summary>
        /// 4%
        /// </summary>
        Percent004 = 4,
        /// <summary>
        /// 6%
        /// </summary>
        Percent006 = 6,
        /// <summary>
        /// 13%
        /// </summary>
        Percent013 = 13,
        /// <summary>
        /// 17%
        /// </summary>
        Percent017 = 17,
    }

    /// <summary>
    /// 预计到货时间段类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderETAHalfDayType
    {
        /// <summary>
        /// 上午
        /// </summary>
        AM,
        /// <summary>
        /// 下午
        /// </summary>
        PM
    }

    /// <summary>
    /// 供应商财务结算方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorSettlePeriodType
    {
        /// <summary>
        /// 手工结算
        /// </summary>
        Manual = 1,
        /// <summary>
        /// 本月结，每月10/25日
        /// </summary>
        PerMonth = 2
    }

    /// <summary>
    /// 佣金 单据类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorCommissionReferenceType
    {
        /// <summary>
        /// 销售单
        /// </summary>
        SO,
        /// <summary>
        /// RMA单
        /// </summary>
        RMA
    }

    /// <summary>
    /// 代销商品规则状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignSettleRuleStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        Wait_Audit = 'O',

        /// <summary>
        /// 未生效
        /// </summary>
        Available = 'A',

        /// <summary>
        /// 已生效
        /// </summary>
        Enable = 'E',

        /// <summary>
        /// 已过期
        /// </summary>
        Disable = 'D',

        /// <summary>
        /// 已终止
        /// </summary>
        Stop = 'S',

        /// <summary>
        /// 已作废
        /// </summary>
        Forbid = 'F'
    }

    /// <summary>
    /// 代销商品规则操作类型
    /// </summary>
    public enum ConsignSettleRuleActionType
    {
        Add,
        Update,
        Audit,
        Abandon,
        Stop
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignSettleCurrencyCode
    {
        //HK = 5,
        //Korea = 10,
        //Lila = 8,
        //Lb = 7,
        Dollar = 2,
        //Uro = 4,
        RMB = 1
        //Jp = 6,
        //Thai = 11,
        //Sing = 9,
        //Pound = 3
    }

    /// <summary>
    /// 合同返利费用类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderEIMSRuleType
    {
        /// <summary>
        /// 市场发展基金()
        /// </summary>
        MDF = 101,
        /// <summary>
        /// 合同返利(VIR)
        /// </summary>
        VIR = 102,
        /// <summary>
        /// 商品管理等费用(Co-op)
        /// </summary>
        COOP = 103,
        /// <summary>
        /// 销售返点(SR)
        /// </summary>
        SR = 104,
        /// <summary>
        /// 价保(PP)
        /// </summary>
        PP = 105,
        /// <summary>
        /// 日常进货奖励(POR)
        /// </summary>
        POR = 106,
        /// <summary>
        /// 市场活动专项费(MKT_ACTIVITY)
        /// </summary>
        MKT_ACTIVITY = 130,
        /// <summary>
        /// 现金返点(CR)
        /// </summary>
        CR = 131,
        /// <summary>
        /// 运费返利(FRF)
        /// </summary>
        FRF = 133
    }

    /// <summary>
    /// PO单商品退货批次- 状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderBatchInfoStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        A,
        /// <summary>
        /// 过期
        /// </summary>
        I,
        /// <summary>
        /// 临期
        /// </summary>
        R
    }

    /// <summary>
    /// 商家类型
    /// </summary>
    public enum VendorType
    {
        /// <summary>
        /// 来自IPP
        /// </summary>
        IPP = 0,
        /// <summary>
        /// 来自VendorPortal
        /// </summary>
        VendorPortal = 1
    }

    /// <summary>
    /// 产出返点计算类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignSettleReturnPointCalcType
    {
        /// <summary>
        /// 返点金额
        /// </summary>
        ReturnPoint_Amt = 0,
        /// <summary>
        /// 返点百分比
        /// </summary>
        ReturnPoint_Percentage = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum EIMSInvoiceStatus
    {
        /// <summary>
        /// 全部收到
        /// </summary>
        Full = 1,
        /// <summary>
        /// 手动关闭
        /// </summary>
        Manual = 2,
        /// <summary>
        /// 打开
        /// </summary>
        Open = 3,
        /// <summary>
        /// 作废
        /// </summary>
        Void = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignToAccStatus
    {
        Origin = 0,
        FinanceSettled = 1,
        VendorSettled = 2,
        SystemCreated = 3,
        ManualCreated = 4,
    }
    /// <summary>
    /// 转租赁标志
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum PurchaseOrderLeaseFlag
    {
        /// <summary>
        /// 否
        /// </summary>
        unLease,
        /// <summary>
        /// 是
        /// </summary>
        Lease
    }

    /// <summary>
    /// 扣款类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum DeductType
    {
        /// <summary>
        /// 合同
        /// </summary>
        Contract = 0,
        /// <summary>
        /// 临时
        /// </summary>
        Temp = 1
    }

    /// <summary>
    /// 记账类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum AccountType
    {
        /// <summary>
        /// 成本
        /// </summary>
        Cost = 0,
        /// <summary>
        ///费用
        /// </summary>
        Fee = 1
    }

    /// <summary>
    /// 扣款方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum DeductMethod
    {
        /// <summary>
        /// 货扣
        /// </summary>
        UnCash = 0,
        /// <summary>
        ///现金
        /// </summary>
        Cash = 1
    }

    /// <summary>
    /// 状态（扣款项）
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum Status
    {
        /// <summary>
        /// 有效
        /// </summary>
        Effective = 0,
        /// <summary>
        /// 作废
        /// </summary>
        Invalid = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorCalcType
    {
        /// <summary>
        /// 固定金额
        /// </summary>
        Fix = 0,
        /// <summary>
        /// 成本金额
        /// </summary>
        Cost = 1,
        /// <summary>
        /// 销售金额
        /// </summary>
        Sale = 2
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum ConsignAdjustStatus
    {
        Abandon = -1,

        WaitAudit = 0,

        Audited = 1
    }

    /// <summary>
    /// 成本变价单 - 状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum CostChangeStatus
    {
        /// <summary>
        /// 已创建
        /// </summary>
        Created = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingForAudited = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        Audited = 2,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandoned = -1,
    }

    /// <summary>
    /// 结算单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum POSettleStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 已创建
        /// </summary>
        Created = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        AuditPassed = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum CommissionRuleStatus
    {
        Deactive,
        Active
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum StoreBrandFilingStatus
    {
        Deactive = 0,
        Draft = 1,
        Approved = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum StoreStatus
    {
        //草稿
        Draft= 1,
        //待审核 
        Checking = 2,
        //已审核 
        Checked = 3,
        //审核未通过 
        Unchecked = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum EPortStatusENUM
    {
        /// <summary>
        /// 有效
        /// </summary>
        Active = 1,
        /// <summary>
        /// 无效
        /// </summary>
        Inactive = 0
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum EPortShippingTypeENUM
    {
        /// <summary>
        /// 保税区
        /// </summary>
        BondedArea = 0,
    }
}
