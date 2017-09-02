using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品产地
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductCountry
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        /// <summary>
        /// 国家代码
        /// </summary>
        [DataMember]
        public string CountryCode { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        [DataMember]
        public string CountryName { get; set; }
    }
}
