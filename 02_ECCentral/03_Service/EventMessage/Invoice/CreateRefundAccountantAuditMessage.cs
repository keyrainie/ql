using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 创建会计退款审核完成Message 
    /// </summary>
    public class CreateRefundAccountantAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_RefundAudit_AccountantAuditCreated"; }
        }

        /// <summary>
        /// ipp3.dbo.Finance_SOIncome_BankInfo.SysNo
        /// </summary>
        public int RefundSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
