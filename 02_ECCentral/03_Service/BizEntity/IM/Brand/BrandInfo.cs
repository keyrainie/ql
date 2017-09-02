//************************************************************************
// 用户名				泰隆优选
// 系统名				品牌管理实体
// 子系统名		        品牌基本信息实体
// 作成者				Tom.H.Li
// 改版日				2012.4.21
// 改版内容				新建
//************************************************************************


using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;
namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 品牌信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrandInfo : IIdentity,ICompany,ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

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

        /// <summary>
        /// 店铺类型
        /// </summary>
        [DataMember]
        public BrandStoreType BrandStoreType { get; set; }

        /// <summary>
        /// 是否有logo
        /// </summary>
        [DataMember]
        public string IsLogo { get; set; }

         /// <summary>
        /// 品牌故事
        /// </summary>
        [DataMember]
        public string BrandStory { get; set; }

        /// <summary>
        /// 商品ID 用于连接
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

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
        public UserInfo User;


        /// <summary>
        /// 用于生成ProductId
        /// </summary>
        [DataMember]
        public string BrandCode { get; set; }
    }
}
