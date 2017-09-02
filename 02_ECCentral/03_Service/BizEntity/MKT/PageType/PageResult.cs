using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 页面结果
    /// </summary>
    public class PageResult
    {
        /// <summary>
        /// 页面类型展示方式
        /// </summary>
        public PageTypePresentationType PresentationType { get; set; }

        /// <summary>
        /// 页面集合
        /// </summary>
        public List<WebPage> PageList { get; set; }
    }
}
