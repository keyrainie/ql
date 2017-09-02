using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.Restful.RequestMsg
{
    public class PurchaseOrderRetreatReq
    {
        public int? poSysNo { get; set; }
        public string retreatType { get; set; }
    }
}
