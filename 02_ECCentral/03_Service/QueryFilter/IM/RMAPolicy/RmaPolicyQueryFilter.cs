using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class RmaPolicyQueryFilter
    {
       public PagingInfo PagingInfo { get; set; }

       public int? SysNo { get; set; }

       public string CreateUserName { get; set; }

       public RmaPolicyType? Type { get; set; }

       public RmaPolicyStatus? Status { get; set; }
    }
}
