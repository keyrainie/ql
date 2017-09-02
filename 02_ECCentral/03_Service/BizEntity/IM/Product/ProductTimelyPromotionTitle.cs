using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM.Product
{
    /// <summary>
    /// 商品实效促销
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductTimelyPromotionTitle : ICompany,ILanguage
    {
        /// <summary>
        /// 促销标题
        /// </summary>
        [DataMember]
        public LanguageContent PromotionTitle { get; set; }

        /// <summary>
        /// 促销标题类型
        /// </summary>
        [DataMember]
        public String PromotionTitleType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public TimelyPromotionTitleStatus Status { get; set; }

        /// <summary>
        /// 促销语开始时间
        /// </summary>
        [DataMember]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 促销语结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

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
        [DataMember]
        public UserInfo OperateUser { get; set; } 
    }
}
