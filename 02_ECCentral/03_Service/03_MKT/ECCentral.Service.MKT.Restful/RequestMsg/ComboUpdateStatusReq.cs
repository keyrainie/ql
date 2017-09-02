using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.Restful.RequestMsg
{
    public class ComboUpdateStatusReq
    {
        public int? SysNo{get;set;}
        public ComboStatus? TargetStatus { get; set; }

    }
}
