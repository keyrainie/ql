using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 品牌申请
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrandRequestInfo : ICompany, ILanguage
    {
        /// <summary>
        /// 申请状态
        /// </summary>
        [DataMember]
        public string ReustStatus { get; set; }

        /// <summary>
        /// 产品线
        /// </summary>
        [DataMember]
        public string ProductLine { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// 品牌本地化名称
        /// </summary>
        [DataMember]
        public LanguageContent BrandNameLocal { get; set; }

        /// <summary>
        /// 品牌国际化名称
        /// </summary>
        [DataMember]
        public string BrandNameGlobal { get; set; }

        /// <summary>
        /// 品牌状态
        /// </summary>
        [DataMember]
        public ValidStatus Status { get; set; }

        /// <summary>
        /// 品牌描述
        /// </summary>
        [DataMember]
        public LanguageContent BrandDescription { get; set; }

        /// <summary>
        /// 所属生产商
        /// </summary>
        [DataMember]
        public ManufacturerInfo Manufacturer { get; set; }

        /// <summary>
        /// 品牌支持信息
        /// </summary>
        [DataMember]
        public BrandSupportInfo BrandSupportInfo { get; set; }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public UserInfo User { get; set; }

        [DataMember]
        public string BrandCode { get; set; }
    }
}
