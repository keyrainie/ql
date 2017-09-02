using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    [Serializable]
    public enum RecommendPosition
    {
        /// <summary>
        /// 首页热卖商品推荐
        /// </summary>
        Home_HotProduct = 1001,
        /// <summary>
        /// 首页品牌商品推荐
        /// </summary>
        Home_BrandProduct = 1002,
        /// <summary>
        /// 首页折扣商品推荐
        /// </summary>
        Home_DiscountProduct = 1003,
        /// <summary>
        /// 大类-广告位左侧-本周推荐
        /// </summary>
        TabStore_HotProduct = 2001,
        /// <summary>
        /// 大类-广告位下侧-新品上市
        /// </summary>
        TabStore_NewProduct = 2002,
        /// <summary>
        /// 大类-广告位下侧-本周特惠
        /// </summary>
        TabStore_SuperSpecial = 2003,

        /// <summary>
        /// 大类-左侧-热卖商品
        /// </summary>
        TabStore_HotSalesProduct = 2004,

        SubStore_HotSale = 3001,

        /// <summary>
        /// 中类本周推荐
        /// </summary>
        MidCategory_ThisWeekProduct = 4001,

        /// <summary>
        /// 中类热卖商品
        /// </summary>
        MidCategory_HotSalesProduct = 4002,

        /// <summary>
        /// 团购页热销排行
        /// </summary>
        GroupBuy_HotSalesProduct = 6001,
    }
}
