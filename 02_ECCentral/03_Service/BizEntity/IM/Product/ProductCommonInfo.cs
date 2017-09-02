using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品固有信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductCommonInfo : ILanguage
    {
        /// <summary>
        /// 商品基本信息
        /// </summary>
        [DataMember]
        public ProductBasicInfo ProductBasicInfo { get; set; }

        /// <summary>
        /// 商品质保信息
        /// </summary>
        [DataMember]
        public ProductWarrantyInfo ProductWarrantyInfo { get; set; }

        /// <summary>
        /// 固有信息编号
        /// </summary>
        [DataMember]
        public int? ProductCommonInfoSysNo { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 商品组系统编号
        /// </summary>
        public int ProductGroupSysno { get; set; }
    }
}
