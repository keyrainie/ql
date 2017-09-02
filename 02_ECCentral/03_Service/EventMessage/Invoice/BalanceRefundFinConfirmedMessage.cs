using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class BalanceRefundFinConfirmedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_BalanceRefundFin_Confirmed"; }
        }

        /// <summary>
        /// IPP3.dbo.Finance_ReturnPrepay.SysNo
        /// </summary>
        public int ReturnPrepaySysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
