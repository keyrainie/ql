using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class BalanceRechargeRequestAbandonedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_BalanceRechargeRequest_Voided"; }
        }

        public int RequestSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
