using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    public partial class ProductInfo
    {
        /// <summary>
        /// 对应的京东商品
        /// </summary>
        [DataMember]
        public string JDProductID { get; set; }

        /// <summary>
        /// 对应的亚马逊商品
        /// </summary>
        [DataMember]
        public string AMProductID { get; set; }

        /// <summary>
        /// 合作商品映射关系
        /// </summary>
        [DataMember]
        public IList<ProductMapping> ProductMappingList { get; set; }

        /// <summary>
        /// 自动调价
        /// </summary>
        [DataMember]
        public AutoAdjustPrice AutoAdjustPrice { get; set; }

        /// <summary>
        /// 最后一次申请调价信息
        /// </summary>
        [DataMember]
        public ProductPriceRequestInfo LastProductPriceRequestInfo { get; set; }
    }
}
