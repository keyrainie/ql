using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品资源
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductResourceForNewegg : ICompany, ILanguage
    {
        [DataMember]
        public int ProductCommonInfoSysNo { get; set; }

        /// <summary>
        /// 资源信息
        /// </summary>
        [DataMember]
        public ResourceForNewegg Resource { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public int? ProductResourceSysNo { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [DataMember]
        public ProductResourceIsShow IsShow { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public UserInfo OperateUser { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编号
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 是否添加水印
        /// </summary>
        [DataMember]
        public bool IsNeedWatermark { get; set; }
    }
}
