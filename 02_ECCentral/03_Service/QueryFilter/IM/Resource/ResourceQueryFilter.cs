using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM.Resource
{
    public class ResourceQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public IList<string> CommonSKUNumberList { get; set; }
    }
}
