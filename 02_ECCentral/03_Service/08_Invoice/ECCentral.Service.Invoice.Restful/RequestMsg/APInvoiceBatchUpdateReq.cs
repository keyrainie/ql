using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class APInvoiceBatchUpdateReq
    {
        public List<int> SysNoList { get; set; }

        public string VPCancelReason { get; set; }
    }
}
