using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class CustomerExperienceLogQueryFilter 
    {
        public PagingInfo PagingInfo { get; set; }

        public int CustomerSysNo { get; set; }

        public DateTime? CreateTimeTo { get; set; }

        public DateTime? CreateTimeFrom { get; set; }
    }
}
