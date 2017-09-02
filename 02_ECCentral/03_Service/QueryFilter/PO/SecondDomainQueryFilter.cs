using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class SecondDomainQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public List<int> VendorSysNoList { get; set; }
    }
}
