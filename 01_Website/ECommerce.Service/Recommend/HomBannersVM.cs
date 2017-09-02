using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;

namespace ECommerce.Facade.Recommend
{
    /// <summary>
    /// 首页所有Banner
    /// </summary>
    public class HomBannersVM
    {
        /// <summary>
        /// 首页轮播广告
        /// </summary>
        public List<BannerInfo> Sliders { get; set; }

        /// <summary>
        /// 搜索框右边
        /// </summary>
        public BannerInfo SearchRight { get; set; }

        /// <summary>
        /// 新闻公告下面
        /// </summary>
        public BannerInfo UnderNews { get; set; }

        /// <summary>
        /// 首页底部精选品牌
        /// </summary>
        public List<BannerInfo> Bottom { get; set; }
    }
}
