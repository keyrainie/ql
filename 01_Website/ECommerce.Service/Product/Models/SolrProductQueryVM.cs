using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Product.Models
{

    public class SolrProductQueryVM
    {
        /// <summary>
        /// 是否搜索结果页
        /// </summary>
        public int IsSearchResultPage { get; set; }

        /// <summary>
        /// 二级类别
        /// </summary>
        public string MidCategoryID { get; set; }

        /// <summary>
        /// 三级类别
        /// </summary>
        public string SubCategoryID { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public string BrandID { get; set; }
    }
}
