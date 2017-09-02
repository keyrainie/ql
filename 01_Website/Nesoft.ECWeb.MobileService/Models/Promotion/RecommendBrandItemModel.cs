using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    /// <summary>
    /// 精选品牌
    /// </summary>
    public class RecommendBrandItemModel
    {
        /// <summary>
        /// 品牌ID
        /// </summary>
        public int BrandID { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
    }
}