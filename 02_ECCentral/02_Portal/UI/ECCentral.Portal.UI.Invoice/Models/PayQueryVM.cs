using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Invoice;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Linq;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 应付款查询条件
    /// </summary>
    public class PayableQueryVM : ModelBase
    {
        public PayableQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.OrderTypeList = EnumConverter.GetKeyValuePairs<PayableOrderType>(EnumConverter.EnumAppendItemType.All);
            this.PayStatusList = EnumConverter.GetKeyValuePairs<PayableStatus>(EnumConverter.EnumAppendItemType.All);
            this.InvoiceStatusList = EnumConverter.GetKeyValuePairs<InvoiceStatus>(EnumConverter.EnumAppendItemType.All);
            this.PayItemStyleList = EnumConverter.GetKeyValuePairs<PayItemStyle>(EnumConverter.EnumAppendItemType.All);
            this.POStatusList = EnumConverter.GetKeyValuePairs<POStatus>(EnumConverter.EnumAppendItemType.All);
            this.POTypeList = EnumConverter.GetKeyValuePairs<POType>(EnumConverter.EnumAppendItemType.All);
            this.PayPeriodTypeList = new List<CodeNamePair>();
            this.VendorSettleOrderStatusList = EnumConverter.GetKeyValuePairs<VendorSettleOrderStatus>(EnumConverter.EnumAppendItemType.All);
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.IsOnlyNegativeOrder = this.IsNotInStock = false;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        private string orderID;
        [Validate(ValidateType.Interger)]
        public string OrderID
        {
            get { return orderID; }
            set { base.SetValue("OrderID", ref orderID, value); }
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        private PayableOrderType? orderType;
        public PayableOrderType? OrderType
        {
            get { return orderType; }
            set { base.SetValue("OrderType", ref orderType, value); }
        }

        /// <summary>
        /// 付款状态
        /// </summary>
        private PayableStatus? payStatus;
        public PayableStatus? PayStatus
        {
            get { return payStatus; }
            set { base.SetValue("PayStatus", ref payStatus, value); }
        }

        /// <summary>
        /// 发票状态
        /// </summary>
        private InvoiceStatus? invoiceStatus;
        public InvoiceStatus? InvoiceStatus
        {
            get { return invoiceStatus; }
            set { base.SetValue("InvoiceStatus", ref invoiceStatus, value); }
        }

        /// <summary>
        /// 应付时间
        /// </summary>
        private DateTime? orderDateFrom;
        public DateTime? OrderDateFrom
        {
            get { return orderDateFrom; }
            set { base.SetValue("OrderDateFrom", ref orderDateFrom, value); }
        }
        private DateTime? orderDateTo;
        public DateTime? OrderDateTo
        {
            get { return orderDateTo; }
            set { base.SetValue("OrderDateTo", ref orderDateTo, value); }
        }

        /// <summary>
        /// 预计付款时间
        /// </summary>
        private DateTime? poETPDateFrom;
        public DateTime? POETPDateFrom
        {
            get { return poETPDateFrom; }
            set { base.SetValue("POETPDateFrom", ref poETPDateFrom, value); }
        }
        private DateTime? poETPDateTo;
        public DateTime? POETPDateTo
        {
            get { return poETPDateTo; }
            set { base.SetValue("POETPDateTo", ref poETPDateTo, value); }
        }

        /// <summary>
        /// 预估付款时间
        /// </summary>
        private DateTime? poEGPDateFrom;
        public DateTime? POEGPDateFrom
        {
            get { return poEGPDateFrom; }
            set { base.SetValue("POEGPDateFrom", ref poEGPDateFrom, value); }
        }
        private DateTime? poEGPDateTo;
        public DateTime? POEGPDateTo
        {
            get { return poEGPDateTo; }
            set { base.SetValue("POEGPDateTo", ref poEGPDateTo, value); }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDate", ref inDateFrom, value); }
        }
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 是否为仅负单据
        /// </summary>
        private bool? isOnlyNegativeOrder;
        public bool? IsOnlyNegativeOrder
        {
            get { return isOnlyNegativeOrder; }
            set { base.SetValue("IsOnlyNegativeOrder", ref isOnlyNegativeOrder, value); }
        }

        /// <summary>
        /// 是否未入库
        /// </summary>
        private bool? isNotInStock;
        public bool? IsNotInStock
        {
            get { return isNotInStock; }
            set { base.SetValue("IsNotInStock", ref isNotInStock, value); }
        }

        /// <summary>
        /// Po归附PM
        /// </summary>
        private int? poBelongPMSysNo;
        private int? POBelongPMSysNo
        {
            get { return poBelongPMSysNo; }
            set { base.SetValue("POBelongPMSysNo", ref poBelongPMSysNo, value); }
        }
        /// <summary>
        /// 付款类型
        /// </summary>
        private PayItemStyle? payStyle;
        public PayItemStyle? PayStyle
        {
            get { return payStyle; }
            set { base.SetValue("PayStyle", ref payStyle, value); }
        }

        /// <summary>
        /// PO状态
        /// </summary>
        private POStatus? poStatus;
        public POStatus? POStatus
        {
            get { return poStatus; }
            set { base.SetValue("POStatus", ref poStatus, value); }
        }

        /// <summary>
        /// PO类型
        /// </summary>
        private POType? poType;
        public POType? POType
        {
            get { return poType; }
            set { base.SetValue("POType", ref poType, value); }
        }

        /// <summary>
        /// 账期类型
        /// </summary>
        private int? payPeriodType;
        public int? PayPeriodType
        {
            get { return payPeriodType; }
            set { base.SetValue("PayPeriodType", ref payPeriodType, value); }
        }

        /// <summary>
        /// 供应商结算单状态
        /// </summary>
        private VendorSettleOrderStatus? vendorSettleStatus;
        public VendorSettleOrderStatus? VendorSettleStatus
        {
            get { return vendorSettleStatus; }
            set { base.SetValue("VendorSettleStatus", ref vendorSettleStatus, value); }
        }
        /// <summary>
        /// 财务月结单状态
        /// </summary>
        private bool? financeSettleOrderStatus;
        public bool? FinanceSettleOrderStatus
        {
            get { return financeSettleOrderStatus; }
            set { base.SetValue("FinanceSettleOrderStatus", ref financeSettleOrderStatus, value); }
        }

        /// <summary>
        /// 调整单状态
        /// </summary>
        private bool? balanceOrderStatus;
        public bool? BalanceOrderStatus
        {
            get { return balanceOrderStatus; }
            set { base.SetValue("BalanceOrderStatus", ref balanceOrderStatus, value); }
        }

        /// <summary>
        /// 供应商
        /// </summary>
        private int? vendorSysNo;
        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        /// <summary>
        /// 货币类型
        /// </summary>
        private int? currencySysNo;
        public int? CurrencySysNo
        {
            get { return currencySysNo; }
            set { base.SetValue("CurrencySysNo", ref currencySysNo, value); }
        }

        /// <summary>
        ///PO创建人（产品管理员）
        /// </summary>
        private int? createPMSysNo;
        public int? CreatePMSysNo
        {
            get { return createPMSysNo; }
            set { base.SetValue("CreatePMSysNo", ref createPMSysNo, value); }
        }

        /// <summary>
        /// 仓库
        /// </summary>
        private string stockSysNo;
        public string StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }


        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue("CompanyCode", ref companyCode, value); }
        }

        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set { SetValue("ChannelID", ref channelID, value); }
        }

        public List<UIWebChannel> WebChannelList { get; set; }
        public List<KeyValuePair<PayableOrderType?, string>> OrderTypeList { get; set; }
        public List<KeyValuePair<PayableStatus?, string>> PayStatusList { get; set; }
        public List<KeyValuePair<InvoiceStatus?, string>> InvoiceStatusList { get; set; }
        public List<KeyValuePair<PayItemStyle?, string>> PayItemStyleList { get; set; }
        public List<KeyValuePair<POStatus?, string>> POStatusList { get; set; }
        public List<KeyValuePair<POType?, string>> POTypeList { get; set; }
        public List<CodeNamePair> PayPeriodTypeList { get; set; }
        public List<KeyValuePair<VendorSettleOrderStatus?, string>> VendorSettleOrderStatusList { get; set; }
        public List<KeyValuePair<Boolean?, string>> YNList { get; set; }
    }
}
