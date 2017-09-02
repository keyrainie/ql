using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.IM
{
    /// <summary>
    /// 品牌审核提交
    /// </summary>
    public class BrandAuditSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_BRAND_SUBMITED";
            }
        }
        /// <summary>
        /// 提交用户编号
        /// </summary>
        public int SubmitUserSysNo { get; set; }
        /// <summary>
        /// 申请编号
        /// </summary>
        public int RequestSysNo { get; set; }
    }
}
