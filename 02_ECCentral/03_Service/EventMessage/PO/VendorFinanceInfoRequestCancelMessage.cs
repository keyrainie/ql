using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 供应商财务信息申请取消审核
    /// </summary>
    public class VendorFinanceInfoRequestCancelMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_VENDORFINANCEINFOREQUEST_CANCELED";
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
