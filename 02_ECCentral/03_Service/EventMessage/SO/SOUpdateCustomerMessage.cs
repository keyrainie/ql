using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.SO
{
    /// <summary>
    /// 修改订单预订人信息
    /// </summary>
    [Serializable]
    public class SOUpdateCustomerMessage : ECCentral.Service.Utility.EventMessage
    {
        /// <summary>
        /// 主单订单编号
        /// </summary>
        public int SOSysNo
        {
            get;
            set;
        }

        public SOCustomer Customer
        {
            get;
            set;
        }
    }
}
