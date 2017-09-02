using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 借货出库
    /// </summary>
    public class OutStockLendRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_LendRequestInfo_OutStock"; }
        }

        /// <summary>
        ///借货单编号
        /// </summary>
        public int LendRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
