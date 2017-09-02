using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class FinanceQueryFilter
    {
       public PagingInfo PageInfo { get; set; }
       public string CustomerID { get; set; }
       public string SysNoList { get; set; }
       public FinanceStatus? Status { get; set; }
       /// <summary>
       /// 开始时间
       /// </summary>
       public DateTime? SettleDateFrom { get; set; }

       /// <summary>
       /// 结束时间
       /// </summary>
       public DateTime? SettleDateTo { get; set; }
    }
}
