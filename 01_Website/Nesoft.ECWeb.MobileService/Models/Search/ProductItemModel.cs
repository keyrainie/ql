using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Product;

namespace Nesoft.ECWeb.MobileService.Models.Search
{
    public class ProductItemModel
    {
        public int ID { get; set; }
        public string Code { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 促销语
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 商品图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 商品价格等销售相关的信息
        /// </summary>
        public SalesInfoModel SalesInfo { get; set; }

        /// <summary>
        /// 综合评分
        /// </summary>
        public decimal ReviewScore { get; set; }

        /// <summary>
        /// 被评论数量
        /// </summary>
        public int ReviewCount { get; set; }
    }
}