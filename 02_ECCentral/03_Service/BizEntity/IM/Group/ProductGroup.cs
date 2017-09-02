using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品组
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductGroup : IIdentity,ICompany,ILanguage
    {
        /// <summary>
        /// 商品组标题
        /// </summary>
        [DataMember]
        public LanguageContent ProductGroupName { get; set; }

        /// <summary>
        /// 商品组型号系列
        /// </summary>
        [DataMember]
        public LanguageContent ProductGroupModel { get; set; }

        /// <summary>
        /// 分组属性设置
        /// </summary>
        [DataMember]
        public IList<ProductGroupSettings> ProductGroupSettings { get; set; }

        /// <summary>
        /// 组内商品列表
        /// </summary>
        [DataMember]
        public List<ProductInfo> ProductList { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言代码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public UserInfo OperateUser { get; set; }
    }
}
