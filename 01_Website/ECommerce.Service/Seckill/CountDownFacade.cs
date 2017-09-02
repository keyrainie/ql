using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.Seckill;
using ECommerce.Entity;
using ECommerce.Entity.Seckill;

namespace ECommerce.Facade.Seckill
{
    public class CountDownFacade
    {
        /// <summary>
        /// 取得限时抢购列表
        /// </summary>
        /// <param name="pageIndex">当前页码：从1开始</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>限时抢购列表</returns>
        public static QueryResult<CountDownInfo> GetCountDownList(int pageIndex, int pageSize = 16)
        {
            QueryResult<CountDownInfo> result = new QueryResult<CountDownInfo>();

            List<CountDownInfo> list = GetAllCountDown();
            if (list == null || list.Count <= 0)
            {
                result.PageInfo = new PageInfo() { PageIndex = 1, PageSize = pageSize, TotalCount = 0 };
                result.ResultList = new List<CountDownInfo>(0);
                return result;
            }

            //活动排序
            list.Sort((c1, c2) =>
            {
                // 验证专区优先级
                int sort = c1.ShowPriority.CompareTo(c2.ShowPriority);
                if (sort == 0)
                {
                    // 验证限时抢购编号顺序
                    sort = -c1.CountDownSysNo.CompareTo(c2.CountDownSysNo);
                }
                return sort;
            });

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            if (list.Count > 0 && pageSize > 0)
            {
                int totalCount = list.Count % pageSize == 0 ? list.Count / pageSize : list.Count / pageSize + 1;
                if (pageIndex > totalCount)
                {
                    pageIndex = totalCount;
                }
            }
            else
            {
                pageIndex = 1;
            }

            result.PageInfo = new PageInfo() { PageIndex = pageIndex, PageSize = pageSize, TotalCount = list.Count };
            result.ResultList = list.Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList(); 

            return result;
        }

        /// <summary>
        /// 为首页的限时抢购推荐位提供数据
        /// </summary>
        /// <returns></returns>
        public static List<CountDownInfo> GetHomepageRecommendCountDown()
        {
            string cacheKey = CommonFacade.GenerateKey("GetHomepageRecommendCountDown");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<CountDownInfo>)HttpRuntime.Cache[cacheKey];
            }
            List<CountDownInfo> list = GetCountDownList(1, 5).ResultList;
            if (list == null)
            {
                list = new List<CountDownInfo>();
            }

            HttpRuntime.Cache.Insert(cacheKey, list, null, DateTime.Now.AddSeconds(CacheTime.Shortest), Cache.NoSlidingExpiration);
            return list;

        }

        private static List<CountDownInfo> GetAllCountDown()
        {
            string cacheKey = CommonFacade.GenerateKey("GetAllCountDown");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<CountDownInfo>)HttpRuntime.Cache[cacheKey];
            }
            List<CountDownInfo> countdownList = CountDownDA.GetAllCountDown(ConstValue.LimitBuyEarlyShowTimeSetting);

            HttpRuntime.Cache.Insert(cacheKey, countdownList, null, DateTime.Now.AddSeconds(CacheTime.Shortest), Cache.NoSlidingExpiration);

            return countdownList;
        }
    }
}
