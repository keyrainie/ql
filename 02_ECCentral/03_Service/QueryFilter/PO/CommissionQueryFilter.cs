using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class CommissionQueryFilter
    {
        public CommissionQueryFilter()
        {
            PageInfo = new PagingInfo();
        }
        public PagingInfo PageInfo { get; set; }

        public int? SysNo { get; set; }

        public int? VendorSysNo { get; set; }

        public string VendorName { get; set; }

        public DateTime? InDateBegin { get; set; }

        public DateTime? InDateEnd { get; set; }

        public DateTime? OutListDateBegin { get; set; }

        public DateTime? OutListDateEnd { get; set; }

        public string CompanyCode { get; set; }
    }
}
