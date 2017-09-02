using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Banner;
using System.Web.Script.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class FloorModel
    {
        public FloorModel()
        {
            Banner = new BannerModel();
            ItemList = new List<RecommendItemModel>();
            
        }

        /// <summary>
        /// 楼层名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 楼层广告
        /// </summary>
        public BannerModel Banner { get; set; }

        /// <summary>
        /// 推荐商品列表
        /// </summary>
        public List<RecommendItemModel> ItemList { get; set; }
    }
}