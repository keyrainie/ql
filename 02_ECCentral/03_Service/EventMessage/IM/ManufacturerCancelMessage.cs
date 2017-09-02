using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.IM
{
    /// <summary>
    /// 生产商审核取消
    /// </summary>
    public class ManufacturerCancelMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Manufacturer_Canceled";
            }
        }
        /// <summary>
        /// 取消用户编号
        /// </summary>
        public int CancelUserSysNo { get; set; }
        /// <summary>
        /// 申请系统编号
        /// </summary>
        public int RequestSysNo { get; set; }
        /// <summary>
        /// 生产商系统编号
        /// </summary>
        public int? ManufacturerSysNo { get; set; }
    }
}
