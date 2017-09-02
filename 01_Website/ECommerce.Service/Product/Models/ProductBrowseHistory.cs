using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Product.Models
{
    [Serializable]
    public class ProductBrowseHistory
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 最后浏览时间
        /// </summary>
        public string ViewTime { get; set; }
    }
}
