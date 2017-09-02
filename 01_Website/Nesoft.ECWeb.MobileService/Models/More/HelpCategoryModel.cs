using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.More
{
    public class HelpCategoryModel
    {
        public HelpCategoryModel()
        {
            SubCategories = new List<HelpCategoryModel>();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 子级分类列表
        /// </summary>
        public List<HelpCategoryModel> SubCategories { get; set; }
    }
}