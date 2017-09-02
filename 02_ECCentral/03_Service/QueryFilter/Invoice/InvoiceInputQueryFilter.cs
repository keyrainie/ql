using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class InvoiceInputQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }
        public int? VendorSysNo { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public DateTime? ConfirmDateFrom { get; set; }
        public DateTime? ConfirmDateTo { get; set; }
        public string POList { get; set; }
        public string APList { get; set; }
        public APInvoiceMasterStatus? Status { get; set; }
        public bool? HasDiff { get; set; }
        public int? DocNo { get; set; }
        public int? ComeFrom { get; set; }
        public int? PaySettleCompany { get; set; }
        public string CompanyCode
        {
            get;
            set;
        }

        public string ChannelID
        {
            get;
            set;
        }
    }
}
