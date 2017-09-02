using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.RMA
{
    /// <summary>
    /// RMA-退款成功后发送的Message类
    /// </summary>
    public class RMARefundCompletedMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        /// 退款单编号
        /// </summary>
        public int RefundSysNo { get; set; }
    }
}
