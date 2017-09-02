using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum BannerPosition
    {
        #region 首页相关

        /// <summary>
        /// 首页轮播广告
        /// </summary>
        HomePage_Top_Slider = 1001,
        /// <summary>
        /// 首页新闻公告下面
        /// </summary>
        HomePage_UnderNews = 1002,
        /// <summary>
        /// 首页底部精选品牌
        /// </summary>
        HomePage_Bottom = 1003,

        #endregion

        /// <summary>
        /// 登录页面广告
        /// </summary>
        Login_Banner = 4001,
        /// <summary>
        /// 限时抢购页面顶部广告
        /// </summary>
        CountDown_Top = 5001,
        /// <summary>
        /// 品牌专区右上部广告
        /// </summary>
        BrandZone_TopRight = 6001,
        /// <summary>
        /// 团购频道页顶部横幅广告
        /// </summary>
        GroupBuying_TopBanner = 6101,
        /// <summary>
        /// 团购频道页右侧底部广告
        /// </summary>
        GroupBuying_RightBottomBanner = 6102,
        /// <summary>
        /// 团购详情页右侧广告
        /// </summary>
        GroupBuying_DetailRightBanner = 6103,

        /// <summary>
        /// 品牌馆左侧广告
        /// </summary>
        Brands_Left = 7001,
        /// <summary>
        /// 品牌馆中间大广告
        /// </summary>
        Brands_Big = 7002,
        /// <summary>
        /// 品牌馆顶部
        /// </summary>
        Brands_Top = 7006,
        /// <summary>
        /// App首页>顶部广告
        /// </summary>
        PositionAppHomeTopBanner = 8001,
        /// <summary>
        /// App首页>精选品牌大图
        /// </summary>
        PositionAppHomeBrandBig = 8002,
        /// <summary>
        /// App首页>精选品牌小图
        /// </summary>
        PositionAppHomeBrandSmall = 8003,

        /// <summary>
        /// 导航栏一级图片Banner
        /// </summary>
        TabStore_Navigate_Image = 100,
        /// <summary>
        /// 一级类滚动广告
        /// </summary>
        TabStore_Top_Middle = 101,
        /// <summary>
        /// 导航栏一级文字Banner
        /// </summary>
        TabStore_Navigate_Text = 102,

        /// <summary>
        /// 中类滚动banner
        /// </summary>
        MidCategory_Top_Middle=200,
        /// <summary>
        /// 中类品牌推荐
        /// </summary>
        MidCategory_Brand=201,
        /// <summary>
        /// 注册页右部广告
        /// </summary>
        Register_Right = 3001,


        
    }
}
