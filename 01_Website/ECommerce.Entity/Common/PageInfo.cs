using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECommerce.Entity
{
    [Serializable]
    [DataContract]
    public class PageInfo
    {
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码：从1开始
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string SortBy { get; set; }


        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (TotalCount > 0 && PageSize > 0)
                {
                    return TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 为商品评论列表页增加查询项
        /// </summary>
        public int SearchType { get; set; }

    }
}
