using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.SO
{
    [Serializable]
    public class SOCreatedCustomerAbandonRequestMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SOCustomerAbandonRequest_Created";
            }
        }
        /// <summary>
        /// 作废订单申请编号
        /// </summary>
        public int RequestSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int SOSysNo
        {
            get;
            set;
        }

        public int MasterSOSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///  
        /// </summary>
        public int CreateUserSysNo { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public string CreateUserName { get; set; }

    }


    [Serializable]
    public class SOCanceledCustomerAbandonRequestMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SOCustomerAbandonRequest_Canceled";
            }
        }
        /// <summary>
        /// 作废订单申请编号
        /// </summary>
        public int RequestSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CancelUserSysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CancelUserName { get; set; }

    }

    [Serializable]
    public class SOCompletedCustomerAbandonRequestMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SOCustomerAbandonRequest_Completed";
            }
        }
        /// <summary>
        /// 作废订单申请编号
        /// </summary>
        public int RequestSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单审核用户编号
        /// </summary>
        public int CompleteUserSysNo { get; set; }

        /// <summary>
        /// 订单审核用户名
        /// </summary>
        public string CompleteUserName { get; set; }

    }
}
