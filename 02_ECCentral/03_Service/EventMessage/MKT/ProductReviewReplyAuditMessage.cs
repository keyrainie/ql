using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.MKT
{
    [Serializable]
    public class ProductReviewReplyAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_ProductReviewReply_Audited";
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
