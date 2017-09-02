using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.M.Models.Search
{
    public class SearchCriteriaModel
    {
        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int? BrandID { get; set; }

        /// <summary>
        /// 商品3级分类系统编号
        /// </summary>
        public int? Category3ID { get; set; }

        /// <summary>
        /// 商品1级分类系统编号
        /// </summary>
        public int? Category1ID { get; set; }

        public int? PageSize { get; set; }

        public int? PageIndex { get; set; }
    }
}