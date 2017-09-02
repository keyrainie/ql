using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    [Serializable]
    public class GroupBuySaveMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_GroupBuy_Created";
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
