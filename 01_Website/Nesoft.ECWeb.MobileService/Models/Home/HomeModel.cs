using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Banner;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.MobileService.Models.Promotion;
using System.Web.Script.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Home
{
    public class HomeModel
    {
        /// <summary>
        /// 首页Banner
        /// </summary>
        public List<BannerModel> Banners { get; set; }

        /// <summary>
        /// 首页新品上市商品列表
        /// </summary>
        public List<RecommendItemModel> RecommendProducts { get; set; }


        /// <summary>
        /// 首页今日特卖商品列表
        /// </summary>
        public List<RecommendItemModel> TodayHotSaleProducts { get; set; }

        /// <summary>
        /// 首页热卖商品列表
        /// </summary>
        public List<RecommendItemModel> HotSaleProducts { get; set; }

        /// <summary>
        /// 首页楼层列表
        /// </summary>
        public List<FloorModel> Floors { get; set; }

        /// <summary>
        /// 精选品牌
        /// </summary>
        public RecommendBrandModel Brands { get; set; }

        /// <summary>
        /// 限时抢购
        /// </summary>
        public List<CountDownItemModel> CountDownList { get; set; }
    }
}