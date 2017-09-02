using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.RMA
{
    public class RMAROBalanceCancelAuditMessage : ECCentral.Service.Utility.EventMessage
    {

        public int SOIncomeRefundSysNo { get; set; }
        public int CurrentUserSysNo { get; set; }

        public override string Subject
        {
            get
            {
                return "ECC_ROBalance_CancelAudited";
            }
        }
    }
}
