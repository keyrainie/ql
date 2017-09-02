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
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderBasicInfoVM : ModelBase
    {

        public PurchaseOrderBasicInfoVM()
        {
            eTATimeInfo = new PurchaseOrderETATimeInfoVM();
            memoInfo = new PurchaseOrderMemoInfoVM();
            privilege = new List<PurchaseOrderPrivilege>();
        }

        /// <summary>
        /// 采购单编号
        /// </summary>
        private string purchaseOrderID;

        public string PurchaseOrderID
        {
            get { return purchaseOrderID; }
            set { this.SetValue("PurchaseOrderID", ref purchaseOrderID, value); }
        }

        private List<PurchaseOrderPrivilege> privilege;

        public List<PurchaseOrderPrivilege> Privilege
        {
            get { return privilege; }
            set { this.SetValue("Privilege", ref privilege, value); }
        }

        /// <summary>
        /// 合同号
        /// </summary>
        private string contractNumber;

        public string ContractNumber
        {
            get { return contractNumber; }
            set { this.SetValue("ContractNumber", ref contractNumber, value); }
        }

        /// <summary>
        /// 账期属性
        /// </summary>
        private PurchaseOrderConsignFlag? consignFlag;

        public PurchaseOrderConsignFlag? ConsignFlag
        {
            get { return consignFlag; }
            set { this.SetValue("ConsignFlag", ref consignFlag, value); }
        }

        /// <summary>
        /// 结算公司编号
        /// </summary>
        private int? settleCompanySysNo;

        public int? SettleCompanySysNo
        {
            get { return settleCompanySysNo; }
            set { this.SetValue("SettleCompanySysNo", ref settleCompanySysNo, value); }
        }

        /// <summary>
        /// 结算公司名称
        /// </summary>
        private string settleCompanyName;

        public string SettleCompanyName
        {
            get { return settleCompanyName; }
            set { this.SetValue("SettleCompanyName", ref settleCompanyName, value); }
        }


        /// <summary>
        ///  到付运费金额
        /// </summary>
        private string carriageCost;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string CarriageCost
        {
            get { return carriageCost; }
            set { this.SetValue("CarriageCost", ref carriageCost, value); }
        }

        private decimal? exchangeRate;

        public decimal? ExchangeRate
        {
            get { return exchangeRate; }
            set { this.SetValue("ExchangeRate", ref exchangeRate, value); }
        }


        private decimal? usingReturnPoint;

        public decimal? UsingReturnPoint
        {
            get { return usingReturnPoint; }
            set { this.SetValue("UsingReturnPoint", ref usingReturnPoint, value); }
        }

        private int? returnPointC3SysNo;

        public int? ReturnPointC3SysNo
        {
            get { return returnPointC3SysNo; }
            set { this.SetValue("ReturnPointC3SysNo", ref returnPointC3SysNo, value); }
        }

        private int? pM_ReturnPointSysNo;

        public int? PM_ReturnPointSysNo
        {
            get { return pM_ReturnPointSysNo; }
            set { this.SetValue("PM_ReturnPointSysNo", ref pM_ReturnPointSysNo, value); }
        }

        /// <summary>
        /// 预计付费时间
        /// </summary>
        private DateTime? eTP;

        public DateTime? ETP
        {
            get { return eTP; }
            set { this.SetValue("ETP", ref eTP, value); }
        }

        /// <summary>
        ///  总价格
        /// </summary>
        private decimal? totalAmt;

        public decimal? TotalAmt
        {
            get { return totalAmt; }
            set { this.SetValue("TotalAmt", ref totalAmt, value); }
        }

        public string TotalAmtDisplayString
        {
            get
            {
                return currencySymbol + (totalAmt.HasValue ? totalAmt.Value.ToString("f2") : "0.00");
            }
        }

        /// <summary>
        /// 计划采购数量
        /// </summary>
        private int? planedPurchaseQty;

        public int? PlanedPurchaseQty
        {
            get { return planedPurchaseQty; }
            set { this.SetValue("PlanedPurchaseQty", ref planedPurchaseQty, value); }
        }

        /// <summary>
        /// 入库总数量
        /// </summary>
        private decimal? totalQty;

        public decimal? TotalQty
        {
            get { return totalQty; }
            set { this.SetValue("TotalQty", ref totalQty, value); }
        }

        /// <summary>
        /// 入库总金额
        /// </summary>
        private decimal? totalActualPrice;

        public decimal? TotalActualPrice
        {
            get { return totalActualPrice; }
            set { this.SetValue("TotalActualPrice", ref totalActualPrice, value); }
        }


        public string TotalActualPriceDisplayString
        {
            get { return currencySymbol + (totalActualPrice.HasValue ? totalActualPrice.Value.ToString("f2") : "0.00"); }
        }

        /// <summary>
        /// 采购单类型(正常，负采购,历史负采购...)
        /// </summary>
        private PurchaseOrderType? purchaseOrderType;


        public PurchaseOrderType? PurchaseOrderType
        {
            get { return purchaseOrderType; }
            set { this.SetValue("PurchaseOrderType", ref purchaseOrderType, value); }
        }

        /// <summary>
        ///  采购单状态
        /// </summary>
        private PurchaseOrderStatus? purchaseOrderStatus;

        public PurchaseOrderStatus? PurchaseOrderStatus
        {
            get { return purchaseOrderStatus; }
            set { this.SetValue("PurchaseOrderStatus", ref purchaseOrderStatus, value); }
        }

        /// <summary>
        /// 采购单 TP Status
        /// </summary>
        private int? purchaseOrderTPStatus;

        public int? PurchaseOrderTPStatus
        {
            get { return purchaseOrderTPStatus; }
            set { this.SetValue("PurchaseOrderTPStatus", ref purchaseOrderTPStatus, value); }
        }

        private int? purchaseOrderExceptStatus;

        public int? PurchaseOrderExceptStatus
        {
            get { return purchaseOrderExceptStatus; }
            set { this.SetValue("PurchaseOrderExceptStatus", ref purchaseOrderExceptStatus, value); }
        }

        /// <summary>
        /// 增值税率
        /// </summary>
        private decimal? taxRate;

        public decimal? TaxRate
        {
            get { return taxRate; }
            set { this.SetValue("TaxRate", ref taxRate, value); }
        }

        private PurchaseOrderTaxRate? taxRateType;

        public PurchaseOrderTaxRate? TaxRateType
        {
            get { return taxRateType; }
            set { this.SetValue("TaxRateType", ref taxRateType, value); }
        }
        /// <summary>
        /// 结算货币编号
        /// </summary>
        private string currencyCode;
        [Validate(ValidateType.Required)]
        public string CurrencyCode
        {
            get { return currencyCode; }
            set { this.SetValue("CurrencyCode", ref currencyCode, value); }
        }
        /// <summary>
        /// 结算货币名称
        /// </summary>
        private string currencyName;

        public string CurrencyName
        {
            get { return currencyName; }
            set { this.SetValue("CurrencyName", ref currencyName, value); }
        }

        /// <summary>
        /// 结算货币符号
        /// </summary>
        private string currencySymbol;

        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set { this.SetValue("CurrencySymbol", ref currencySymbol, value); }

        }




        private string source;

        public string Source
        {
            get { return source; }
            set { this.SetValue("Source", ref source, value); }
        }

        /// <summary>
        /// 采购入库渠道仓库信息
        /// </summary>
        //private StockInfo StockInfo ;

        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { this.SetValue("StockSysNo", ref stockSysNo, value); }
        }
        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { this.SetValue("StockName", ref stockName, value); }
        }

        private int? purchaseStockSysNo;

        public int? PurchaseStockSysNo
        {
            get { return purchaseStockSysNo; }
            set { this.SetValue("PurchaseStockSysNo", ref purchaseStockSysNo, value); }
        }

        /// <summary>
        /// 经中转的目标仓
        /// </summary>
        //private StockInfo ITStockInfo ;
        private int? iTStockSysNo;

        public int? ITStockSysNo
        {
            get { return iTStockSysNo; }
            set { this.SetValue("ITStockSysNo", ref iTStockSysNo, value); }
        }
        private string iTStockName;

        public string ITStockName
        {
            get { return iTStockName; }
            set { this.SetValue("ITStockName", ref iTStockName, value); }
        }
        /// <summary>
        /// 采购单归属PM
        /// </summary>
        //private ProductManagerInfo ProductManager ;
        private string pMSysNo;
        //CRL21776 所属PM为选择商品是自动带出,故无需验证非空
        //[Validate(ValidateType.Required)]
        public string PMSysNo
        {
            get { return pMSysNo; }
            set { this.SetValue("PMSysNo", ref pMSysNo, value); }
        }
        private string pMName;

        public string PMName
        {
            get { return pMName; }
            set { this.SetValue("PMName", ref pMName, value); }
        }

        /// <summary>
        /// 采购入库送货类型
        /// </summary>
        private string shippingTypeSysNo;

        [Validate(ValidateType.Required)]
        public string ShippingTypeSysNo
        {
            get { return shippingTypeSysNo; }
            set { this.SetValue("ShippingTypeSysNo", ref shippingTypeSysNo, value); }
        }

        private string shippingTypeName;

        public string ShippingTypeName
        {
            get { return shippingTypeName; }
            set { this.SetValue("ShippingTypeName", ref shippingTypeName, value); }
        }

        private int? payTypeSysNo;

        public int? PayTypeSysNo
        {
            get { return payTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref payTypeSysNo, value); }
        }

        private string payTypeName;

        public string PayTypeName
        {
            get { return payTypeName; }
            set { this.SetValue("PayTypeName", ref payTypeName, value); }
        }


        /// <summary>
        ///  移仓单编号
        /// </summary>
        private string shiftSysNo;

        public string ShiftSysNo
        {
            get { return shiftSysNo; }
            set { this.SetValue("ShiftSysNo", ref shiftSysNo, value); }
        }

        /// <summary>
        /// 预计到货时间(ETATime)信息
        /// </summary>
        private PurchaseOrderETATimeInfoVM eTATimeInfo;

        public PurchaseOrderETATimeInfoVM ETATimeInfo
        {
            get { return eTATimeInfo; }
            set { this.SetValue("ETATimeInfo", ref eTATimeInfo, value); }
        }

        /// <summary>
        /// 采购单创建日期
        /// </summary>
        private DateTime? createDate;

        public DateTime? CreateDate
        {
            get { return createDate; }
            set { this.SetValue("CreateDate", ref createDate, value); }
        }

        private DateTime? confirmTime;

        public DateTime? ConfirmTime
        {
            get { return confirmTime; }
            set { this.SetValue("ConfirmTime", ref confirmTime, value); }
        }

        /// <summary>
        /// 采购单创建人
        /// </summary>
        private int? createUserSysNo;

        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { this.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private int? inUserSysNo;

        public int? InUserSysNo
        {
            get { return inUserSysNo; }
            set { this.SetValue("InUserSysNo", ref inUserSysNo, value); }
        }


        /// <summary>
        /// 是否撤销
        /// </summary>
        private int? isApportion;

        public int? IsApportion
        {
            get { return isApportion; }
            set { this.SetValue("IsApportion", ref isApportion, value); }
        }

        private int? apportionUserSysNo;

        public int? ApportionUserSysNo
        {
            get { return apportionUserSysNo; }
            set { this.SetValue("ApportionUserSysNo", ref apportionUserSysNo, value); }
        }

        private string mailAddress;

        public string MailAddress
        {
            get { return mailAddress; }
            set { this.SetValue("MailAddress", ref mailAddress, value); }
        }

        /// <summary>
        /// 自动发送邮件地址
        /// </summary>
        private string autoSendMailAddress;

        public string AutoSendMailAddress
        {
            get { return autoSendMailAddress; }
            set { this.SetValue("AutoSendMailAddress", ref autoSendMailAddress, value); }
        }

        /// <summary>
        /// 审核日期
        /// </summary>
        private DateTime? auditDate;

        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { this.SetValue("AuditDate", ref auditDate, value); }
        }

        /// <summary>
        /// 审核人编号
        /// </summary>
        private int? auditUserSysNo;

        public int? AuditUserSysNo
        {
            get { return auditUserSysNo; }
            set { this.SetValue("AuditUserSysNo", ref auditUserSysNo, value); }
        }

        private string checkResult;

        public string CheckResult
        {
            get { return checkResult; }
            set { this.SetValue("CheckResult", ref checkResult, value); }
        }

        /// <summary>
        /// 备忘录信息
        /// </summary>
        private PurchaseOrderMemoInfoVM memoInfo;

        public PurchaseOrderMemoInfoVM MemoInfo
        {
            get { return memoInfo; }
            set { this.SetValue("MemoInfo", ref memoInfo, value); }
        }

        private int? aRMCount;

        public int? ARMCount
        {
            get { return aRMCount; }
            set { this.SetValue("ARMCount", ref aRMCount, value); }
        }

        private bool? isManagerPM;
        public bool? IsManagerPM
        {
            get { return isManagerPM; }
            set { this.SetValue("IsManagerPM", ref isManagerPM, value); }
        }

        /// <summary>
        ///转租赁
        /// </summary>
        private PurchaseOrderLeaseFlag? purchaseOrderLeaseFlag;

        public PurchaseOrderLeaseFlag? PurchaseOrderLeaseFlag
        {
            get { return purchaseOrderLeaseFlag ==null ? ECCentral.BizEntity.PO.PurchaseOrderLeaseFlag.unLease : purchaseOrderLeaseFlag; }
            set { this.SetValue("PurchaseOrderLeaseFlag", ref purchaseOrderLeaseFlag, value); }
        }

        private string logisticsNumber;

        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.MaxLength, 50)]
        public string LogisticsNumber
        {
            get { return logisticsNumber; }
            set { this.SetValue("LogisticsNumber", ref logisticsNumber, value); }
        }

        private string expressName;

        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.MaxLength, 50)]
        public string ExpressName
        {
            get { return expressName; }
            set { this.SetValue("ExpressName", ref expressName, value); }
        }
    }
}
