using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品资源
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductResource
    {
        /// <summary>
        /// 资源信息
        /// </summary>
        [DataMember]
        public Resource Resource { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ProductResourceStatus Status { get; set; }
    }
}
