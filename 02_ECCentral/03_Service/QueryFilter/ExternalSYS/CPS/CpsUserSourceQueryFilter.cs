using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
  public class CpsUserSourceQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

      /// <summary>
      /// 用户编号
      /// </summary>
        public int UserSysNo { get; set; }
    }
}
