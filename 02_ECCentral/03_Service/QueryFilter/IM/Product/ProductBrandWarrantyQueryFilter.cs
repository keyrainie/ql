using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ProductBrandWarrantyQueryFilter
    { 
        public int? BrandSysNo { get; set; }
        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }
        public PagingInfo PageInfo { get; set; }
    }
}
