using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class HolidayQueryFilter
    {
        public string HolidayDate { get; set; }
        public string BlockedService { get; set; }
        public SYNStatus? IsUntilNow { get; set; }
        public int? ShipTypeSysNo { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
