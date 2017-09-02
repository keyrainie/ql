using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class TopItemFilter
    {
        public PagingInfo PageInfo { get; set; }
        public int? PageType { get; set; }
        public int? RefSysNo { get; set; }
        public int? C1SysNo { get; set; }
        public int? FrontPageSize { get; set; }
        public string ProductID { get; set; }
        public string CompanyCode { get; set; }
        public string ChannelID { get; set; }
    }
}
