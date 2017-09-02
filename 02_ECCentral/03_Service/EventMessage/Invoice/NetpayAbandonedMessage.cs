using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    public class NetpayAbandonedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_Netpay_Voided"; }
        }

        /// <summary>
        /// ipp3.dbo.Finance_NetPay.SysNo
        /// </summary>
        public int NetpaySysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
