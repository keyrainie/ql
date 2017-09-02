using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 取消作废损益单
    /// </summary>
    public class CancelVoidAdjustRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_AdjustRequestInfo_CancelVoid"; }
        }

        /// <summary>
        ///损益单编号
        /// </summary>
        public int AdjustRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
