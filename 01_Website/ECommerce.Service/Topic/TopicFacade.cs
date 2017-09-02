using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Topic;
using ECommerce.Utility;
using ECommerce.Entity.Topic;
using ECommerce.Entity;
using System.Web;
using ECommerce.Enums;
using System.Web.Caching;

namespace ECommerce.Facade.Topic
{
    public class TopicFacade
    {

        public static void InsertSubscriptionEmail(List<int> subscriptionCategorySysNos, string email)
        {
            if (subscriptionCategorySysNos == null || subscriptionCategorySysNos.Count == 0 || string.IsNullOrWhiteSpace(email))
            {
                throw new BusinessException("Please input subscription category id and email address!");
            }
            foreach (int no in subscriptionCategorySysNos)
            {
                Subscription entity = new Subscription()
                {
                    CompanyCode = ConstValue.CompanyCode,
                    Email = email,
                    SubscriptionCategorySysNo = no,
                    StoreCompanyCode = ConstValue.CompanyCode,
                    Status = "A",
                    LanguageCode = ConstValue.LanguageCode,
                    IPAddress = HttpContext.Current.Request.UserHostAddress
                };
                TopicDA.SetSubscription(entity);
            }
        }

        /// <summary>
        /// 获取首页新闻公告信息
        /// </summary>
        /// <returns></returns>
        public static List<NewsInfo> GetHomePageNews(NewsType newsType, int topNum = 5)
        {
            string cacheKey = CommonFacade.GenerateKey("GetHomePageNews", newsType.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<NewsInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<NewsInfo> entityList = TopicDA.GetNewsInfoByNewsType(newsType, topNum);

            HttpRuntime.Cache.Insert(cacheKey, entityList, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return entityList;
        }

        /// <summary>
        /// 获取指定数量，指定页面，指定位置的Topic
        /// </summary>
        /// <param name="newsType"></param>
        /// <param name="refSysNo"></param>
        /// <param name="topNum"></param>
        /// <param name="pageShowInheritance"></param>
        /// <returns></returns>
        public static List<NewsInfo> GetTopTopicList(NewsType newsType, int? refSysNo, int? pageShowInheritance, int topNum)
        {
            string cacheKey = CommonFacade.GenerateKey("GetTopTopicList", newsType.ToString(), refSysNo.HasValue ? refSysNo.ToString() : "N", pageShowInheritance.HasValue ? pageShowInheritance.ToString() : "N", topNum.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<NewsInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<NewsInfo> result = TopicDA.GetTopTopicList(newsType, refSysNo, pageShowInheritance, topNum);
            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);
            return result;
        }

        /// <summary>
        /// 分页查询新闻公告
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static QueryResult<NewsInfo> QueryNewsInfo(NewsQueryFilter filter)
        {
            return TopicDA.QueryNewsInfo(filter);
        }

        /// <summary>
        /// 根据编号获取新闻信息
        /// </summary>
        /// <param name="sysNo">新闻编号</param>
        /// <returns>新闻信息</returns>
        public static NewsInfo GetNewsInfoBySysNo(int sysNo)
        {
            return TopicDA.GetNewsInfoBySysNo(sysNo);
        }

        /// <summary>
        /// 获取帮助中心新闻类型
        /// </summary>
        /// <param name="newsType">要获取的帮助中心新闻类型</param>
        /// <returns>帮助中心新闻类型列表</returns>
        public static List<NewsInfo> GetHelperCenterCategory()
        {
            return TopicDA.GetHelperCenterCategory();
        }

        /// <summary>
        ///  获取指定数量帮助中心新闻
        /// </summary>
        /// <param name="categorySysNo">新闻类型</param>
        /// <param name="topNum">获取数量</param>
        /// <returns>帮助中心新闻</returns>
        public static List<NewsInfo> GetTopHelperCenterList(string categorySysNo, int topNum)
        {
            return TopicDA.GetTopHelperCenterList(categorySysNo, topNum);
        }

        /// <summary>
        ///  根据编号获取帮助中心新闻
        /// </summary>
        /// <param name="sysNo">新闻编号</param>
        /// <returns>帮助中心新闻</returns>
        public static NewsInfo GetTopHelperCenterBySysNo(int sysNo)
        {
            return TopicDA.GetTopHelperCenterBySysNo(sysNo);
        }
    }
}
