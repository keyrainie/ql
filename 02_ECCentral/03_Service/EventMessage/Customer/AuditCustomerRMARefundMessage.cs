using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Customer
{
    /// <summary>
    /// 审核通过 RMA退款单 Message
    /// </summary>
    public class AuditCustomerRMARefundMessage : Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_RMARefund_Audited";
            }
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }


        public int CurrentUserSysNo { get; set; }
    }
}
