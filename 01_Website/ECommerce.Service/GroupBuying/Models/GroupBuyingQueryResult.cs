using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Promotion.GroupBuying;

namespace ECommerce.Facade.GroupBuying.Models
{
    /// <summary>
    /// 团购筛选信息
    /// </summary>
    [Serializable]
    public class GroupBuyingQueryResult
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        public GroupBuyingQueryInfo QueryInfo { get; set; }
        /// <summary>
        /// 团购分类
        /// </summary>
        public List<GroupBuyingCategoryInfo> CategoryList { get; set; }
        /// <summary>
        /// 查询结果
        /// </summary>
        public QueryResult<GroupBuyingInfo> Result { get; set; }
    }
}
