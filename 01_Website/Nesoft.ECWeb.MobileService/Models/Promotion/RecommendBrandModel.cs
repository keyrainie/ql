using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    /// <summary>
    /// 精选品牌
    /// </summary>
    public class RecommendBrandModel
    {
        /// <summary>
        /// 大图
        /// </summary>
        public List<RecommendBrandItemModel> BigBrands { get; set; }

        /// <summary>
        /// 小图
        /// </summary>
        public List<RecommendBrandItemModel> SmallBrands { get; set; }
    }
}