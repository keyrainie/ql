using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class SOIncomeConfirmCanceledMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_SoIncomeConfirm_Canceled"; }
        }

        /// <summary>
        /// ipp3.dbo.Finance_SOIncome.SysNo
        /// </summary>
        public int SOIncomeSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
