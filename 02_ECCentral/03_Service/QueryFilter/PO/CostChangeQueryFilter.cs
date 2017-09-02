using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.PO
{
    public class CostChangeQueryFilter
    {
        public CostChangeQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }
        public string VendorName { get; set; }
        public int? VendorSysNo { get; set; }
        public CostChangeStatus? Status { get; set; }
        public int? PMSysNo { get; set; }
        public string Memo { get; set; }
        public string CompanyCode { get; set; }
    }
}
