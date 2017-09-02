using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.BizEntity.Invoice.SAP
{
    public class SAPCompanyInfo : IIdentity, ICompany
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
        public int? StockID { get; set; }

        public string StockName { get; set; }

        public string SAPCompanyCode { get; set; }

        public string SAPBusinessArea { get; set; }

        public decimal? SalesTaxRate { get; set; }

        public decimal? PurchaseTaxRate { get; set; }

        public SAPStatus? Status { get; set; }

        public SAPStatus? WorkStatus { get; set; }

    }
}
