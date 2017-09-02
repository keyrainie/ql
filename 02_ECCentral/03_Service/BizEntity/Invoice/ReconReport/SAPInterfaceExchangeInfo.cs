using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.ReconReport
{
    public class SAPInterfaceExchangeInfo
    {
        public int TransactionNumber { get; set; }

        public DateTime PostingDate { get; set; }

        public string CompanyCode { get; set; }

        public string DocumentType { get; set; }

        public string GLAccount { get; set; }

        public Decimal Legacy_GLAmount { get; set; }

        public Decimal SAP_GLAmount { get; set; }

        public string AcctType { get; set; }

        public DateTime? InDate { get; set; }

        public decimal DateBalance
        {
            get { return (Legacy_GLAmount - SAP_GLAmount); }
        }

        public string AcctTypeDisplay
        {
            get
            {
                switch (AcctType)
                {
                    case "K":
                        return "AP";
                    case "D":
                        return "AR";
                    default:
                        return "";
                }
            }
        }

        public MTDInfo MTDData { get; set; }

    }

    public class MTDInfo
    {
        public string DOC_TYPE { get; set; }

        public string GL_ACCOUNT { get; set; }

        public Decimal MTDLegacy_GLAmount { get; set; }

        public Decimal MTDSAP_GLAmount { get; set; }

        public string CompanyCode { get; set; }

        public string AcctType { get; set; }

        public Decimal MTDBalance
        {
            get { return (MTDLegacy_GLAmount - MTDSAP_GLAmount); }
        }
    }
}
