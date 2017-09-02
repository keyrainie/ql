using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品基本信息
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class ProductBasicInfo
    {
        /// <summary>
        /// 商品Common SKU编号
        /// </summary>
        [DataMember]
        public string CommonSkuNumber { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        [DataMember]
        public CategoryInfo ProductCategoryInfo { get; set; }

        /// <summary>
        /// 商品品牌
        /// </summary>
        [DataMember]
        public BrandInfo ProductBrandInfo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public LanguageContent ProductTitle { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        [DataMember]
        public LanguageContent ProductModel { get; set; }

        /// <summary>
        /// 商品简名全称
        /// </summary>
        [DataMember]
        public String ProductBriefName
        {
            get
            {
                return (ProductBriefTitle == null ? "" : ProductBriefTitle.Content) + (ProductBriefAddition == null ? "" : ProductBriefAddition.Content);
            }
            set { }
        }

        /// <summary>
        /// 商品简名
        /// </summary>
        [DataMember]
        public LanguageContent ProductBriefTitle { get; set; }

        /// <summary>
        /// 商品简名附加
        /// </summary>
        [DataMember]
        public LanguageContent ProductBriefAddition { get; set; }

        /// <summary>
        /// UPC编号
        /// </summary>
        [DataMember]
        public string UPCCode { get; set; }

        /// <summary>
        /// BM编号
        /// </summary>
        [DataMember]
        public string BMCode { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public ProductType ProductType { get; set; }

        /// <summary>
        /// 商品描述 
        /// </summary>
        [DataMember]
        public LanguageContent ShortDescription { get; set; }

        /// <summary>
        /// 商品详细描述 
        /// </summary>
        [DataMember]
        public LanguageContent LongDescription { get; set; }

        /// <summary>
        /// 商品图片描述
        /// </summary>
        [DataMember]
        public LanguageContent PhotoDescription { get; set; }

        /// <summary>
        /// 包装清单
        /// </summary>
        [DataMember]
        public LanguageContent PackageList { get; set; }

        /// <summary>
        /// 官网链接
        /// </summary>
        [DataMember]
        public String ProductLink { get; set; }

        /// <summary>
        /// 注意事项
        /// </summary>
        [DataMember]
        public LanguageContent Attention { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [DataMember]
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 默认图片名
        /// </summary>
        [DataMember]
        public string DefaultImage { get; set; }

        /// <summary>
        /// 商品尺寸重量信息
        /// </summary>
        [DataMember]
        public ProductDimensionInfo ProductDimensionInfo { get; set; }

        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMember]
        public IList<ProductProperty> ProductProperties { get; set; }

        /// <summary>
        /// 商品管理员
        /// </summary>
        [DataMember]
        public ProductManagerInfo ProductManager { get; set; }

        /// <summary>
        /// 商品配件
        /// </summary>
        [DataMember]
        public IList<ProductAccessory> ProductAccessories { get; set; }

        /// <summary>
        /// 商品资源文件
        /// </summary>
        [DataMember]
        public List<ProductResourceForNewegg> ProductResources { get; set; }

        /// <summary>
        /// 入境税号
        /// </summary>
        [DataMember]
        public string TaxNo { get; set; }

        /// <summary>
        /// 入境备案号
        /// </summary>
        [DataMember]
        public string EntryRecord { get; set; }


        /// <summary>
        /// 完税价格
        /// </summary>
        [DataMember]
        public string TariffPrice { get; set; }

        [DataMember]
        public TariffInfo Tariff { get; set; }

        /// <summary>
        /// 导购URL
        /// </summary>
        [DataMember]
        public String ShoppingGuideURL { get; set; }

        /// <summary>
        /// 贸易类型
        /// </summary>
        [DataMember]
        public TradeType TradeType { get; set; }


        /// <summary>
        /// 运输存储方式
        /// </summary>
        [DataMember]
        public StoreType StoreType { get; set; }


        /// <summary>
        /// 运输存储方式
        /// </summary>
        [DataMember]
        public int? SafeQty { get; set; }
    }
}
