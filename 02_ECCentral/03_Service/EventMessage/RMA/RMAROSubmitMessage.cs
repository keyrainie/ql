using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.RMA
{
    /// <summary>
    /// RMA-RO单提交审核Message 
    /// </summary>
    public class RMAROSubmitMessage : ECCentral.Service.Utility.EventMessage
    {

        public int SOIncomeRefundSysNo { get; set; }
        public int CurrentUserSysNo { get; set; }
        public override string Subject
        {
            get
            {

                return "ECC_RMARO_Submited";
            }
        }
    }

    /// <summary>
    ///  RMA-RO单取消提交审核Message
    /// </summary>
    public class RMAROCancelSubmitMessage : ECCentral.Service.Utility.EventMessage
    {

        public int SOIncomeRefundSysNo { get; set; }
        public int CurrentUserSysNo { get; set; }
        public override string Subject
        {
            get
            {

                return "ECC_RO_CancelSubmited";
            }
        }
    }
}
