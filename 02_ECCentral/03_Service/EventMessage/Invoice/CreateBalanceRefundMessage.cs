using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 创建用户余额退款message
    /// </summary>
    public class CreateBalanceRefundMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_BalanceRefund_Created"; }
        }

        /// <summary>
        /// IPP3.dbo.Finance_ReturnPrepay.SysNo
        /// </summary>
        public int ReturnPrepaySysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
