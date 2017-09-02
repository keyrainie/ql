using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT.Floor
{
    /// <summary>
    /// 楼层的每一个分组标签内的内容信息，包含商品、Banner图片、品牌、文本链接 4种类型
    /// </summary>
    [Serializable]
    [DataContract]
    public class FloorSectionItem
    {

        /// <summary>
        ///系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///楼层编号
        /// </summary>
        [DataMember]
        public int FloorMasterSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///分组标签编号
        /// </summary>
        [DataMember]
        public int FloorSectionSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///内容优先级，数字越小排在前面
        /// </summary>
        [DataMember]
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        ///内容类型，供支持商品、Banner图片、品牌、文本链接 4种类型
        /// </summary>
        [DataMember]
        public FloorItemType ItemType
        {
            get;
            set;
        }

        /// <summary>
        ///内容位置，一个楼层中，每种内容类型默认最多提供3个位置，协助前台进行展示
        /// </summary>
        [DataMember]
        public int ItemPosition
        {
            get;
            set;
        }



        /// <summary>
        /// 是否是当前页面打开，0=新打开页面，1=当前页面
        /// </summary>
        [DataMember]
        public int IsSelfPage { get; set; }

        /// <summary>
        ///内容详细，为XML结构，对于4种模式的对象序列化结构，
        ///统一使用ECCentral.Service.Utility.SerializationUtility.XmlSerialize和XmlDeserialize来做序列化和反序列化
        /// </summary>
        [DataMember]
        public string ItemValue
        {
            get;
            set;
        }

        [DataMember]
        public FloorItemProduct ItemProudct
        {
            get;
            set;
        }
        [DataMember]
        public FloorItemBanner ItemBanner
        {
            get;
            set;
        }
        [DataMember]
        public FloorItemBrand ItemBrand
        {
            get;
            set;
        }
        [DataMember]
        public FloorItemTextLink ItemTextLink
        {
            get;
            set;
        }
    }

    [Serializable]
    [DataContract]
    public class FloorItemProduct 
    {
        /// <summary>
        /// 商品编号——选择决定
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品标题——可重新设置
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品副标题——可重新设置
        /// </summary>
        [DataMember]
        public string ProductSubTitle { get; set; }

        /// <summary>
        /// 商品价格——不可设置
        /// </summary>
        [DataMember]
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 折扣金额——可设置
        /// </summary>
        [DataMember]
        public decimal ProductDiscount { get; set; }
        
        /// <summary>
        /// 默认图片——可重新设置
        /// </summary>
        [DataMember]
        public string DefaultImage { get; set; }

        /// <summary>
        /// 商品图片——不可设置，Service端自动取出
        /// </summary>
        [DataMember]
        public List<string> ImageList { get; set; }

    }

    [Serializable]
    [DataContract]
    public class FloorItemBanner 
    {
        /// <summary>
        /// Banner图片URL
        /// </summary>
        [DataMember]
        public string ImageSrc { get; set; }

        /// <summary>
        /// Banner文字内容
        /// </summary>
        [DataMember]
        public string BannerText { get; set; }

        /// <summary>
        /// Banner链接地址
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }

    }

    [Serializable]
    [DataContract]
    public class FloorItemBrand 
    {
        /// <summary>
        /// 品牌编号——选择决定
        /// </summary>
        [DataMember]
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 品牌图片URL
        /// </summary>
        [DataMember]
        public string ImageSrc { get; set; }

        /// <summary>
        /// 品牌文字内容
        /// </summary>
        [DataMember]
        public string BrandText { get; set; }

        /// <summary>
        /// 品牌链接地址
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }


    }

    [Serializable]
    [DataContract]
    public class FloorItemTextLink 
    {
        /// <summary>
        /// 文字内容
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }


        /// <summary>
        /// 是否需要标记热点
        /// </summary>
        [DataMember]
        public bool IsHot { get; set; }

    }
}

