
using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品尺寸重量信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductDimensionInfo : IIdentity
    {
        /// <summary>
        /// 商品长度
        /// </summary>
        [DataMember]
        public decimal Length { get; set; }

        /// <summary>
        /// 商品宽度
        /// </summary>
        [DataMember]
        public decimal Width { get; set; }

        /// <summary>
        /// 商品高度
        /// </summary>
        [DataMember]
        public decimal Height { get; set; }

        /// <summary>
        /// 商品重量
        /// </summary>
        [DataMember]
        public decimal Weight { get; set; }

        /// <summary>
        /// 大货标记
        /// </summary>
        [DataMember]
        public Large LargeFlag { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
    }
}
