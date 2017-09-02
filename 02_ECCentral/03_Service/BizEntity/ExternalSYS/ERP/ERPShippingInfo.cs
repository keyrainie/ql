using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Serializable]
    [DataContract]
    public class ERPShippingInfo
    {
        /// <summary>
        /// 送货单编号
        /// </summary>
        [DataMember]
        public int? SHDSysNo { get; set; }

        /// <summary>
        /// 送货单关联单据号
        /// </summary>
        [DataMember]
        public string RefOrderNo { get; set; }

        /// <summary>
        /// 送货单关联单据类型
        /// </summary>
        [DataMember]
        public string RefOrderType { get; set; }

        /// <summary>
        /// 送货单状态，0 未配送，1，已配送
        /// </summary>
        [DataMember]
        public int? ShippingStatus { get; set; }

    }
}
