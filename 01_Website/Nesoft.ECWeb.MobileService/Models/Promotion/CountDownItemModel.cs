using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.Product;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class CountDownItemModel
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
        /// 标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品促销语
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 剩余抢购时间(单位秒)
        /// </summary>
        public int LeftSeconds { get; set; }
        /// <summary>
        /// 距离开始时间（单位秒）
        /// </summary>
        public String StartTime { get; set; }

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