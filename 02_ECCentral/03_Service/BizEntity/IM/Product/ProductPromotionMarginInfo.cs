using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.MKT;
namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品促销折扣信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductPromotionMarginInfo
    {
        /// <summary>
        /// 促销类型
        /// </summary>
        [DataMember]
        public PromotionType PromotionType { get; set; }

        /// <summary>
        /// 促销编号
        /// </summary>
        [DataMember]
        public int ReferenceSysNo { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        [DataMember]
        public decimal Margin { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [DataMember]
        public string ReturnMsg { get; set; }
    }
}
