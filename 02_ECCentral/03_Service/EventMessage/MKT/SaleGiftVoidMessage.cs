using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    /// <summary>
    /// 赠品审核消息体
    /// </summary>
    [Serializable]
    public class SaleGiftVoidMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SaleGift_Voided";
            }
        }

        /// <summary>
        /// 赠品系统编号
        /// </summary>
        public int SaleGiftSysNo { get; set; }

        /// <summary>
        /// 赠品活动名称
        /// </summary>
        public string SaleGiftName { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
