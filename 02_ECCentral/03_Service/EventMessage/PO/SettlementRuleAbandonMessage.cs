using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 作废代销商品规则
    /// </summary>
    public class SettlementRuleAbandonMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SETTLEMENTRULE_ABANDONED";
            }
        }
        /// <summary>
        /// 作废用户编号
        /// </summary>
        public int AbandonUserSysNo { get; set; }
        /// <summary>
        /// 规则代码
        /// </summary>
        public string SettleRulesCode { get; set; }
    }
}
