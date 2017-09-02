using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class ShipTypeSMSQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public int? ShipTypeSMSStatus { get; set; }
        public int? ShipType { get; set; }
        public int? SMSType { get; set; }
        public string CompanyCode { get; set; }
        public string WebChannelID { get; set; }
    }
}
