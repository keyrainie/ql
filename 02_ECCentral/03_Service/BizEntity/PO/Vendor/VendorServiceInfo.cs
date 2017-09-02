using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 供应商售后支持信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class VendorServiceInfo
    {
        /// <summary>
        /// 售后联系人
        /// </summary>
        [DataMember]
        public string Contact { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public string ContactPhone { get; set; }

        /// <summary>
        /// 省市区
        /// </summary>
        [DataMember]
        public AreaInfo AreaInfo { get; set; }

        /// <summary>
        /// 售后地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        
        /// <summary>
        /// 邮编
        /// </summary>
        [DataMember]
        public string ZipCode { get; set; }


        /// <summary>
        /// 售后服务范围
        /// </summary>
        [DataMember]
        public string RMAServiceArea { get; set; }

        /// <summary>
        /// 退货策略
        /// </summary>
        [DataMember]
        public string RMAPolicy { get; set; }
    }
}
