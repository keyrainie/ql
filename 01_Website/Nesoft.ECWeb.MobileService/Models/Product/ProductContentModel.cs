using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ProductContentModel
    {
        /// <summary>
        /// 商品详情
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 规格参数
        /// </summary>
        public string Performance { get; set; }

        /// <summary>
        /// 购买须知
        /// </summary>
        public string Attention { get; set; }

        /// <summary>
        /// 售后服务
        /// </summary>
        public string Warranty { get; set; }
    }
}