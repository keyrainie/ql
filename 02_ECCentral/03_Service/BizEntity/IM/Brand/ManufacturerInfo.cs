//************************************************************************
// 用户名				泰隆优选
// 系统名				厂商管理实体
// 子系统名		        厂商基本信息实体
// 作成者				Tom.H.Li
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 生产商信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ManufacturerInfo : IIdentity,ICompany,ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 生产商ID
        /// </summary>
        [DataMember]
        public string ManufacturerID { get; set; }

        /// <summary>
        /// 生产商本地化名称
        /// </summary>
        [DataMember]
        public LanguageContent ManufacturerNameLocal { get; set; }

        /// <summary>
        /// 生产商国际化名称
        /// </summary>
        [DataMember]
        public string ManufacturerNameGlobal { get; set; }

        /// <summary>
        /// 生产商状态
        /// </summary>
        [DataMember]
        public ManufacturerStatus Status { get; set; }

        /// <summary>
        /// 生产商描述
        /// </summary>
        [DataMember]
        public LanguageContent ManufacturerDescription { get; set; }

        /// <summary>
        /// 生产商信息
        /// </summary>
        [DataMember]
        public ManufacturerSupportInfo SupportInfo { get; set; }

        /// <summary>
        /// 店铺类型
        /// </summary>
        [DataMember]
        public BrandStoreType BrandStoreType { get; set; }

        /// <summary>
        /// 是否有LOGO
        /// </summary>
        [DataMember]
        public bool IsLogo { get; set; }

        /// <summary>
        /// 是否前台专区显示
        /// </summary>
        [DataMember]
        public string IsShowZone { get; set; }

        /// <summary>
        /// 品牌店广告图
        /// </summary>
        [DataMember]
        public string BrandImage { get; set; }

        /// <summary>
        /// 前台专卖店URL
        /// </summary>
        [DataMember]
        public string ShowUrl { get; set; }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }
    }
}
