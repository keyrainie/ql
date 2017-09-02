using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    /// <summary>
    /// 限时抢购审核拒绝消息体
    /// </summary>
    [Serializable]
    public class CountDownAuditRejectMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Countdown_AuditRejected";
            }
        }

        /// <summary>
        /// 限时抢购系统编号
        /// </summary>
        public int CountdownSysNo { get; set; }

        /// <summary>
        /// 限时抢购名称
        /// </summary>
        public string CountdownName { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
