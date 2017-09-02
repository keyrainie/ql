using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.RMA
{
    /// <summary>
    /// RMA申请单待审核待办事项 Message
    /// </summary>
    public class RMACreateRequestWaitingForAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public int RequestSysNo { get; set; }

        public int SOSysNo { get; set; }

        public override string Subject
        {
            get
            {
                return "ECC_CreateRMARequest_Audited";
            }
        }
        public int CurrentUserSysNo { get; set; }

    }

    /// <summary>
    /// RMA申请单待审核待办事项 Message
    /// </summary>
    public class RMACompleteRequestWaitingForAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public int RequestSysNo { get; set; }

        public override string Subject
        {
            get
            {
                return "ECC_CompleteRMARequest_Audited";
            }
        }
        public int CurrentUserSysNo { get; set; }

    }
}
