using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
   public class ProductAccessoriesQueryConditionPreViewQueryFilter
    {
       public PagingInfo PagingInfo { get; set; }

       public int ConditionValueSysNo1 { get;  set; }
       public int ConditionValueSysNo2 { get;  set; }
       public int ConditionValueSysNo3 { get;  set; }
       public int ConditionValueSysNo4 { get;  set; }

       public int? Category1SysNo { get; set; }
       public int? Category2SysNo { get; set; }
       public int? Category3SysNo { get; set; }

       public string ProductID { get; set; }
       public int MasterSysNo { get; set; }

    }
}
