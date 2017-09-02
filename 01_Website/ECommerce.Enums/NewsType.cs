using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    /// <summary>
    /// 新闻类型
    /// </summary>
    [Serializable]
    public enum NewsType
    {
        None = 0,
        /// <summary>
        /// 首页新闻公告                                                                                              
        /// </summary>
        HomePageNews = 1,
        /// <summary>
        /// 首页促销新闻                                                                                            
        /// </summary>
        HomePagePromotionNews=1001,
    }
}
