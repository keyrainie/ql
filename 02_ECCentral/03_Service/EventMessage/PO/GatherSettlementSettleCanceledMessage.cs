using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 取消结算代收结算单
    /// </summary>
    public class GatherSettlementSettleCanceledMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_GATHERSETTLEMENTSETTLE_CANCELED"; }
        }
        /// <summary>
        /// 结算单编号
        /// </summary>
        public int SettlementSysNo { get; set; }
        /// <summary>
        /// 当前用户编号
        /// </summary>
        public int CurrentUserSysNo { get; set; }
    }
}
