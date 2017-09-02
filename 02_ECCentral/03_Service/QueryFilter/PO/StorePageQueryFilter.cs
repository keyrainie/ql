using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class StorePageQueryFilter
    {
        public StorePageQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        public int? SysNo { get; set; }

        public int? MerchantSysNo { get; set; }

        public string PageType { get; set; }
    }
}
