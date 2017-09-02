using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品批发价
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductWholeSalePriceInfo
    {
        /// <summary>
        /// 批发等级
        /// </summary>
        [DataMember]
        public WholeSaleLevelType Level { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int? Qty { get; set; }

        /// <summary>
        /// 批发价格
        /// </summary>
        [DataMember]
        public decimal? Price { get; set; }
    }
}
