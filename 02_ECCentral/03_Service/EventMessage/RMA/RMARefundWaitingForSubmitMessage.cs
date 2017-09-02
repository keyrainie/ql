using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.RMA
{
    public class RMACreateRefundWaitingForSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        ///  退款单编号
        /// </summary>
        public int RefundSysNo { get; set; }

        public override string Subject
        {
            get
            {
                return "ECC_CreateRMARefund_Submited";
            }
        }

        public int CurrentUserSysNo { get; set; }
    }

    public class RMACompleteRefundWaitingForSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        ///  退款单编号
        /// </summary>
        public int RefundSysNo { get; set; }

        public override string Subject
        {
            get
            {
                return "ECC_CompleteRMARefund_Submited";
            }
        }

        public int CurrentUserSysNo { get; set; }
    }
}
