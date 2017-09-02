using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.RMA
{
    /// <summary>
    /// RMA  - 设置待退款成功后发送Message
    /// </summary>
    public class RMASetWaitingRefundedMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        ///  单件编号:
        /// </summary>
        public int RegisterSysNo { get; set; }
    }
}
