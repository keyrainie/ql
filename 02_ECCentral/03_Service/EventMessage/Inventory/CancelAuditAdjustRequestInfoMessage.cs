using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 取消审核损益单
    /// </summary>
    public class CancelAuditAdjustRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_AdjustRequestInfo_CancelAudit"; }
        }

        /// <summary>
        ///借货单编号
        /// </summary>
        public int AdjustRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
