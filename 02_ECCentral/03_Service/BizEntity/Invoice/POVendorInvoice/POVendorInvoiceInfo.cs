using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class POVendorInvoiceInfo : IIdentity, ICompany
    {

        public int? SysNo
        {
            get;
            set;
        }
        public string CompanyCode
        {
            get;
            set;
        }

        public int? VendorSysNo { get; set; }

        public string InvoiceNumber { get; set; }

        public int? StockSysNo { get; set; }

        public DateTime? InvoiceTime { get; set; }

        public DateTime? InputTime { get; set; }

        public int? InputUserSysNo { get; set; }


        public DateTime? UpdateTime { get; set; }

        public int? UpdateUserSysNo { get; set; }


        public DateTime? AuditTime { get; set; }

        public int? AuditUserSysNo { get; set; }


        public InvoiceStatus? Status { get; set; }

        public decimal? TaxAmt { get; set; }

        public decimal? GoodsAmt { get; set; }

        public decimal? TotalAmt { get; set; }

        public string Note { get; set; }
    }
}
