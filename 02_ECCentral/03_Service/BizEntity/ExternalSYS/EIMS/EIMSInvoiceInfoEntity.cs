using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class EIMSInvoiceInfoEntity
    {
        public int SysNo { get; set; }

        public int? VendorNumber { get; set; }

        public string InvoiceInputNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? InvoiceInputAmount { get; set; }

        public decimal? TaxRate { get; set; }

        public DateTime? InvoiceInputDateTime { get; set; }

        public string InvoiceInputUser { get; set; }

        public DateTime? InvoiceEditDateTime { get; set; }

        public string InvoiceEditUser { get; set; }

        public string Memo { get; set; }

        public int Status { get; set; }

        public string CurrentUser { get; set; }

        public List<EIMSInvoiceInputExtendInfo> EIMSInvoiceInputExtendList { get; set; }
    }
    public class EIMSInvoiceInputExtendInfo
    {
        public int SysNo { get; set; }

        public string InvoiceNumber { get; set; }

        public int InvoiceInputSysNo { get; set; }

        public int Status { get; set; }
    }
}
