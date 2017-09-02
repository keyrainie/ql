using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class CouponCodeQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? CouponSysNo { get; set; }

        public int? CodeSysNoFrom { get; set; }

        public int? CodeSysNoTo { get; set; }
        
        public string CouponCode { get; set; }
        
        public DateTime? InDateFrom { get; set; }

        public DateTime? InDateTo { get; set; }
        
        public DateTime? BeginDateFrom { get; set; }

        public DateTime? EndDateTo { get; set; }
         
    }
}
