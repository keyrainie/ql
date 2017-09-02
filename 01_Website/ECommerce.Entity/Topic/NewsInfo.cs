using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    [Serializable]
    public class NewsInfo
    {
        public int SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 封面图
        /// </summary>
        public string CoverImageUrl { get; set; }

        public string LinkUrl { get; set; }

        public int Priority { get; set; }

        public int TopMost { get; set; }

        public DateTime CreateDate { get; set; }

        public string Content { get; set; }

        public NewsType Type { get; set; }

        public int ReferenceSysNo { get; set; }

        public int PageShowInheritance { get; set; }
    }
}
