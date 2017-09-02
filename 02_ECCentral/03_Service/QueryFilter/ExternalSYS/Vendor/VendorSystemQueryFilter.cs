using ECCentral.QueryFilter.Common;
using System;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class VendorSystemQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? RegionSysNo { get; set; }

        public int? CategorySysNo { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string LogType { get; set; }
    }
}
