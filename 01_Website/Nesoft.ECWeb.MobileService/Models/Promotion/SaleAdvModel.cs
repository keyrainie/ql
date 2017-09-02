using Nesoft.ECWeb.MobileService.Models.Banner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    /// <summary>
    /// 促销模板信息
    /// </summary>
    public class SaleAdvModel
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 组下商品列表
        /// </summary>
        public List<RecommendItemModel> ItemList { get; set; }
    }
}