using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 退款会计审核通过完成Message
    /// </summary>
    public class RefundAuditAccountantPassedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_Refund_AccountantAudited"; }
        }
        
        /// <summary>
        /// ipp3.dbo.Finance_SOIncome_BankInfo.SysNo
        /// </summary>
        public int RefundSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
