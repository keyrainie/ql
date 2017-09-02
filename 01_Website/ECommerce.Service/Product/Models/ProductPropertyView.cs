using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;

namespace ECommerce.Facade.Product.Models
{
   public class ProductPropertyView
    {
        /// <summary>
        /// 获取或设置当前选择商品属性
        /// </summary>
       public ProductPropertyInfo Current { get; set; }

        /// <summary>
        /// 获取或设置所有相关商品属性
        /// </summary>
       public List<ProductPropertyInfo> ProductList { get; set; }

        /// <summary>
        /// 获取或设置属性级别
        /// </summary>
       public int Type { get; set; }
    }
}
