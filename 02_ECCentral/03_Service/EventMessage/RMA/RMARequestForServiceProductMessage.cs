using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.RMA
{
    /// <summary>
    /// 服务类商品申请单维护-确认时发送Message
    /// </summary>
    public class RMARequestForServiceProductMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        ///  申请单编号
        /// </summary>
        public int RegisterSysNo { get; set; }
        public override string Subject
        {
            get
            {
                return "ECC_RMAServiceProduct_Request";
            }
        }


        /// <summary>
        /// true:套餐卡,false服务类
        /// </summary>
        public bool? SoType { get; set; }

        public int CategoryId { get; set; }

        public int CurrentUserSysNo { get; set; }

        public int RMASysNo { get; set; }
    }
}
