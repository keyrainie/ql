using System;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class SOLogQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int SOSysNo
        {
            get;
            set;
        }
    }
}
