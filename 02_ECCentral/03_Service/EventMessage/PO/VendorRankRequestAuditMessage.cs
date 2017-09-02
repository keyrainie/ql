using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 供应商等级申请审核通过
    /// </summary>
    public class VendorRankRequestAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_VENDORRANKREQUEST_AUDITED";
            }
        }
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public int AuditUserSysNo { get; set; }
        /// <summary>
        /// 申请编号
        /// </summary>
        public int RequestSysNo { get; set; }
    }
}
