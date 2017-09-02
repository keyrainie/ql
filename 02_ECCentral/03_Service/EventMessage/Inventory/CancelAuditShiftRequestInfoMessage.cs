using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
   public class CancelAuditShiftRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
       /// <summary>
       /// 取消审核以仓单
       /// </summary>
        public override string Subject
        {
            get { return "ECC_ShiftRequestInfo_CancelAudit"; }
        }

        /// <summary>
        ///移仓单据号
        /// </summary>
        public int ShiftRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
