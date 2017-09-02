using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    /// <summary>
    /// 赠品提交审核消息体
    /// </summary>
    [Serializable]
    public class SaleGiftSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SaleGift_Submited";
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
