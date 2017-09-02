using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 品牌信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrandInfo 
    {

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        [DataMember]
        public string BrandID { get; set; }

        /// <summary>
        /// 品牌本地化名称
        /// </summary>
        [DataMember]
        //public LanguageContent BrandNameLocal { get; set; }
        public string BrandNameLocal { get; set; }

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
        //public LanguageContent BrandDescription { get; set; }
        public string BrandDescription { get; set; }

        /// <summary>
        /// 所属生产商
        /// </summary>
        [DataMember]
        public ManufacturerInfo Manufacturer { get; set; }

        /// <summary>
        /// 生产商SysNo
        /// </summary>
        [DataMember]
        public int? ManufacturerSysNo { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        [DataMember]
        public string ManufacturerName { get; set; }

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
        /// 品牌故事
        /// </summary>
        [DataMember]
        public string BrandStory { get; set; }

    }
}
