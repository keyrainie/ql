using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class SMSQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Tel { get; set; }
        public string CompanyCode { get; set; }
        public string WebChannel { get; set; }
    }
}
