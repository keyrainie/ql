using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.PO
{
    public class CostChangeItemsQueryFilter
    {
        public CostChangeItemsQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }
        public int ProductSysNo { get; set; }
        public int PMSysNo { get; set; }
        public int VendorSysNo { get; set; }
        public string CompanyCode { get; set; }
    }
}