using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Product
{
    public class ProductBasicInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 商品Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品简称
        /// </summary>
        public string BriefName{get;set;}
        /// <summary>
        /// 型号
        /// </summary>
        public string ProductMode{get;set;}
        /// <summary>
        /// 描述
        /// </summary>
        public string ProductDesc { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public int? Weight { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string ProductDescLong { get; set; }

        public string ProductPhotoDesc { get; set; }

        /// <summary>
        /// 规格参数
        /// </summary>
        public string Performance { get; set; }

        /// <summary>
        /// 售后
        /// </summary>
        public string Warranty { get; set; }

        public string Attention { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus ProductStatus { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ProductType ProductType { get; set; }
        /// <summary>
        ///默认图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 图片版本
        /// </summary>
        public string ImageVersion { get; set; }
        /// <summary>
        /// 是否代销
        /// </summary>
        public bool IsConsign { get; set; }
        /// <summary>
        /// 促销标题
        /// </summary>
        public string PromotionTitle { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }


        public string KeyWords { get; set; }

        public int ProductCommonInfoSysNo { get; set; }
        /// <summary>
        /// 商品组No
        /// </summary>
        public int ProductGroupSysNo { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        public string ProductGroupName { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int VendorSysno { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 商品进口信息
        /// </summary>
        public ProductEntryInfo ProductEntryInfo { get; set; }

        /// <summary>
        /// 是否是限时促销商品
        /// </summary>
        public bool IsCountDownItem { get; set; }

        /// <summary>
        /// 聚合商品数量【搜索用】
        /// </summary>
        public int PolymericProductCount { get; set; }

        public VendorInfo VendorInfo
        {
            get;
            set;
        }

        public GroupPropertyInfo GroupPropertyInfo
        {
            get;
            set;
        }

        public Product_ReviewMaster ReviewInfo { get; set; }
        /// <summary>
        /// 贸易类型
        /// </summary>
        public TradeType ProductTradeType { get; set; }
        /// <summary>
        /// 导购商品URL
        /// </summary>
        public string ShoppingGuideURL { get; set; }

        /// <summary>
        /// Gets or sets the brief name addition.
        /// </summary>
        /// <value>
        /// The brief name addition.
        /// </value>
        public string BriefNameAddition {get;set;}

    }
}
