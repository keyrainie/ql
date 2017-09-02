using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// PO单审核确认
    /// </summary>
    public class PurchaseOrderConfirmMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_PURCHASEORDER_CONFIRMED";
            }
        }
        /// <summary>
        /// 确认用户编号
        /// </summary>
        public int ConfirmUserSysNo { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
    }
}
