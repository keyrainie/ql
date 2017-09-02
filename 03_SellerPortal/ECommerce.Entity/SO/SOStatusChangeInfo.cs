using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.SO
{

    /// <summary>
    /// 订单状态更改信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOStatusChangeInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 操作业类型
        /// </summary>
        [DataMember]
        public SOOperatorType OperatorType
        {
            get;
            set;
        }

        /// <summary>
        /// 操作者编号
        /// </summary>
        [DataMember]
        public int? OperatorSysNo
        {
            get;
            set;
        }
        /// <summary>
        ///  订单原来状态
        /// </summary>
        [DataMember]
        public SOStatus? OldStatus
        { get; set; }

        /// <summary>
        /// 订单当前状态，要更改到的状态
        /// </summary>
        [DataMember]
        public SOStatus? Status
        { get; set; }


        /// <summary>
        /// 状态更改时间
        /// </summary>
        [DataMember]
        public DateTime? ChangeTime
        { get; set; }

        /// <summary>
        /// 是否发送邮件给客户
        /// </summary>
        [DataMember]
        public bool? IsSendMailToCustomer { get; set; }

        /// <summary>
        /// 状态更改备注
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }

    }
}
