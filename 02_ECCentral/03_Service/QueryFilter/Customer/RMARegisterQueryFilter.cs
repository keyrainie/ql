using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class RMARegisterQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public int? RequestSysNo { get; set; }
        public int? ProductSysNo { get; set; }
        public string CompanyCode { get; set; }
    }
}
