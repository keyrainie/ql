using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.Restful.ResponseMsg
{
    public class ConsignSettlementProductsQueryRsp
    {
        public int totalCount { get; set; }
        public List<ConsignSettlementItemInfo> ResultList { get; set; }
    }
}
