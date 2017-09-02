using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品属性
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductProperty : ICompany,ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 属性组
        /// </summary>
        [DataMember]
        public PropertyGroupInfo PropertyGroup { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        [DataMember]
        public PropertyValueInfo Property { get; set; }

        /// <summary>
        /// 自定义值
        /// </summary>
        [DataMember]
        public LanguageContent PersonalizedValue { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        [DataMember]
        public ProductPropertyRequired Required { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo OperationUser { get; set; }
    }
}
