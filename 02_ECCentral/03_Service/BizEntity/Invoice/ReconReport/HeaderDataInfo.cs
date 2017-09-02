using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class HeaderDataInfo
    {
        public int? TransactionNumber { get; set; }

        public DateTime? PostingDate { get; set; }

        public string CompanyCode { get; set; }

        public string DocumentType { get; set; }

        public string GLAccount { get; set; }

        public Decimal? SAP_GLAmount { get; set; }

        public string FI_DOC { get; set; }

        public string LineItem { get; set; }

        public string FiscalYear { get; set; }

        public DateTime? InDate { get; set; }
    }

    public class CompanyCode
    {
        public string SapCoCode { get; set; }
    }
}
