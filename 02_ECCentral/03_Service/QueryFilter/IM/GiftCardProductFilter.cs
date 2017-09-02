using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter
{
    public class GiftCardProductFilter
    {
        public PagingInfo PageInfo { get; set; }

        public decimal? Price { get; set; }

        public int? SysNo { get; set; }

        public int? RelationSysNo { get; set; }
    }
}
