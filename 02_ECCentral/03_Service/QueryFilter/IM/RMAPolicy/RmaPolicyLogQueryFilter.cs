using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class RmaPolicyLogQueryFilter
    {
       public PagingInfo PagingInfo { get; set; }

       public int? RmaPolicySysNO { get; set; }

       public DateTime? UpdateDateTo { get; set; }

       public int? RmaPolicy { get; set; }
       public DateTime? UpdateDateFrom { get; set; }

       public string EidtUserName { get; set; }
    }
}
