using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class GroupBuyQueryResult
    {
        /// <summary>
        /// 团购分类
        /// </summary>
        public List<GroupBuyCatModel> Filters { get; set; }
        /// <summary>
        /// 查询结果
        /// </summary>
        public QueryResult<GroupBuyBaseModel> Result { get; set; }
    }
}