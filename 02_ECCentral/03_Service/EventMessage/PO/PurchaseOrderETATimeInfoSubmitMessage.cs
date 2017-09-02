using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 购单预计到货时间提交审核
    /// </summary>
    public class PurchaseOrderETATimeInfoSubmitMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_PURCHASEORDERETATIMEINFO_SUBMITED";
            }
        }
        /// <summary>
        /// 提交用户编号
        /// </summary>
        public int SubmitUserSysNo { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int ETATimeSysNo { get; set; }
    }
}
