using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.Restful.RequestMsg
{
    public class VirtualRequestInfoReq
    {
        public bool CanOperateItemOfLessThanPrice { get; set; }
        public bool CanOperateItemOfSecondHand { get; set; }

        public List<VirtualRequestInfo> RequestList { get; set; }
        public VirtualRequestInfoReq()
        {
            RequestList = new List<VirtualRequestInfo>();
        }
    }
}
