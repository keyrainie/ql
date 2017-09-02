using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品图片信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductImageInfo : EntityBase
    {
        /// <summary>
        /// 商品图片编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// 资源相对路径
        /// </summary>
        [DataMember]
        public string ResourceUrl { get; set; }
        /// <summary>
        /// 商品默认图片
        /// </summary>
        [DataMember]
        public string DefaultImage { get; set; }
    }
}
