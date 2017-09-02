using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class SysConfigQueryFilter
    {
        public string ChannelID { get; set; }
        public string ConfigType { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
