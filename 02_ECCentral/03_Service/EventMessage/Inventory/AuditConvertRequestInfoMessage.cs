using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 审核转换单
    /// </summary>
    public class AuditConvertRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_ConvertRequestInfo_Audit"; }
        }

        /// <summary>
        ///借货单编号
        /// </summary>
        public int ConvertRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
