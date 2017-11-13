using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品维护基础信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductMaintainBasicInfo : EntityBase
    {
        /// <summary>
        /// 商品组编号
        /// </summary>
        [DataMember]
        public int? ProductGroupSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int? ProductSysNo { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }
        /// <summary>
        /// 商品简称
        /// </summary>
        [DataMember]
        public string BriefName { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        [DataMember]
        public string ProductTitle { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [DataMember]
        public string Keywords { get; set; }
        /// <summary>
        /// C3
        /// </summary>
        [DataMember]
        public int C3SysNo { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [DataMember]
        public string CategoryText { get; set; }
        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int BrandSysNo { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public string ProductType { get; set; }
        [DataMember]
        public string UPCCode { get; set; }
        [DataMember]
        public string BMCode { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMember]
        public string ProductDesc { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        [DataMember]
        public string ProductDescLong { get; set; }
        /// <summary>
        /// 详细规格（根据一般属性生成）
        /// </summary>
        [DataMember]
        public string Performance { get; set; }
        /// <summary>
        /// 包装清单
        /// </summary>
        [DataMember]
        public string PackageList { get; set; }
        /// <summary>
        /// 图片描述
        /// </summary>
        [DataMember]
        public string ProductPhotoDesc { get; set; }
        /// <summary>
        /// 保修条款
        /// </summary>
        [DataMember]
        public string Warranty { get; set; }
        /// <summary>
        /// 购买须知
        /// </summary>
        [DataMember]
        public string Attention { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note { get; set; }
        /// <summary>
        /// 生产商编号
        /// </summary>
        [DataMember]
        public int ManufacturerSysNo { get; set; }
        /// <summary>
        /// 商品官网链接
        /// </summary>
        [DataMember]
        public string ProductLink { get; set; }
        /// <summary>
        /// 是否大货
        /// </summary>
        [DataMember]
        public string IsLarge { get; set; }
        [DataMember]
        public int CurrencySysNo { get; set; }
        /// <summary>
        /// 是否拍照
        /// </summary>
        [DataMember]
        public int IsTakePictures { get; set; }
        /// <summary>
        /// 是否设置同组商品（0-不设置；1-设置）
        /// </summary>
        [DataMember]
        public int IsGroupProduct { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public int Weight { get; set; }
        [DataMember]
        public int PMUserSysNo { get; set; }
        [DataMember]
        public int PPMUserSysNo { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        [DataMember]
        public string OriginCode { get; set; }
        /// <summary>
        /// 前台类别
        /// </summary>
        [DataMember]
        public int FrontCategorySysNo { get; set; }
        /// <summary>
        /// 贸易类型
        /// </summary>
        [DataMember]
        public TradeType ProductTradeType { get; set; }
        /// <summary>
        /// 一般属性列表
        /// </summary>
        [DataMember]
        public List<CategoryPropertyInfo> NormalProperties { get; set; }
        /// <summary>
        /// 一般属性值列表
        /// </summary>
        [DataMember]
        public List<CategoryPropertyValueInfo> NormalPropertyValues { get; set; }
        /// <summary>
        /// 产地列表
        /// </summary>
        [DataMember]
        public List<ProductCountry> CountryCodeList { get; set; }
        /// <summary>
        /// 选择的属性列表
        /// </summary>
        [DataMember]
        public List<ProductPropertyInfo> SelectNormalProperties { get; set; }

        public ProductMaintainBasicInfo()
        {
            //默认正常品
            this.ProductType = "O";
            //默认重量为0
            this.Weight = 0;
            //默认不是大货
            this.IsLarge = "N";
            //默认为 默认生产商
            this.ManufacturerSysNo = 1;
            //默认无保修条款
            this.Warranty = "";
            //默认IPPAdmin
            this.PMUserSysNo = 3105;
            //默认IPPAdmin
            this.PPMUserSysNo = 3105;
            //默认人民币
            this.CurrencySysNo = 1;
            this.NormalProperties = new List<CategoryPropertyInfo>();
            this.NormalPropertyValues = new List<CategoryPropertyValueInfo>();
            this.SelectNormalProperties = new List<ProductPropertyInfo>();
        }
        /// <summary>
        /// 是否撮合交易
        /// </summary>
        [DataMember]
        public int IsMatchedTrading { get; set; }
    }
}