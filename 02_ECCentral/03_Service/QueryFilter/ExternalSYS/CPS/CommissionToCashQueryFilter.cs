using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.QueryFilter.ExternalSYS
{
  public class CommissionToCashQueryFilter
    {
      public PagingInfo PageInfo { get; set; }

      /// <summary>
      /// 会员账号
      /// </summary>
      public string CustomerID { get; set; }

      /// <summary>
      /// 申请日期开始
      /// </summary>
      public DateTime? ApplicationDateFrom { get; set; }

      /// <summary>
      /// 申请日期到
      /// </summary>
      public DateTime? ApplicationDateTo { get; set; }

      /// <summary>
      /// 申请状态
      /// </summary>
      public ToCashStatus? Status { get; set; }
    }
}
