using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class GroupBuyQueryModel
    {
        /// <summary>
        /// 是否请求筛选项
        /// </summary>
        public bool GetFilters { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public int? CatSysNo { get; set; }

        /// <summary>
        /// 排序类型
        /// 0-默认排序
        /// 10-销量升序
        /// 11-销量降序
        /// 20-价格升序
        /// 21-价格降序
        /// 30-评论升序
        /// 31-评论降序
        /// 40-上架时间升序
        /// 41-上架时间降序
        /// </summary>
        public int SortType { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}