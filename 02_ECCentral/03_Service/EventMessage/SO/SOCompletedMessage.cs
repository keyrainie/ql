using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.SO
{
    [Serializable]
    public class SOCompletedMessage : ECCentral.Service.Utility.EventMessage
    {

        public override string Subject
        {
            get
            {
                return "ECC_SO_Completed";
            }
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 主订单编号，表示子订单（SOSysNo）对应的主订单编号
        /// </summary> 
        public int? MasterSOSysNo { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }

        public int CompletedUserSysNo { get; set; }

        public string CompletedUserName { get; set; }

    }
}
