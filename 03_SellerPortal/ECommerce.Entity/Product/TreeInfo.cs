using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class TreeInfo
    {
        public int id { get; set; }

        public string text { get; set; }
        /// <summary>
        /// 特殊处理请定义
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 类型default file 要使用type就不能定义icon
        /// </summary>
        public string type { get; set; }

        public TreeState state { get; set; }

        public List<TreeInfo> children { get; set; }
    }

    public class TreeState
    {
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool opened { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool selected { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; }
    }
}
