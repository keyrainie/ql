using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 供应商等级申请审核取消
    /// </summary>
    public class VendorRankRequestCancelMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_VENDORRANKREQUEST_CANCELED";
            }
        }
        /// <summary>
        /// 取消用户编号
        /// </summary>
        public int CancelUserSysNo { get; set; }
        /// <summary>
        /// 申请编号
        /// </summary>
        public int RequestSysNo { get; set; }
    }
}
