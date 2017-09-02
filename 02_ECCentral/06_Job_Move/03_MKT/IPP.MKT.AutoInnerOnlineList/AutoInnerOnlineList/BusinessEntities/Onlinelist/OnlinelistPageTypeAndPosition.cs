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
        [Description("首页")]
        Homepage = 0,
        [Description("一级类页面")]
        C1page = 1,
        [Description("二级类页面")]
        C2page = 2,
        [Description("三级类页面")]
        C3page = 3,
        [Description("专卖店")]
        Brand = 4,
        [Description("购物车页面")]
        Shopcart = 5
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
        [Description("大类--清库产品")]
        UnsalableItem = 22,
        [Description("大类--新品上架")]
        New = 201,
        [Description("大类--热销产品")]
        Hot = 200,
        [Description("大类--特价商品")]
        SpecialItem = 202
    }

    [Serializable]
    public enum Category2 : int
    {
        [Description("中类--特别推荐")]
        SpecialOfferToday = 205
    }
    [Serializable]
    public enum Category3 : int
    {
        [Description("小类--热销推荐")]
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
}
