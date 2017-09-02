using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.QueryFilter.Invoice
{
    public class POVendorInvoiceQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public int? VendorSysNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public DateTime? InvoiceCreateDateFrom { get; set; }
        public DateTime? InvoiceCreateDateTo { get; set; }
        public InvoiceStatus? Status { get; set; }
        public int? StockSysNo { get; set; }
        public bool? IsFilterAbandon { get; set; }
        public bool? IsVendorTotal { get; set; }
        public string CompanyCode { get; set; }
        public string ChannelID { get; set; }
    }
}
