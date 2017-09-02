using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
namespace ECCentral.QueryFilter.PO
{
    public class ConsignSettlementProductsQueryFilter
    {
        public ConsignSettlementProductsQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        public int? ProductSysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public int? StockSysNo { get; set; }
        public int? Category1SysNo { get; set; }
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }
        public int? BrandSysNo { get; set; }
        public int? PMSysNo { get; set; }
        public List<int> PMList { get; set; }
        public string CompanyCode { get; set; }

        public int? IsConsign
        {
            get;
            set;
        }
    }
}
