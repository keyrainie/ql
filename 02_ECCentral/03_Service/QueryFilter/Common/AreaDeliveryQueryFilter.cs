using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Common
{
    public class AreaDeliveryQueryFilter
    {
        public int? WHArea { get; set; }
        public string City { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
