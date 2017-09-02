using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 生产商信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ManufacturerInfo 
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
        //public LanguageContent ManufacturerNameLocal { get; set; }
        public string ManufacturerNameLocal { get; set; }

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
        //public LanguageContent ManufacturerDescription { get; set; }
        public string ManufacturerDescription { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }
    }
}
