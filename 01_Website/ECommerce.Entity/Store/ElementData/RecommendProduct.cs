using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store.ElementData
{
    public class RecommendProduct
    {
        public int Sysno { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Priority { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string OriginalID { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        public string OriginalImage { get; set; }
        /// <summary>
        /// 商品原价格
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 商品原标题
        /// </summary>
        public string OriginalTitle { get; set; }
    }
}
