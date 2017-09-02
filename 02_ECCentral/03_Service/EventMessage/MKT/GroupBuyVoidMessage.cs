using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    /// <summary>
    /// 作废消息体
    /// </summary>
    [Serializable]
    public class GroupBuyVoidMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_GroupBuy_Voided";
            }
        }

        /// <summary>
        /// 团购系统编号
        /// </summary>
        public int GroupBuySysNo { get; set; }

        /// <summary>
        /// 团购名称
        /// </summary>
        public string GroupBuyName { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
