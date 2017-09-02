using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.IM
{
    /// <summary>
    /// 品牌审核拒绝
    /// </summary>
    public class BrandRejectMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Brand_Rejected";
            }
        }
        /// <summary>
        /// 拒绝用户编号
        /// </summary>
        public int RejectUserSysNo { get; set; }
        /// <summary>
        /// 申请编号
        /// </summary>
        public int RequestSysNo { get; set; }
    }
}
