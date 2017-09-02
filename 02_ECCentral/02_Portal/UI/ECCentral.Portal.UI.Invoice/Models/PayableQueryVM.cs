using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 应付款查询条件
    /// </summary>
    public class PayableQueryVM : ModelBase
    {
        public PayableQueryVM()
        {

            this.WebChannelList = new List<UIWebChannel>(CPApplication.Current.CurrentWebChannelList);
            this.WebChannelList.Insert(0, new UIWebChannel
            {
                ChannelName = ResCommonEnum.Enum_All,
                ChannelType = UIWebChannelType.Sales
            });
            this.OrderTypeList = EnumConverter.GetKeyValuePairs<PayableOrderType>(); 

            this.PayStatusList = EnumConverter.GetKeyValuePairs<PayableStatus>(EnumConverter.EnumAppendItemType.All);
            this.InvoiceStatusList = EnumConverter.GetKeyValuePairs<PayableInvoiceStatus>(EnumConverter.EnumAppendItemType.All);
            this.PayItemStyleList = EnumConverter.GetKeyValuePairs<PayItemStyle>(EnumConverter.EnumAppendItemType.All);

            this.POStatusList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PurchaseOrderStatus>(EnumConverter.EnumAppendItemType.All);
            this.POStatusList.RemoveAll(x => x.Key == ECCentral.BizEntity.PO.PurchaseOrderStatus.Origin);

            this.POTypeList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PurchaseOrderType>(EnumConverter.EnumAppendItemType.All);
            this.PayPeriodTypeList = new List<CodeNamePair>();
            this.PaySettleCompanyList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PaySettleCompany>(EnumConverter.EnumAppendItemType.All);
            this.VendorSettleOrderStatusList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.SettleStatus>(EnumConverter.EnumAppendItemType.All);
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.IsOnlyNegativeOrder = this.IsNotInStock = false;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        private string orderID;
        [Validate(ValidateType.Regex, @"^\s*[1-9][0-9]{0,8}(\s*\.\s*[1-9][0-9]{0,8}\s*)*$", ErrorMessageResourceName = "Msg_ValidateNoList", ErrorMessageResourceType = typeof(ResPayQuery))]
        public string OrderID
        {
            get
            {
                return orderID;
            }
            set
            {
                base.SetValue("OrderID", ref orderID, value);
            }
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        private PayableOrderType? orderType;
        public PayableOrderType? OrderType
        {
            get
            {
                return orderType;
            }
            set
            {
                base.SetValue("OrderType", ref orderType, value);
            }
        }

        /// <summary>
        /// 付款状态
        /// </summary>
        private PayableStatus? payStatus;
        public PayableStatus? PayStatus
        {
            get
            {
                return payStatus;
            }
            set
            {
                base.SetValue("PayStatus", ref payStatus, value);
            }
        }

        /// <summary>
        /// 发票状态
        /// </summary>
        private PayableInvoiceStatus? invoiceStatus;
        public PayableInvoiceStatus? InvoiceStatus
        {
            get
            {
                return invoiceStatus;
            }
            set
            {
                base.SetValue("InvoiceStatus", ref invoiceStatus, value);
            }
        }

        /// <summary>
        /// 应付时间
        /// </summary>
        private DateTime? orderDateFrom;
        public DateTime? OrderDateFrom
        {
            get
            {
                return orderDateFrom;
            }
            set
            {
                base.SetValue("OrderDateFrom", ref orderDateFrom, value);
            }
        }
        private DateTime? orderDateTo;
        public DateTime? OrderDateTo
        {
            get
            {
                return orderDateTo;
            }
            set
            {
                base.SetValue("OrderDateTo", ref orderDateTo, value);
            }
        }

        /// <summary>
        /// 预计付款时间
        /// </summary>
        private DateTime? poETPDateFrom;
        public DateTime? POETPDateFrom
        {
            get
            {
                return poETPDateFrom;
            }
            set
            {
                base.SetValue("POETPDateFrom", ref poETPDateFrom, value);
            }
        }
        private DateTime? poETPDateTo;
        public DateTime? POETPDateTo
        {
            get
            {
                return poETPDateTo;
            }
            set
            {
                base.SetValue("POETPDateTo", ref poETPDateTo, value);
            }
        }

        /// <summary>
        /// 预估付款时间
        /// </summary>
        private DateTime? poEGPDateFrom;
        public DateTime? POEGPDateFrom
        {
            get
            {
                return poEGPDateFrom;
            }
            set
            {
                base.SetValue("POEGPDateFrom", ref poEGPDateFrom, value);
            }
        }
        private DateTime? poEGPDateTo;
        public DateTime? POEGPDateTo
        {
            get
            {
                return poEGPDateTo;
            }
            set
            {
                base.SetValue("POEGPDateTo", ref poEGPDateTo, value);
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get
            {
                return inDateFrom;
            }
            set
            {
                base.SetValue("InDateFrom", ref inDateFrom, value);
            }
        }
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get
            {
                return inDateTo;
            }
            set
            {
                base.SetValue("InDateTo", ref inDateTo, value);
            }
        }

        /// <summary>
        /// 是否为仅负单据
        /// </summary>
        private bool? isOnlyNegativeOrder;
        public bool? IsOnlyNegativeOrder
        {
            get
            {
                return isOnlyNegativeOrder;
            }
            set
            {
                base.SetValue("IsOnlyNegativeOrder", ref isOnlyNegativeOrder, value);
            }
        }

        /// <summary>
        /// 是否未入库
        /// </summary>
        private bool? isNotInStock;
        public bool? IsNotInStock
        {
            get
            {
                return isNotInStock;
            }
            set
            {
                base.SetValue("IsNotInStock", ref isNotInStock, value);
            }
        }

        /// <summary>
        /// Po归附PM
        /// </summary>
        private int? poBelongPMSysNo;
        public int? POBelongPMSysNo
        {
            get
            {
                return poBelongPMSysNo;
            }
            set
            {
                base.SetValue("POBelongPMSysNo", ref poBelongPMSysNo, value);
            }
        }
        /// <summary>
        /// 付款类型
        /// </summary>
        private PayItemStyle? payStyle;
        public PayItemStyle? PayStyle
        {
            get
            {
                return payStyle;
            }
            set
            {
                base.SetValue("PayStyle", ref payStyle, value);
            }
        }

        /// <summary>
        /// PO状态
        /// </summary>
        private ECCentral.BizEntity.PO.PurchaseOrderStatus? poStatus;
        public ECCentral.BizEntity.PO.PurchaseOrderStatus? POStatus
        {
            get
            {
                return poStatus;
            }
            set
            {
                base.SetValue("POStatus", ref poStatus, value);
            }
        }

        /// <summary>
        /// PO类型
        /// </summary>
        private ECCentral.BizEntity.PO.PurchaseOrderType? poType;
        public ECCentral.BizEntity.PO.PurchaseOrderType? POType
        {
            get
            {
                return poType;
            }
            set
            {
                base.SetValue("POType", ref poType, value);
            }
        }

        /// <summary>
        /// 账期类型
        /// </summary>
        private int? payPeriodType;
        public int? PayPeriodType
        {
            get
            {
                return payPeriodType;
            }
            set
            {
                base.SetValue("PayPeriodType", ref payPeriodType, value);
            }
        }

        /// <summary>
        /// 商家结算单状态
        /// </summary>
        private ECCentral.BizEntity.PO.SettleStatus? vendorSettleStatus;
        public ECCentral.BizEntity.PO.SettleStatus? VendorSettleStatus
        {
            get
            {
                return vendorSettleStatus;
            }
            set
            {
                base.SetValue("VendorSettleStatus", ref vendorSettleStatus, value);
            }
        }
        /// <summary>
        /// 财务月结单状态
        /// </summary>
        private bool? financeSettleOrderStatus;
        public bool? FinanceSettleOrderStatus
        {
            get
            {
                return financeSettleOrderStatus;
            }
            set
            {
                base.SetValue("FinanceSettleOrderStatus", ref financeSettleOrderStatus, value);
            }
        }

        /// <summary>
        /// 调整单状态
        /// </summary>
        private bool? balanceOrderStatus;
        public bool? BalanceOrderStatus
        {
            get
            {
                return balanceOrderStatus;
            }
            set
            {
                base.SetValue("BalanceOrderStatus", ref balanceOrderStatus, value);
            }
        }

        /// <summary>
        /// 商家
        /// </summary>
        private int? vendorSysNo;
        public int? VendorSysNo
        {
            get
            {
                return vendorSysNo;
            }
            set
            {
                base.SetValue("VendorSysNo", ref vendorSysNo, value);
            }
        }
        /// <summary>
        /// 货币类型
        /// </summary>
        private int? currencySysNo;
        public int? CurrencySysNo
        {
            get
            {
                return currencySysNo;
            }
            set
            {
                base.SetValue("CurrencySysNo", ref currencySysNo, value);
            }
        }

        /// <summary>
        ///PO创建人（产品管理员）
        /// </summary>
        private int? createPMSysNo;
        public int? CreatePMSysNo
        {
            get
            {
                return createPMSysNo;
            }
            set
            {
                base.SetValue("CreatePMSysNo", ref createPMSysNo, value);
            }
        }

        /// <summary>
        /// 仓库
        /// </summary>
        private string stockSysNo;
        public string StockSysNo
        {
            get
            {
                return stockSysNo;
            }
            set
            {
                base.SetValue("StockSysNo", ref stockSysNo, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                SetValue("CompanyCode", ref companyCode, value);
            }
        }

        private string channelID;
        public string ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                SetValue("ChannelID", ref channelID, value);
            }
        }

        private ECCentral.BizEntity.PO.PaySettleCompany? paySettleCompany;
        public ECCentral.BizEntity.PO.PaySettleCompany? PaySettleCompany
        {
            get
            {
                return paySettleCompany;
            }
            set
            {
                SetValue("PaySettleCompany", ref paySettleCompany, value);
            }
        }

        //入库起始时间
        private DateTime? inStockDateFrom;
        public DateTime? InStockDateFrom
        {
            get {
                return inStockDateFrom;
            }
            set {
                SetValue("InStockDateFrom", ref inStockDateFrom, value);
            }
        }

        //入库结束时间
        private DateTime? inStockDateTo;
        public DateTime? InStockDateTo
        {
            get
            {
                return inStockDateTo;
            }
            set {
                SetValue("InStockDateTo", ref inStockDateTo, value);
            }
        }

        public List<UIWebChannel> WebChannelList
        {
            get;
            set;
        }
        public List<KeyValuePair<PayableOrderType?, string>> OrderTypeList
        {
            get;
            set;
        }
        public List<KeyValuePair<PayableStatus?, string>> PayStatusList
        {
            get;
            set;
        }
        public List<KeyValuePair<PayableInvoiceStatus?, string>> InvoiceStatusList
        {
            get;
            set;
        }
        public List<KeyValuePair<PayItemStyle?, string>> PayItemStyleList
        {
            get;
            set;
        }
        public List<KeyValuePair<ECCentral.BizEntity.PO.PurchaseOrderStatus?, string>> POStatusList
        {
            get;
            set;
        }
        public List<KeyValuePair<ECCentral.BizEntity.PO.PurchaseOrderType?, string>> POTypeList
        {
            get;
            set;
        }
        public List<CodeNamePair> PayPeriodTypeList
        {
            get;
            set;
        }
        public List<KeyValuePair<ECCentral.BizEntity.PO.PaySettleCompany?, string>> PaySettleCompanyList
        {
            get;
            set;
        }
        public List<KeyValuePair<ECCentral.BizEntity.PO.SettleStatus?, string>> VendorSettleOrderStatusList
        {
            get;
            set;
        }
        public List<KeyValuePair<Boolean?, string>> YNList
        {
            get;
            set;
        }
    }
}