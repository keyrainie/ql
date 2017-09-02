using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Inventory
{
    /// <summary>
    /// 作废转换单
    /// </summary>
    public class VoidConvertRequestInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_ConvertRequestInfo_Void"; }
        }

        /// <summary>
        ///转换单编号
        /// </summary>
        public int ConvertRequestInfoSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }
    }
}
