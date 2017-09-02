using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 供应商退款单审核拒绝
    /// </summary>
    public class VendorRefundInfoRejectMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_VENDORREFUNDINFO_REJECTED";
            }
        }
        /// <summary>
        /// 拒绝用户编号
        /// </summary>
        public int RejectUserSysNo { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
    }
}
