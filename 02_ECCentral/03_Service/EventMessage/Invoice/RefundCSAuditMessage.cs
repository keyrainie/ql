using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// CS审核 --退款审核
    /// </summary>
    public class RefundCSAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_RefundCS_Audit"; }
        }

        /// <summary>
        /// 退款单编号
        /// </summary>
        public int RefundSysNo { get; set; }


        public int CurrentUserSysNo { get; set; }
    }
}
