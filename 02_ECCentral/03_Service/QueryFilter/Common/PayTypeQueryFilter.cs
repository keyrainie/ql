using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class PayTypeQueryFilter
    {
        public int? SysNo { get; set; }
        public string PayTypeName { get; set; }

        public HYNStatus? IsOnlineShow { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
