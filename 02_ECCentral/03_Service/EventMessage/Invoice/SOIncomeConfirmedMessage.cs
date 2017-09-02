using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class SOIncomeConfirmedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_SOIncome_Confirmed"; }
        }

        /// <summary>
        /// ipp3.dbo.Finance_SOIncome.SysNo
        /// </summary>
        public int SOIncomeSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
