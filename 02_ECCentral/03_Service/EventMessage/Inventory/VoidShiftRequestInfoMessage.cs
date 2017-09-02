using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 作废移仓单
    /// </summary>
    public class VoidShiftRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_ShiftRequestInfo_Void"; }
        }

        /// <summary>
        ///移仓单据号
        /// </summary>
        public int ShiftRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
