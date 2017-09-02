using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 页面
    /// </summary>
    public class WebPage
    {
        /// <summary>
        /// 前台网站页面编号
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        ///  前台网站页面名称
        /// </summary>
        public string PageName { get; set; }
    }
}
