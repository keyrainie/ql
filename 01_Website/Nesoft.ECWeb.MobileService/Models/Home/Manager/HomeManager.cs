using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Models.Banner;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.MobileService.Models.Promotion;
using Nesoft.ECWeb.Facade.Topic;
using Nesoft.ECWeb.Entity.Topic;
using Nesoft.Utility;
using System.Text.RegularExpressions;
using Nesoft.ECWeb.MobileService.AppCode;

namespace Nesoft.ECWeb.MobileService.Models.Home
{
    public class HomeManager
    {
        public HomeModel GetHomeInfo()
        {
            HomeModel homeModel = new HomeModel();
            //首页Banner
            BannerManager bannerManager = new BannerManager();
            homeModel.Banners = bannerManager.GetHomeBanners();
            //热卖商品列表
            RecommendManager recommandManager = new RecommendManager();
            homeModel.HotSaleProducts = recommandManager.GetHomeHotSaleItemList();

            //新品上市商品列表:
            homeModel.RecommendProducts = recommandManager.GetHomeRecommendProductList();

            //今日特卖商品列表:
            homeModel.TodayHotSaleProducts = recommandManager.GetTodayHotSaleItemlist();

            //首页楼层列表
            FloorManager floorManager = new FloorManager();
            homeModel.Floors = floorManager.GetHomeFloors();

            //精选品牌
            homeModel.Brands = recommandManager.GetRecommendBrands();

            //限时抢购
            QueryResult<CountDownItemModel> countDownList = new CountdownManager().GetCountDownList(0, 3);
            homeModel.CountDownList = countDownList.ResultList;

            return homeModel;
        }

        public static QueryResult<NewsInfoViewModel> GetNewsList(int pageIndex, int pageSize)
        {
            NewsQueryFilter queryFilter = new NewsQueryFilter() { PageInfo = new PageInfo() { PageIndex = pageIndex + 1, PageSize = pageSize }, NewsType = NewsType.HomePageNews };
            return EntityConverter<QueryResult<NewsInfo>, QueryResult<NewsInfoViewModel>>.Convert(TopicFacade.QueryNewsInfo(queryFilter), (s, t) =>
            {

                for (int i = 0; i < s.ResultList.Count; i++)
                {
                    t.ResultList[i].CreateTimeString = s.ResultList[i].CreateDate.ToString("yyyy-MM-dd");
                    t.ResultList[i].Title = StringUtility.RemoveHtmlTag(t.ResultList[i].Title);
                    string rawHtmlContent = t.ResultList[i].Content;
                    if (!string.IsNullOrEmpty(rawHtmlContent))
                    {
                        string plainTextContent = StringUtility.RemoveHtmlTag(rawHtmlContent).Replace("&nbsp;", "");
                        t.ResultList[i].Content = (plainTextContent.Length > 60 ? plainTextContent.Substring(0, 60) + "..." : plainTextContent);
                        t.ResultList[i].ImageUrl = ExtractImageSrc(rawHtmlContent);
                    }
                }
            });
        }

        public static NewsInfoViewModel GetNewsDetail(int sysNo)
        {
            var config = AppSettings.GetCachedConfig();
            return EntityConverter<NewsInfo, NewsInfoViewModel>.Convert(TopicFacade.GetNewsInfoBySysNo(sysNo), (s, t) =>
            {
                t.CreateTimeString = s.CreateDate.ToString("yyyy-MM-dd");
                t.Title = StringUtility.RemoveHtmlTag(t.Title);
                t.Content = config.NewsContentTemplate.Replace("${content}", s.Content);
            });

        }

        private static string ExtractImageSrc(string html)
        {
            if (html != null && html.Length > 0)
            {
                String regexImg = "<img.+?src\\s*=\\s*['|\"]?\\s*([^'\"\\s>]+).+?/?>?";
                Regex reg = new Regex(regexImg);
                Match match = reg.Match(html);
                if (match.Success && match.Groups.Count == 2)
                {
                    string imgUrl = match.Groups[1].Value;
                    return imgUrl;
                }
            }
            return "";
        }
    }
}
