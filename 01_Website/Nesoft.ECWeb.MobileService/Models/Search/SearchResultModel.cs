using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;

namespace Nesoft.ECWeb.MobileService.Models.Search
{
    public class SearchResultModel
    {
        /// <summary>
        /// 搜索结果列表
        /// </summary>
        public List<ProductItemModel> ProductListItems { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PageInfo { get; set; }

        /// <summary>
        /// 可用的筛选项
        /// </summary>
        public List<SearchFilterModel> Filters { get; set; }
    }
}