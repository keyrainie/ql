using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class ShipTypeSMSTemplateQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public string Keywords { get; set; }
        public string WebChannelID { get; set; }
    }
}
