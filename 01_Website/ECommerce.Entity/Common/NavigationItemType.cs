using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    public enum NavigationItemType
    {
		/// <summary>
		/// 分类浏览
		/// </summary>
		Category,

		/// <summary>
		/// 小类类别
		/// </summary>
		SubCategory,

		/// <summary>
		/// 品牌
		/// </summary>
		Brand,

		/// <summary>
		/// 价格
		/// </summary>
		Price,

		/// <summary>
		/// 产品特性
		/// </summary>
		Attribute,

        /// <summary>
        /// 产地
        /// </summary>
        Origin,

        /// <summary>
        /// 店铺类别
        /// </summary>
        StoreCategory,
    }
}
