using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// PO单提交审核
    /// </summary>
    public class PurchaseOrderSubmitAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_PURCHASEORDER_SUBMITED";
            }
        }
        /// <summary>
        /// 提交用户编号
        /// </summary>
        public int SubmitUserSysNo { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
    }
}
