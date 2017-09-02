using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 代销结算单作废
    /// </summary>
    public class ConsignSettlementAbandonMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_CONSIGNSETTLEMENT_ABANDONED";
            }
        }
        /// <summary>
        /// 当前用户编号
        /// </summary>
        public int CurrentUserSysNo { get; set; }
        /// <summary>
        /// 代销结算单编号
        /// </summary>
        public int SysNo { get; set; }
    }
}
