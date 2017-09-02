using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.PO.Models
{
    public class SettlementRequestBaseVM : ModelBase
    {
        public SettlementRequestBaseVM()
        {
            vendorInfo = new VendorInfoVM();
        }

        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        private string referenceSysNo;

        public string ReferenceSysNo
        {
            get { return referenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref referenceSysNo, value); }
        }

        /// <summary>
        /// 供应商信息
        /// </summary>
        private VendorInfoVM vendorInfo;

        public VendorInfoVM VendorInfo
        {
            get { return vendorInfo; }
            set { base.SetValue("VendorInfo", ref vendorInfo, value); }
        }

        /// <summary>
        /// 源渠道仓库 -编号
        /// </summary>
        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }


        /// <summary>
        /// 源渠道仓库 -编号
        /// </summary>
        private string stockID;

        public string StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }

        /// <summary>
        /// 源渠道仓库 -名称
        /// </summary>
        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        /// <summary>
        /// 税率
        /// </summary>
        private PurchaseOrderTaxRate? taxRateData;
        [Validate(ValidateType.Required)]
        public PurchaseOrderTaxRate? TaxRateData
        {
            get { return taxRateData; }
            set { base.SetValue("TaxRateData", ref taxRateData, value); }
        }

        /// <summary>
        /// 货币类型
        /// </summary>
        private string currencyCode;
        [Validate(ValidateType.Required)]
        public string CurrencyCode
        {
            get { return currencyCode; }
            set { base.SetValue("CurrencyCode", ref currencyCode, value); }
        }

        /// <summary>
        /// 结算总金额
        /// </summary>
        private decimal? totalAmt;

        public decimal? TotalAmt
        {
            get { return totalAmt; }
            set { base.SetValue("TotalAmt", ref totalAmt, value); }
        }

        /// <summary>
        /// 结算单状态
        /// </summary>
        private SettleStatus? status;

        public SettleStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 调整单编号
        /// </summary>
        private string settleBalanceSysNo;

        public string SettleBalanceSysNo
        {
            get { return settleBalanceSysNo; }
            set { base.SetValue("SettleBalanceSysNo", ref settleBalanceSysNo, value); }
        }

        /// <summary>
        /// 创建时成本
        /// </summary>
        private decimal? createCostTotalAmt;

        public decimal? CreateCostTotalAmt
        {
            get { return createCostTotalAmt; }
            set { base.SetValue("CreateCostTotalAmt", ref createCostTotalAmt, value); }
        }

        /// <summary>
        /// 差额
        /// </summary>
        private decimal? difference;

        public decimal? Difference
        {
            get { return difference; }
            set { base.SetValue("Difference", ref difference, value); }
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        private DateTime? createDate;
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        /// <summary>
        /// 创建日期 -从
        /// </summary>
        private DateTime? createDateFrom;
        public DateTime? CreateDateFrom
        {
            get { return createDateFrom; }
            set { base.SetValue("CreateDate", ref createDateFrom, value); }
        }

        /// <summary>
        /// 创建日期 -到
        /// </summary>
        private DateTime? createDateTo;
        public DateTime? CreateDateTo
        {
            get { return createDateTo; }
            set { base.SetValue("CreateDateTo", ref createDateTo, value); }
        }

        private DateTime? outStockRefundDateFrom;
        public DateTime? OutStockRefundDateFrom
        {
            get { return outStockRefundDateFrom; }
            set { base.SetValue("OutStockRefundDateFrom", ref outStockRefundDateFrom, value); }
        }

        private DateTime? outStockRefundDateTo;
        public DateTime? OutStockRefundDateTo
        {
            get { return outStockRefundDateTo; }
            set { base.SetValue("OutStockRefundDateTo", ref outStockRefundDateTo, value); }
        }

        /// <summary>
        /// 结算所属PM信息 -编号
        /// </summary>
        private string pMSysNo;
        [Validate(ValidateType.Required)]
        public string PMSysNo
        {
            get { return pMSysNo; }
            set { base.SetValue("PMSysNo", ref pMSysNo, value); }
        }

        /// <summary>
        /// 结算所属PM信息 - 名称
        /// </summary>
        private int? pMDisplayName;

        public int? PMDisplayName
        {
            get { return pMDisplayName; }
            set { base.SetValue("PMDisplayName", ref pMDisplayName, value); }
        }

        /// <summary>
        /// 毛利总金额
        /// </summary>
        private decimal? rateMarginCount;

        public decimal? RateMarginCount
        {
            get { return rateMarginCount; }
            set { base.SetValue("RateMarginCount", ref rateMarginCount, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string memo;

        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }

        /// <summary>
        /// 记录
        /// </summary>
        private string note;

        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        /// <summary>
        /// 审核人编号
        /// </summary>
        private int? auditUserSysNo;

        public int? AuditUserSysNo
        {
            get { return auditUserSysNo; }
            set { base.SetValue("AuditUserSysNo", ref auditUserSysNo, value); }
        }
        /// <summary>
        /// 审核人名称
        /// </summary>
        private int? auditUserName;

        public int? AuditUserName
        {
            get { return auditUserName; }
            set { base.SetValue("AuditUserName", ref auditUserName, value); }
        }

        /// <summary>
        /// 审核日期
        /// </summary>
        private DateTime? auditDate;

        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { base.SetValue("AuditDate", ref auditDate, value); }
        }

        /// <summary>
        /// 出库人编号
        /// </summary>
        private int? outStockUserSysNo;

        public int? OutStockUserSysNo
        {
            get { return outStockUserSysNo; }
            set { base.SetValue("OutStockUserSysNo", ref outStockUserSysNo, value); }
        }
        /// <summary>
        /// 出库人名称
        /// </summary>
        private string outStockUserName;

        public string OutStockUserName
        {
            get { return outStockUserName; }
            set { base.SetValue("OutStockUserName", ref outStockUserName, value); }
        }


        /// <summary>
        /// 出库日期
        /// </summary>
        private DateTime? outStockDate;

        public DateTime? OutStockDate
        {
            get { return outStockDate; }
            set { base.SetValue("OutStockDate", ref outStockDate, value); }
        }

        /// <summary>
        /// 结算单编号
        /// </summary>
        private string settleID;

        public string SettleID
        {
            get { return settleID; }
            set { base.SetValue("SettleID", ref settleID, value); }
        }

        /// <summary>
        /// 结算人
        /// </summary>
        private int? settleUserSysNo;

        public int? SettleUserSysNo
        {
            get { return settleUserSysNo; }
            set { base.SetValue("SettleUserSysNo", ref settleUserSysNo, value); }
        }
        private string settleUserName;

        public string SettleUserName
        {
            get { return settleUserName; }
            set { base.SetValue("SettleUserName", ref settleUserName, value); }
        }

        /// <summary>
        /// 结算日期
        /// </summary>
        private DateTime? settleDate;

        public DateTime? SettleDate
        {
            get { return settleDate; }
            set { base.SetValue("SettleDate", ref settleDate, value); }
        }

        private VendorIsToLease? leaseType;
        public VendorIsToLease? LeaseType
        {
            get { return leaseType; }
            set { base.SetValue("LeaseType", ref leaseType, value); }
        }

        public string LeaseTypeDisplay
        {
            get
            {
                if (LeaseType != null)
                {
                    return EnumConverter.GetDescription(LeaseType.Value);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public DeductMethod? DeductMethod { get; set; }

        public AccountType? DeductAccountType { get; set; }

        private decimal deductAmt;
        public decimal DeductAmt { 
          get { return deductAmt; }
            set { base.SetValue("DeductAmt", ref deductAmt, value); }
        }

        public string ConsignRange { get; set; }

        public string DeductMethodDisplay
        {
            get
            {
                if (DeductMethod.HasValue)
                {
                    return DeductMethod.Value.ToDescription();
                }
                return string.Empty;
            }           
        }

        public string DeductAccountTypeDisplay {
            get
            {
                if (DeductAccountType.HasValue)
                {
                    return DeductAccountType.Value.ToDescription();
                }
                return string.Empty;
            }

        }
    }
}
