using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Topic
{
    public class NewsQueryFilter : QueryFilterBase
    {
        /// <summary>
        /// 新闻编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 新闻类型
        /// </summary>
        public int? ReferenceSysNo { get; set; }


        public NewsType? NewsType { get; set; }

        /// <summary>
        /// 评论级别
        /// </summary>
        public int? PageShowInheritance { get; set; }
    }
}
