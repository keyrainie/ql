using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.MobileService.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    public class StoreProductListModel
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