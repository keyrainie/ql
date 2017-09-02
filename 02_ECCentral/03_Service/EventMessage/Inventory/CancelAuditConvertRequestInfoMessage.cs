using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 取消审核转换
    /// </summary>
    public class CancelAuditConvertRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_ConvertRequestInfo_CancelAudit"; }
        }

        /// <summary>
        ///转换单编号
        /// </summary>
        public int ConvertRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
