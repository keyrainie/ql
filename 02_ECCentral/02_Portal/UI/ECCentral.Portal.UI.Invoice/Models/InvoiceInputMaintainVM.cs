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
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using System.Linq;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class InvoiceInputMaintainVM : ModelBase
    {
        public InvoiceInputMaintainVM()
        {
            this.POItemList = new List<APInvoicePOItemVM>();
            this.InvoiceItemList = new List<APInvoiceInvoiceItemVM>();
            this.DiffTypeList = EnumConverter.GetKeyValuePairs<InvoiceDiffType>(EnumConverter.EnumAppendItemType.None);
            this.OrderTypeList = EnumConverter.GetKeyValuePairs<PayableOrderType>();          
            this.OrderTypeList.RemoveAll(p => (int)p.Key>10);
            this.OrderType = PayableOrderType.PO;
        }
        #region APInvoiceMaster信息
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_VendorPortalSysNo;
        public Int32? VendorPortalSysNo
        {
            get { return this.m_VendorPortalSysNo; }
            set { this.SetValue("VendorPortalSysNo", ref m_VendorPortalSysNo, value); }
        }

        private DateTime? m_DocDate;
        public DateTime? DocDate
        {
            get { return this.m_DocDate; }
            set { this.SetValue("DocDate", ref m_DocDate, value); }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private Int32? m_VendorSysNo;
        public Int32? VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        private String m_VendorName;
        public String VendorName
        {
            get { return this.m_VendorName; }
            set { this.SetValue("VendorName", ref m_VendorName, value); }
        }

        private Decimal? m_VendorTaxRate;
        public Decimal? VendorTaxRate
        {
            get
            {
                return ConstValue.Invoice_TaxRateBase - 1;
            }
            set { this.SetValue("VendorTaxRate", ref m_VendorTaxRate, value); }
        }

        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        private String m_DiffMemo;
        public String DiffMemo
        {
            get { return this.m_DiffMemo; }
            set { this.SetValue("DiffMemo", ref m_DiffMemo, value); }
        }

        private Decimal? m_PoNetAmt;
        public Decimal? PoNetAmt
        {
            get { return this.m_PoNetAmt; }
            set { this.SetValue("PoNetAmt", ref m_PoNetAmt, value); }
        }

        private Decimal? m_PoNetTaxAmt;
        public Decimal? PoNetTaxAmt
        {
            get { return this.m_PoNetTaxAmt; }
            set { this.SetValue("PoNetTaxAmt", ref m_PoNetTaxAmt, value); }
        }

        private Decimal? m_InvoiceAmt;
        public Decimal? InvoiceAmt
        {
            get { return this.m_InvoiceAmt; }
            set { this.SetValue("InvoiceAmt", ref m_InvoiceAmt, value); }
        }

        private Decimal? m_InvoiceTaxAmt;
        public Decimal? InvoiceTaxAmt
        {
            get { return this.m_InvoiceTaxAmt; }
            set { this.SetValue("InvoiceTaxAmt", ref m_InvoiceTaxAmt, value); }
        }

        private Decimal? m_DiffTaxAmt;
        public Decimal? DiffTaxAmt
        {
            get { return this.m_DiffTaxAmt; }
            set { this.SetValue("DiffTaxAmt", ref m_DiffTaxAmt, value); }
        }

        private InvoiceDiffType? m_DiffTaxTreatmentType;
        public InvoiceDiffType? DiffTaxTreatmentType
        {
            get { return this.m_DiffTaxTreatmentType; }
            set { this.SetValue("DiffTaxTreatmentType", ref m_DiffTaxTreatmentType, value); }
        }

        private APInvoiceMasterStatus? m_Status;
        public APInvoiceMasterStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private String m_ConfirmUser;
        public String ConfirmUser
        {
            get { return this.m_ConfirmUser; }
            set { this.SetValue("ConfirmUser", ref m_ConfirmUser, value); }
        }

        private DateTime? m_ConfirmDate;
        public DateTime? ConfirmDate
        {
            get { return this.m_ConfirmDate; }
            set { this.SetValue("ConfirmDate", ref m_ConfirmDate, value); }
        }

        private Int32? m_UpdateInvoiceUserSysNo;
        public Int32? UpdateInvoiceUserSysNo
        {
            get { return this.m_UpdateInvoiceUserSysNo; }
            set { this.SetValue("UpdateInvoiceUserSysNo", ref m_UpdateInvoiceUserSysNo, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        public string VendorTaxRateString
        {
            get
            {
                return string.Format("{0}%", decimal.Round(VendorTaxRate.Value * 100, 2));
            }
        }
        #endregion
        private List<APInvoicePOItemVM> poItemList;
        public List<APInvoicePOItemVM> POItemList
        {
            get { return this.poItemList; }
            set { this.SetValue("POItemList", ref poItemList, value); }
        }

        private List<APInvoiceInvoiceItemVM> invoiceItemList;
        public List<APInvoiceInvoiceItemVM> InvoiceItemList
        {
            get { return this.invoiceItemList; }
            set { this.SetValue("InvoiceItemList", ref invoiceItemList, value); }
        }
        #region 界面上用于绑定录入POItem和InvoiceItem的字段

        /// <summary>
        /// item编号（三种方式：单一、以'.'分隔，以'-'分隔）
        /// </summary>
        private string poItemNoList;
        public string POItemNoList
        {
            get { return this.poItemNoList; }
            set { this.SetValue("POItemNoList", ref poItemNoList, value); }
        }

        private string invoiceItemNoList;
        public string InvoiceItemNoList
        {
            get { return this.invoiceItemNoList; }
            set { this.SetValue("InvoiceItemNoList", ref invoiceItemNoList, value); }
        }
        /// <summary>
        /// 发票日期
        /// </summary>
        private DateTime? invoiceDate;
        public DateTime? InvoiceDate
        {
            get { return this.invoiceDate; }
            set { this.SetValue("InvoiceDate", ref invoiceDate, value); }
        }
        /// <summary>
        /// 发票金额
        /// </summary>
        private decimal? invoiceAmt;
        public decimal? ItemInvoiceAmt
        {
            get { return this.invoiceAmt; }
            set { this.SetValue("ItemInvoiceAmt", ref invoiceAmt, value); }
        }
        /// <summary>
        /// PO列表
        /// </summary>
        private PayableOrderType? orderType;
        public PayableOrderType? OrderType
        {
            get { return this.orderType; }
            set { this.SetValue("OrderType", ref orderType, value); }
        }
        /// <summary>
        /// PO单起始日期
        /// </summary>
        private DateTime? poDateFrom;
        public DateTime? PODateFrom
        {
            get { return this.poDateFrom; }
            set { this.SetValue("PODateFrom", ref poDateFrom, value); }
        }
        #endregion

        public List<KeyValuePair<InvoiceDiffType?, string>> DiffTypeList
        {
            get;
            set;
        }
        public List<KeyValuePair<PayableOrderType?, string>> OrderTypeList
        {
            get;
            set;
        }

        private bool isDiffAmtRight;
        public bool IsDiffAmtRight
        {
            get { return this.isDiffAmtRight; }
            set { this.SetValue("IsDiffAmtRight", ref isDiffAmtRight, value); }
        }

        private bool isDataRight;
        public bool IsDataRight
        {
            get { return this.isDataRight; }
            set { this.SetValue("IsDataRight", ref isDataRight, value); }
        }

    }


    public class APInvoicePOItemVM : ModelBase
    {
        public APInvoicePOItemVM()
        {
            this.OrderTypeList = EnumConverter.GetKeyValuePairs<PayableOrderType>(EnumConverter.EnumAppendItemType.All);
        }
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }
        private int? poNo;
        public int? PoNo
        {
            get { return poNo; }
            set
            {
                base.SetValue("PoNo", ref poNo, value);
            }
        }
        private int? poStockSysNo;
        public int? PoStockSysNo
        {
            get { return poStockSysNo; }
            set
            {
                base.SetValue("PoStockSysNo", ref poStockSysNo, value);
            }
        }
        private int? poCurrency;
        public int? PoCurrency
        {
            get { return poCurrency; }
            set
            {
                base.SetValue("PoCurrency", ref poCurrency, value);
            }
        }
        private decimal? poAmt;
        public decimal? PoAmt
        {
            get { return poAmt; }
            set
            {
                base.SetValue("PoAmt", ref poAmt, value);
            }
        }
        private int? eimsNo;
        public int? EIMSNo
        {
            get { return eimsNo; }
            set
            {
                base.SetValue("EIMSNo", ref eimsNo, value);
            }
        }
        private string eimsNoList;
        public string EIMSNoList
        {
            get { return eimsNoList; }
            set
            {
                base.SetValue("EIMSNoList", ref eimsNoList, value);
            }
        }
        private decimal? eimsAmt;
        public decimal? EIMSAmt
        {
            get { return eimsAmt; }
            set
            {
                base.SetValue("EIMSAmt", ref eimsAmt, value);
            }
        }
        private decimal? eimsNetAmt;
        public decimal? EIMSNetAmt
        {
            get { return eimsNetAmt; }
            set
            {
                base.SetValue("EIMSNetAmt", ref eimsNetAmt, value);
            }
        }
        private decimal? eimsNetTaxAmt;
        public decimal? EIMSNetTaxAmt
        {
            get { return eimsNetTaxAmt; }
            set
            {
                base.SetValue("EIMSNetTaxAmt", ref eimsNetTaxAmt, value);
            }
        }
        private decimal? poNetAmt;
        public decimal? PoNetAmt
        {
            get { return poNetAmt; }
            set
            {
                base.SetValue("PoNetAmt", ref poNetAmt, value);
            }
        }
        private decimal? paymentAmt;
        public decimal? PaymentAmt
        {
            get { return paymentAmt; }
            set
            {
                base.SetValue("PaymentAmt", ref paymentAmt, value);
            }
        }
        private DateTime? poBaselineDate;
        public DateTime? PoBaselineDate
        {
            get { return poBaselineDate; }
            set
            {
                base.SetValue("PoBaselineDate", ref poBaselineDate, value);
            }
        }
        private int? poPaymentTerm;
        public int? PoPaymentTerm
        {
            get { return poPaymentTerm; }
            set
            {
                base.SetValue("PoPaymentTerm", ref poPaymentTerm, value);
            }
        }
        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }
        private PayableOrderType? orderType;
        public PayableOrderType? OrderType
        {
            get { return orderType; }
            set
            {
                base.SetValue("OrderType", ref orderType, value);
            }
        }
        private int? batchNumber;
        public int? BatchNumber
        {
            get { return batchNumber; }
            set
            {
                base.SetValue("BatchNumber", ref batchNumber, value);
            }
        }
        private decimal? payableTaxAmt;
        public decimal? PayableTaxAmt
        {
            get { return payableTaxAmt; }
            set
            {
                base.SetValue("PayableTaxAmt", ref payableTaxAmt, value);
            }
        }
        private DateTime? etp;
        public DateTime? ETP
        {
            get { return etp; }
            set
            {
                base.SetValue("ETP", ref etp, value);
            }
        }
        public string PoNoStr
        {
            get
            {
                if (BatchNumber.HasValue)
                {
                    return PoNo.Value.ToString() + "-" + BatchNumber.Value.ToString().PadLeft(2, '0');
                }
                return PoNo.Value.ToString();
            }
        }
        public List<KeyValuePair<PayableOrderType?, string>> OrderTypeList
        {
            get;
            set;
        }

    }
    public class APInvoiceInvoiceItemVM : ModelBase
    {
        public APInvoiceInvoiceItemVM()
        {
        }
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }
        private int? poNo;
        public int? PoNo
        {
            get { return poNo; }
            set
            {
                base.SetValue("PoNo", ref poNo, value);
            }
        }
        private string invoiceNo;
        public string InvoiceNo
        {
            get { return invoiceNo; }
            set
            {
                base.SetValue("InvoiceNo", ref invoiceNo, value);
            }
        }
        private DateTime? invoiceDate;
        public DateTime? InvoiceDate
        {
            get { return invoiceDate; }
            set
            {
                base.SetValue("InvoiceDate", ref invoiceDate, value);
            }
        }
        private int? invoiceCurrency;
        public int? InvoiceCurrency
        {
            get { return invoiceCurrency; }
            set
            {
                base.SetValue("InvoiceCurrency", ref invoiceCurrency, value);
            }
        }
        private decimal? invoiceAmt;
        public decimal? InvoiceAmt
        {
            get { return invoiceAmt; }
            set
            {
                base.SetValue("InvoiceAmt", ref invoiceAmt, value);
            }
        }
        private decimal? invoiceNetAmt;
        public decimal? InvoiceNetAmt
        {
            get { return invoiceNetAmt; }
            set
            {
                base.SetValue("InvoiceNetAmt", ref invoiceNetAmt, value);
            }
        }
        private decimal? invoiceTaxAmt;
        public decimal? InvoiceTaxAmt
        {
            get { return invoiceTaxAmt; }
            set
            {
                base.SetValue("InvoiceTaxAmt", ref invoiceTaxAmt, value);
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

    }
}
