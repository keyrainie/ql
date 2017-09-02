using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class EIMSInvoiceEntryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? VendorNumber { get; set; }

        public string VendorName { get; set; }

        public string RuleAssignedCode { get; set; }

        public string AssignedCode { get; set; }

        public int? EIMSType { get; set; }

        public int? ReceiveType { get; set; }

        public string Status { get; set; }

        public string InvoiceInputStatus { get; set; }

        public string IsSAPImported { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceInputNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
