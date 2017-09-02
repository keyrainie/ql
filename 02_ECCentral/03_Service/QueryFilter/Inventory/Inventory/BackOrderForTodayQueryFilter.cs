using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class BackOrderForTodayQueryFilter
    {

        public PagingInfo PageInfo { get; set; }

        public string VendorSysNo { get; set; }

        public string VendorName { get; set; }
    }
}
