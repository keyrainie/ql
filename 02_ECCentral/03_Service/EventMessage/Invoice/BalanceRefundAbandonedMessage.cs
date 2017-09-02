using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 用户余额退款作废消息
    /// </summary>
    public class BalanceRefundAbandonedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_BalanceRefund_Voided"; }
        }

        /// <summary>
        /// IPP3.dbo.Finance_ReturnPrepay.SysNo
        /// </summary>
        public int ReturnPrepaySysNo { get; set; }

        public int CurrentUserSysNo { get; set; }

        public int LastRefundStatus { get; set; }
    }
}
