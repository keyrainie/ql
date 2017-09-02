using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    [Serializable]
    public class CountDownSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Countdown_Submited";
            }
        }

        /// <summary>
        /// 限时抢购系统编号
        /// </summary>
        public int CountdownSysNo { get; set; }

        /// <summary>
        /// 限时抢购名称
        /// </summary>
        public string CountdownName { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
