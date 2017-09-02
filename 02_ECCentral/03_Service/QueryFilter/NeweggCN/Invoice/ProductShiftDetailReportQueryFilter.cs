using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class ProductShiftDetailReportQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? StockSysNoA { get; set; }

        public int? StockSysNoB { get; set; }

        public DateTime? OutTimeStart { get; set; }

        public DateTime? OutTimeEnd { get; set; }

        public string GoldenTaxNo { get; set; }

        public string OutCompany { get; set; }

        public string EnterCompany { get; set; }

        public bool IsCheckDetail { get; set; }

        public string CompanyCode { get; set; }

        public List<int> StItemSysNos { get; set; }

        public bool IsCheckCompany
        {
            get
            {
                return !string.IsNullOrEmpty(OutCompany) && !string.IsNullOrEmpty(EnterCompany);
            }
        }
    }
}