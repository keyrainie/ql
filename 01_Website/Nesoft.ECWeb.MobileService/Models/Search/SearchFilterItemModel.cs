using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Search
{
    public class SearchFilterItemModel
    {
        /// <summary>
        /// 构造好的enid
        /// </summary>
        public String EnId { get; set; }
        /// <summary>
        /// 过滤条件名
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 产品数量
        /// </summary>
        public Int32 ProductCount { get; set; }
    }
}