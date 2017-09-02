using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class InvoicePrintAllQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public string SOSysNo
        {
            get;
            set;
        }

        public string StockSysNo
        {
            get;
            set;
        }

        public bool? IsVAT
        {
            get;
            set;
        }

        public ECCentral.BizEntity.SO.SOType? SOType
        {
            get;
            set;
        }

        public ECCentral.BizEntity.SO.SOStatus? SOStatus
        {
            get;
            set;
        }

        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        public DateTime? AuditDateFrom
        {
            get;
            set;
        }

        public DateTime? AuditDateTo
        {
            get;
            set;
        }

        public DateTime? OutDateFrom
        {
            get;
            set;
        }

        public DateTime? OutDateTo
        {
            get;
            set;
        }

        public ECCentral.BizEntity.Invoice.InvoiceType? InvoiceType
        {
            get;
            set;
        }

        public ECCentral.BizEntity.Invoice.StockType? StockType
        {
            get;
            set;
        }

        public ECCentral.BizEntity.Invoice.DeliveryType? ShippingType
        {
            get;
            set;
        }

        public int? VendorSysNo
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }
    }
}