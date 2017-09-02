using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.ControlPannel;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class PurchaseOrderBasicInfo
    {
        /// <summary>
        /// 采购单编号
        /// </summary>
        public string PurchaseOrderID { get; set; }

        /// <summary>
        /// 账期属性
        /// </summary>
        public PurchaseOrderConsignFlag? ConsignFlag { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// 预计付费时间
        /// </summary>
        public DateTime? ETP { get; set; }

        /// <summary>
        ///  总价格
        /// </summary>
        public decimal? TotalAmt { get; set; }

        /// <summary>
        /// 计划采购数量
        /// </summary>
        public int? PlanedPurchaseQty { get; set; }

        /// <summary>
        /// 入库总数量
        /// </summary>
        public decimal? TotalQty { get; set; }

        /// <summary>
        /// 入库总金额
        /// </summary>
        public decimal? TotalActualPrice { get; set; }

        /// <summary>
        /// 计划报关数量
        /// </summary>
        public int? TotalPrePurchaseQty { get; set; }

        /// <summary>
        /// 采购单类型(正常，负采购,历史负采购...)
        /// </summary>
        public PurchaseOrderType? PurchaseOrderType { get; set; }

        /// <summary>
        ///  采购单状态
        /// </summary>
        public PurchaseOrderStatus? PurchaseOrderStatus { get; set; }

        /// <summary>
        /// 采购单待审核状态
        /// </summary>
        public int? PurchaseOrderTPStatus { get; set; }

        /// <summary>
        /// 采购单ExecptStatus
        /// </summary>
        public int? PurchaseOrderExceptStatus { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal? TaxRate { get; set; }

        /// <summary>
        /// 增值税率类型
        /// </summary>
        public PurchaseOrderTaxRate? TaxRateType { get; set; }

        /// <summary>
        /// 结算货币编号
        /// </summary>
        public int? CurrencyCode { get; set; }

        /// <summary>
        /// 结算货币名称
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// 结算货币符号
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 采购入库渠道仓库信息
        /// </summary>
        public StockInfo StockInfo { get; set; }

        public int? PurchaseStockSysNo { get; set; }

        /// <summary>
        /// 预计到货时间(ETATime)信息
        /// </summary>
        public PurchaseOrderETATimeInfo ETATimeInfo { get; set; }

        /// <summary>
        /// 采购单创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 采购单确认时间
        /// </summary>
        public DateTime? ConfirmTime { get; set; }

        /// <summary>
        /// 采购单创建人编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 采购单创建人名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime? InTime { get; set; }

        /// <summary>
        /// 入库人系统编号
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 是否撤销
        /// </summary>
        public int? IsApportion { get; set; }

        /// <summary>
        /// 撤销人系统编号
        /// </summary>
        public int? ApportionUserSysNo { get; set; }

        /// <summary>
        /// 撤销时间
        /// </summary>
        public DateTime? ApportionTime { get; set; }

        /// <summary>
        /// Email地址
        /// </summary>
        public string MailAddress { get; set; }

        /// <summary>
        /// 自动发送邮件地址
        /// </summary>
        public string AutoSendMailAddress { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 审核人编号
        /// </summary>
        public int? AuditUserSysNo { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 采购单检查结果
        /// </summary>
        public string CheckResult { get; set; }

        /// <summary>
        /// 备忘录信息
        /// </summary>
        public PurchaseOrderMemoInfo MemoInfo { get; set; }
    }
}
