using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.PO
{
    /// <summary>
    /// 购单预计到货时间取消审核
    /// </summary>
    public class PurchaseOrderETATimeInfoCancelMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_PURCHASEORDERETATIMEINFO_CANCELED";
            }
        }
        /// <summary>
        /// 取消用户编号
        /// </summary>
        public int CancelUserSysNo { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int ETATimeSysNo { get; set; }
    }
}
