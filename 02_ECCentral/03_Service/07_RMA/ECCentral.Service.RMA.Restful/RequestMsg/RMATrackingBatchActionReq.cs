using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.RMA.Restful.RequestMsg
{
    public class RMATrackingBatchActionReq
    {
        public List<int> SysNoList { get; set; }

        public int? HandlerSysNo
        {
            get;
            set;
        }
    }
}
