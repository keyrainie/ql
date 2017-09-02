using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 创建出纳退款完成Message 
    /// </summary>
    public class CreateRefundCashierRefundMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_RefundAudit_CashierRefundCreated"; }
        }

        /// <summary>
        /// ipp3.dbo.Finance_SOIncome_BankInfo.SysNo
        /// </summary>
        public int RefundSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
