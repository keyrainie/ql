using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Search
{
    public class SearchCriteriaModel
    {
        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int? BrandID { get; set; }

        /// <summary>
        /// 商品分类系统编号
        /// </summary>
        public int? CategoryID { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 一维码或二维码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 筛选值
        /// </summary>
        public string FilterValue { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortValue { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
    }
}