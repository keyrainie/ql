using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.Restful.RequestMsg
{
    public class ConsignSettlementEIMSQueryRsq
    {
        public PagingInfo PageInfo { get; set; }

        public ConsignSettlementInfo queryCondition { get; set; }
    }
}
