using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities
{
    //页面类型
    [Serializable]
    public enum PageType : int
    {
        //[Description("首页")]
        //Homepage = 0,
        [Description("一级类页面")]
        C1page = 1,
        [Description("二级类页面")]
        C2page = 2,
        [Description("三级类页面")]
        C3page = 3,
        [Description("专卖店")]
        Brand = 4,
        [Description("购物车页面")]
        Shopcart = 5,
        [Description("百元店品牌专属")]
        HundredShopManufacturer = 6,
        [Description("百元店类别专属")]
        HundredShopCategory = 7,
        [Description("百元店推荐专属")]
        HundredShopRecommend = 8,
        [Description("百元店")]
        HundredShop = 9,
        [Description("所有新品")]
        NewItem = 10
    }


    //商品推荐管理 位置描述
    [Serializable]
    public enum HomePage : int
    {
        [Description("特价快报1")]
        Special1 = 21,
        [Description("特价快报2")]
        Special2 = 31,
        [Description("特价快报3")]
        Special3 = 41,
        [Description("特价快报4")]
        Special4 = 51,
        [Description("首页推荐1")]
        HomeRecommended1 = 23,
        [Description("首页推荐2")]
        HomeRecommended2 = 33,
        [Description("首页推荐3")]
        HomeRecommended3 = 43,
        [Description("首页推荐4")]
        HomeRecommended4 = 53
    }

    [Serializable]
    public enum Category1 : int
    {
        [Description("大类--今日推荐")]
        StarItem = 200,
        [Description("大类--新品上架")]
        New = 201,
        [Description("大类--特价商品")]
        Pricedown = 202,
        [Description("大类--自定义项")]
        Userdefined = 203
    }

    [Serializable]
    public enum Category2 : int
    {
        [Description("中类--新品上架")]
        New = 201,
        [Description("中类--特别推荐")]
        Reccommend = 205,
        [Description("中类--自定义项")]
        Userdefined = 203
    }
    [Serializable]
    public enum Category3 : int
    {
        [Description("小类--今日特惠")]
        SmallSpecialOfferToday = 31
    }
    [Serializable]
    public enum Brand : int
    {
        [Description("品牌专卖--新品上架")]
        New = 31,
        [Description("品牌专卖--让利促销")]
        Promotion = 32,
        [Description("品牌专卖--当季热销")]
        Hot = 33
    }
    [Serializable]
    public enum ShopCart : int
    {
        [Description("立即换购")]
        ImmediatelyChange = 10
    }

    [Serializable]
    public enum RecommendedHundredExclusiveShop : int
    {
        [Description("推荐专属")]
        RecommendedExclusive = 31
    }

    [Serializable]
    public enum DomainList : int
    {
        [Description("大")]
        Big = 100,
        [Description("小")]
        Small = 101
    }

    [Serializable]
    public enum HundredShop : int
    {
        [Description("应季商品")]
        SeasonalItem = 41,
        [Description("节日商品1")]
        FestivalItem1 = 42,
        [Description("节日商品2")]
        FestivalItem2 = 43,
        [Description("主题商品")]
        TopicItem = 44,
        [Description("百元品推荐")]
        HundredItemRecommend = 45,
        [Description("最新降价")]
        LatestPrice = 46,
        [Description("百元新品1")]
        HundredNew1 = 47,
        [Description("百元新品2")]
        HundredNew2 = 48,
        [Description("百元新品3")]
        HundredNew3 = 49,
        [Description("百元新品4")]
        HundredNew4 = 50
    }
}
