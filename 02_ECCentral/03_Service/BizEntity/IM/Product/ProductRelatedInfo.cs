using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 相关商品信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductRelatedInfo : IIdentity, ICompany, ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 主商品SysNo
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 主商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }

        /// <summary>
        /// 相关商品ID
        /// </summary>
        [DataMember]
        public string RelatedProductID { get; set; }
        /// <summary>
        /// 相关商品SysNo
        /// </summary>
        [DataMember]
        public int RelatedProductSysNo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 是否相互相关
        /// </summary>
        [DataMember]
        public bool IsMutual { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }
    }
}
