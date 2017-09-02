using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 创建netpay消息
    /// </summary>
    public class CreateNetpayMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_Netpay_Created"; }
        }

        /// <summary>
        /// ipp3.dbo.Finance_NetPay.SysNo
        /// </summary>
        public int NetpaySysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
