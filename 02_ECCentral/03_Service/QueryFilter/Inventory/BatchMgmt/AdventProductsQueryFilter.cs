using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class AdventProductsQueryFilter
    {
        public AdventProductsQueryFilter()
        {
            this.PageInfo = new PagingInfo();
        }
        public PagingInfo PageInfo { get; set; }

        public int? BrandSysNo { get; set; }

        public int? CategoryC3SysNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
