using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Common
{
    public class PagingInfo
    {
        public int? PageIndex { get; set; }

        public int? PageSize { get; set; }


        /// <summary>
        /// 排序字符串，如：CustomerID ASC
        /// </summary>
        public string SortBy { get; set; } 
    }
}
 