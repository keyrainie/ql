using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Product;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class RecommendItemModel
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
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
        /// 销售相关信息
        /// </summary>
        public SalesInfoModel Price { get; set; }
    }
}