using System;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class GiftInvoiceDetaiReportQueryVM : ModelBase
    {
        private DateTime? m_OutDateFrom;
        public DateTime? OutDateFrom
        {
            get
            {
                return this.m_OutDateFrom;
            }
            set
            {
                this.SetValue("OutDateFrom", ref m_OutDateFrom, value);
            }
        }

        private DateTime? m_OutDateTo;
        public DateTime? OutDateTo
        {
            get
            {
                return this.m_OutDateTo;
            }
            set
            {
                this.SetValue("OutDateTo", ref m_OutDateTo, value);
            }
        }

        private DateTime? m_InvoiceDateFrom;
        public DateTime? InvoiceDateFrom
        {
            get
            {
                return this.m_InvoiceDateFrom;
            }
            set
            {
                this.SetValue("InvoiceDateFrom", ref m_InvoiceDateFrom, value);
            }
        }

        private DateTime? m_InvoiceDateTo;
        public DateTime? InvoiceDateTo
        {
            get
            {
                return this.m_InvoiceDateTo;
            }
            set
            {
                this.SetValue("InvoiceDateTo", ref m_InvoiceDateTo, value);
            }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get
            {
                return this.m_StockSysNo;
            }
            set
            {
                this.SetValue("StockSysNo", ref m_StockSysNo, value);
            }
        }

        private String m_OrderID;
        public String OrderID
        {
            get
            {
                return this.m_OrderID;
            }
            set
            {
                this.SetValue("OrderID", ref m_OrderID, value);
            }
        }

        private String m_InvoiceNumber;
        public String InvoiceNumber
        {
            get
            {
                return this.m_InvoiceNumber;
            }
            set
            {
                this.SetValue("InvoiceNumber", ref m_InvoiceNumber, value);
            }
        }

        private String m_CustomerName;
        public String CustomerName
        {
            get
            {
                return this.m_CustomerName;
            }
            set
            {
                this.SetValue("CustomerName", ref m_CustomerName, value);
            }
        }

        private SOType? m_SOType;
        public SOType? SOType
        {
            get
            {
                return this.m_SOType;
            }
            set
            {
                this.SetValue("SOType", ref m_SOType, value);
            }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get
            {
                return this.m_CompanyCode;
            }
            set
            {
                this.SetValue("CompanyCode", ref m_CompanyCode, value);
            }
        }
    }
}