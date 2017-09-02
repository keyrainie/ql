using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Home
{
    public class NewsInfoViewModel
    {
        public int SysNo { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public int Priority { get; set; }

        public int TopMost { get; set; }

        public string CreateTimeString { get; set; }

        public string Content { get; set; }

        public int Type { get; set; }

        /// <summary>
        /// 从内容中提取出来的图片地址
        /// </summary>
        public string ImageUrl { get; set; }
    }
}