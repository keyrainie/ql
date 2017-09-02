using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 厂商支持信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ManufacturerSupportInfo
    {
        /// <summary>
        /// 客服电话
        /// </summary>
        [DataMember]
        public String ServicePhone { get; set; }

        /// <summary>
        /// 售后支持邮箱
        /// </summary>
        [DataMember]
        public String ServiceEmail { get; set; }

        /// <summary>
        /// 售后支持链接
        /// </summary>
        [DataMember]
        public String ServiceUrl { get; set; }

        /// <summary>
        /// 厂商链接
        /// </summary>
        [DataMember]
        public String ManufacturerUrl { get; set; }
    }
}
