using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 作废损益单
    /// </summary>
    public class VoidAdjustRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_AdjustRequestInfo_Void"; }
        }

        /// <summary>
        ///借货单编号
        /// </summary>
        public int AdjustRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
