using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    public class UnifiedImageQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string ImageName { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeTo { get; set; }

    }
}
