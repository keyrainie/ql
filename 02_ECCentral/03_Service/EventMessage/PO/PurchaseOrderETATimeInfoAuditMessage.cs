using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 购单预计到货时间审核通过
    /// </summary>
    public class PurchaseOrderETATimeInfoAuditMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_PURCHASEORDERETATIMEINFO_AUDITED";
            }
        }
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public int AuditUserSysNo { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int ETATimeSysNo { get; set; }
    }
}
