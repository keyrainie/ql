using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.App
{
    [Serializable]
    [XmlRoot("MobileAppConfig")]
    public class MobileAppConfig
    {
        /// <summary>
        /// ArticleID of 关于我们
        /// </summary>
        [XmlElement("ArticleIDAboutUs")]
        public int ArticleIDAboutUs { get; set; }


        /// <summary>
        ///  Count of 首页新品上市商品列表
        /// </summary>
        [XmlElement("CountRecommendProductItemList")]
        public int CountRecommendProductItemList { get; set; }

        /// <summary>
        ///  Count of 首页今日特卖商品列表
        /// </summary>
        [XmlElement("CountTodaySaleItemList")]
        public int CountTodaySaleItemList { get; set; }

        /// <summary>
        ///  Count of 首页热卖推荐商品列表
        /// </summary>
        [XmlElement("CountHomeHotSaleItemList")]
        public int CountHomeHotSaleItemList { get; set; }

        /// <summary>
        /// Count of 首页品牌推荐商品列表
        /// </summary>
        [XmlElement("CountHomeBrandItemList")]
        public int CountHomeBrandItemList { get; set; }

        /// <summary>
        ///  Count of 首页超低折扣商品列表
        /// </summary>
        [XmlElement("CountHomeDiscountItemList")]
        public int CountHomeDiscountItemList { get; set; }

        /// <summary>
        ///  PositionID of 首页今日特卖商品列表
        /// </summary>
        [XmlElement("PositionIDTodaySaleItemList")]
        public int PositionIDTodaySaleItemList { get; set; }

        /// <summary>
        ///  PositionID of 首页热卖推荐商品列表
        /// </summary>
        [XmlElement("PositionIDHomeHotSaleItemList")]
        public int PositionIDHomeHotSaleItemList { get; set; }

        /// <summary>
        /// PositionID of 首页品牌推荐商品列表
        /// </summary>
        [XmlElement("PositionIDHomeBrandItemList")]
        public int PositionIDHomeBrandItemList { get; set; }

        /// <summary>
        ///  PositionID of 首页超低折扣商品列表
        /// </summary>
        [XmlElement("PositionIDHomeDiscountItemList")]
        public int PositionIDHomeDiscountItemList { get; set; }

        /// <summary>
        /// PageCode of App首页楼层编号
        /// </summary>
        [XmlElement("PageCodeFloorAppHome")]
        public int PageCodeFloorAppHome { get; set; }

        /// <summary>
        /// PageID of App首页
        /// </summary>
        [XmlElement("PageIDAppHome")]
        public int PageIDAppHome { get; set; }

        /// <summary>
        /// 手机服务网站地址
        /// </summary>
        [XmlElement("MobileAppServiceHost")]
        public string MobileAppServiceHost { get; set; }

        /// <summary>
        /// Crash Log email to
        /// </summary>
        [XmlElement("CrashLogEmailTo")]
        public string CrashLogEmailTo { get; set; }

        /// <summary>
        /// 商品介绍Html模板
        /// </summary>
        [XmlElement("ProductDescTemplate")]
        public string ProductDescTemplate { get; set; }

        /// <summary>
        /// 商品规格参数Html模板
        /// </summary>
        [XmlElement("ProductSpecTemplate")]
        public string ProductSpecTemplate { get; set; }

        /// <summary>
        /// 新闻类内容Html模板
        /// </summary>
        [XmlElement("NewsContentTemplate")]
        public string NewsContentTemplate { get; set; }
    }
}