using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class RejectUnicomOrderMessage:IEventMessage
    {
        public MessageHeader Header { get; set; }
        public int OrderSysNo { get; set; }
        public OPType Type { get; set; }

        public string Subject
        {
            get { return "RejectUnicomOrderMessage"; }
        }
    }

    public enum OPType
    {
        AbandonSO = 1,
        RejectSO = 2,
    }
}
