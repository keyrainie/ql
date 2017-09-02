using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
  public class PurgeToolQueryFilter
    {
      public PagingInfo PageInfo { get; set; }

      /// <summary>
      /// 清除类型
      /// </summary>
      public ClearType ClearType { get; set; }
    }
}
