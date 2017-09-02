using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Common
{
    public class ShipTypePayTypeQueryFilter
    {
        public int? SysNo { get; set; }
        public int? ShipTypeSysNo { get; set; }
        public int? PayTypeSysNo { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
