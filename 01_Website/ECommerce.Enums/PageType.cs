using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    [Serializable]
    public enum PageType
    {
        All = -1,
        Home = 0,
        /// <summary>
        /// 一级类别
        /// </summary>
        TabStore = 1,
        /// <summary>
        /// 二级类别
        /// </summary>
        MidCategory = 2,
        /// <summary>
        /// 三级类别
        /// </summary>
        SubStore=3,
        /// <summary>
        /// 品牌专区
        /// </summary>
        BrandZone = 6,
        /// <summary>
        /// 品牌商品搜索
        /// </summary>
        BrandProductSearch = 7,
        /// <summary>
        /// 全部品牌
        /// </summary>
        Brands=7000,

        ShoppingCart=5000,
        /// <summary>
        /// 限时抢购
        /// </summary>
        CountDown = 5001,
        /// <summary>
        /// 团购
        /// </summary>
        GroupBuying = 6000,
        /// <summary>
        /// App首页
        /// </summary>
        PageTypeAppHome = 7001,
        /// <summary>
        /// 登录页面
        /// </summary>
        Login = 4000,
        /// <summary>
        /// 注册页面
        /// </summary>
        Register = 3000,
    }
}
