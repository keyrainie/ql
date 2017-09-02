using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 创建代销商品规则
    /// </summary>
    public class SettlementRuleCreateMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SETTLEMENTRULE_CREATED";
            }
        }
        /// <summary>
        /// 创建用户编号
        /// </summary>
        public int CreateUserSysNo { get; set; }
        /// <summary>
        /// 规则代码
        /// </summary>
        public string SettleRulesCode { get; set; }
    }
}
