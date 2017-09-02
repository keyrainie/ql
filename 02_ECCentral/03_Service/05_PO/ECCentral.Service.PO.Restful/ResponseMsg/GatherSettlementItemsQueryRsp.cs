using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.Restful.ResponseMsg
{
    public class GatherSettlementItemsQueryRsp
    {
        public int TotalCount { get; set; }
        public List<GatherSettlementItemInfo> ResultList { get; set; }
    }
}
