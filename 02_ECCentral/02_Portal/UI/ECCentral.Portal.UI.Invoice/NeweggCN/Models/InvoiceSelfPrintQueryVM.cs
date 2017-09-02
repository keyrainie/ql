using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.QueryFilter.Invoice
{
    public class InvoiceSelfPrintQueryVM : ModelBase
    {
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

        private int? m_StockSysNo;
        public int? StockSysNo
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

        private string m_CompanyCode;
        public string CompanyCode
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

        #region 扩展属性

        #endregion 扩展属性
    }
}
