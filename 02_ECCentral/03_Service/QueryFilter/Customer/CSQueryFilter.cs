using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    public class CSQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public CSRole? Role { get; set; }
        public string Name { get; set; }
        public bool IsGetUnderling { get; set; }
        public string CompanyCode { get; set; }
    }
}
