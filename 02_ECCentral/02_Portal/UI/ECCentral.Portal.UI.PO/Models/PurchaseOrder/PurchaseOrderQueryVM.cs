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
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderQueryVM : ModelBase
    {
        public PurchaseOrderQueryVM()
        {
            createTimeBegin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Subtract(new TimeSpan(2, 0, 0, 0, 0));
            createTimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
        }
        private string pOSysNo;

        public string POSysNo
        {
            get { return pOSysNo; }
            set { base.SetValue("POSysNo", ref pOSysNo, value); }
        }
        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }
        private string productSysNo;

        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        private string stockSysNo;

        public string StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }

        private string companySysNo;

        public string CompanySysNo
        {
            get { return companySysNo; }
            set { base.SetValue("CompanySysNo", ref companySysNo, value); }
        }

        private DateTime? createTimeBegin;

        public DateTime? CreateTimeBegin
        {
            get { return createTimeBegin; }
            set { base.SetValue("CreateTimeBegin", ref createTimeBegin, value); }
        }
        private DateTime? createTimeTo;

        public DateTime? CreateTimeTo
        {
            get { return createTimeTo; }
            set { base.SetValue("CreateTimeTo", ref createTimeTo, value); }
        }
        private DateTime? inStockFrom;

        public DateTime? InStockFrom
        {
            get { return inStockFrom; }
            set { base.SetValue("InStockFrom", ref inStockFrom, value); }
        }
        private DateTime? inStockTo;

        public DateTime? InStockTo
        {
            get { return inStockTo; }
            set { base.SetValue("InStockTo", ref inStockTo, value); }
        }
        private string currencySysNo;

        public string CurrencySysNo
        {
            get { return currencySysNo; }
            set { base.SetValue("CurrencySysNo", ref currencySysNo, value); }
        }
        private string isApportion;

        public string IsApportion
        {
            get { return isApportion; }
            set { base.SetValue("IsApportion", ref isApportion, value); }
        }
        private PurchaseOrderConsignFlag? isConsign;

        public PurchaseOrderConsignFlag? IsConsign
        {
            get { return isConsign; }
            set { base.SetValue("IsConsign", ref isConsign, value); }
        }
        private PurchaseOrderStatus? status;
        public PurchaseOrderStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// 多种状态
        /// </summary>
        private string statusList;
        public string StatusList
        {
            get { return statusList; }
            set { base.SetValue("StatusList", ref statusList, value); }
        }

        private string isStockStatus;

        public string IsStockStatus
        {
            get { return isStockStatus; }
            set { base.SetValue("IsStockStatus", ref isStockStatus, value); }
        }
        private DateTime? printTime;

        public DateTime? PrintTime
        {
            get { return printTime; }
            set { base.SetValue("PrintTime", ref printTime, value); }
        }
        private PurchaseOrderVerifyStatus? verifyStatus;

        public PurchaseOrderVerifyStatus? VerifyStatus
        {
            get { return verifyStatus; }
            set { base.SetValue("VerifyStatus", ref verifyStatus, value); }
        }
        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }
        private string vendorSysNo;

        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        private string pMSysNo;

        public string PMSysNo
        {
            get { return pMSysNo; }
            set { base.SetValue("PMSysNo", ref pMSysNo, value); }
        }
        private string createPOSysNo;

        public string CreatePOSysNo
        {
            get { return createPOSysNo; }
            set { base.SetValue("CreatePOSysNo", ref createPOSysNo, value); }
        }
        private string pOType;

        public string POType
        {
            get { return pOType; }
            set { base.SetValue("POType", ref pOType, value); }
        }
        private bool? isTransfer;

        public bool? IsTransfer
        {
            get { return isTransfer; }
            set { base.SetValue("IsTransfer", ref isTransfer, value); }
        }
        private int? tranferStock;

        public int? TranferStock
        {
            get { return tranferStock; }
            set { base.SetValue("TranferStock", ref tranferStock, value); }
        }
        private string pOSysNoExtention;

        public string POSysNoExtention
        {
            get { return pOSysNoExtention; }
            set { base.SetValue("POSysNoExtention", ref pOSysNoExtention, value); }
        }

        private string auditUser;

        public string AuditUser
        {
            get { return auditUser; }
            set { base.SetValue("AuditUser", ref auditUser, value); }
        }

        /// <summary>
        /// 是否为在途查询
        /// </summary>
        private bool? isPurchaseQtySearch;

        public bool? IsPurchaseQtySearch
        {
            get { return isPurchaseQtySearch; }
            set { base.SetValue("IsPurchaseQtySearch", ref isPurchaseQtySearch, value); }
        }

        /// <summary>
        /// 获取或设置在途PO数据状态[,分割]
        /// </summary>
        private string queryStatus;

        public string QueryStatus
        {
            get { return queryStatus; }
            set { base.SetValue("QueryStatus", ref queryStatus, value); }
        }
        /// <summary>
        /// 获取或设置在途查询仓库[,分割]
        /// </summary>
        private string queryStock;

        public string QueryStock
        {
            get { return queryStock; }
            set { base.SetValue("QueryStock", ref queryStock, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private PaySettleCompany? paySettleCompany;
        public PaySettleCompany? PaySettleCompany 
        {
            get { return paySettleCompany; }
            set { base.SetValue("PaySettleCompany", ref paySettleCompany, value); }
        }

        private DateTime? eTATimeFrom;

        public DateTime? ETATimeFrom
        {
            get { return eTATimeFrom; }
            set { base.SetValue("ETATimeFrom", ref eTATimeFrom, value); }
        }

        private DateTime? eTATimeTo;

        public DateTime? ETATimeTo
        {
            get { return eTATimeTo; }
            set { base.SetValue("ETATimeTo", ref eTATimeTo, value); }
        }

        private string fromTotalAmount;
        /// <summary>
        /// 开始订单总额
        /// </summary>
        [Validate(ValidateType.Regex, @"^\d{1,9}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ResPurchaseOrderQuery), ErrorMessageResourceName = "Msg_TotalAmount_Format")]
        public string FromTotalAmount
        {
            get { return fromTotalAmount; }
            set { SetValue("FromTotalAmount", ref fromTotalAmount, value); }
        }
        private string toTotalAmount;
        /// <summary>
        /// 开始订单总额
        /// </summary>
        [Validate(ValidateType.Regex, @"^\d{1,9}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ResPurchaseOrderQuery), ErrorMessageResourceName = "Msg_TotalAmount_Format")]
        public string ToTotalAmount
        {
            get
            {
                return toTotalAmount;
            }
            set
            {
                SetValue("ToTotalAmount", ref toTotalAmount, value);
            }
        }

        /// <summary>
        /// 物流单号
        /// </summary>
        private string logisticsNumber;

        public string LogisticsNumber
        {
            get { return logisticsNumber; }
            set { base.SetValue("LogisticsNumber", ref logisticsNumber, value); }
        }

        /// <summary>
        /// 快递公司名称
        /// </summary>
        private string expressName;

        public string ExpressName
        {
            get { return expressName; }
            set { base.SetValue("ExpressName", ref expressName, value); }
        }
    }
}
