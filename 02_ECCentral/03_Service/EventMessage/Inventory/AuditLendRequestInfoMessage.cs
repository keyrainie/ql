using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 审核借货单
    /// </summary>
    public class AuditLendRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_LendRequestInfo_Audit"; }
        }

        /// <summary>
        ///借货单编号
        /// </summary>
        public int LendRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
