using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.Store;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Entity.Store;
using ECommerce.Entity.Store.Filter;
using ECommerce.Entity.Store;
using ECommerce.Entity.Store.Vendor;

namespace ECommerce.Facade.Store
{
    public class StoreFacade
    {
        public static QueryResult<StoreNavigation> QueryStoreNavigationList(StorePageListQueryFilter filter, int SellSysno)
        {
            var result = StoreDA.QueryStoreNavigationList(filter, SellSysno);

            //insert home to first
            var home = QueryHomePage(SellSysno);
            if (home != null)
            {
                var navHome = new StoreNavigation();
                navHome.LinkUrl = string.Format("/Store/{0}/{1}", SellSysno, home.SysNo.Value);
                navHome.Content = "首页";
                result.ResultList.Insert(0, navHome);
            }

            return result;
        }
        /// <summary>
        /// 获得店铺的页面,如果是预览则查询ECStore..StorePageInfo,否则查询ECStore..[PublishedStorePageInfo]
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static StorePage QueryStorePage(StorePageFilter filter)
        {
            return StoreDA.QueryStorePage(filter);
        }

        public static StorePage QueryHomePage(int sellerSysNo)
        {
            var filter = new StorePageFilter();
            filter.PageType = "Home";
            filter.SellerSysNo = sellerSysNo;
            return StoreDA.QueryStorePage(filter);
        }

        public static StorePage QueryProductListPage(int sellerSysNo)
        {
            var filter = new StorePageFilter();
            filter.PageType = "ProductList";
            return StoreDA.QueryStorePage(filter);
        }

        public static StorePage QuerySearchResultPage(int sellerSysNo)
        {
            var filter = new StorePageFilter();
            filter.PageType = "SearchResult";
            return StoreDA.QueryStorePage(filter);
        }

        public static StoreBasicInfo QueryStoreBasicInfo(int sellerSysNo)
        {
            return StoreDA.QueryStoreBasicInfo(sellerSysNo);
        }

        /// <summary>
        /// 获得店铺的页头信息
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static string QueryStorePageHeader(int sellerSysNo)
        {
            return StoreDA.QueryStorePageHeader(sellerSysNo);
        }

        public static bool IsCurrentForNavigation(string url, int pageSysNo)
        {
            var str = url.Split(new[] { '/' }).LastOrDefault();
            var index = str.IndexOf("?");
            int sysNo = 0;
            if (index > 0)
                int.TryParse(str.Substring(0, index), out sysNo);
            else
                int.TryParse(str, out sysNo);

            return sysNo == pageSysNo;
        }

        /// <summary>
        /// 新品上市
        /// </summary>
        /// <param name="MerchantSysNo"></param>
        /// <param name="C1SysNo"></param>
        /// <param name="count"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryStoreNewRecommendProduct(
            int MerchantSysNo,
            string CategoryCode,
            int count,
            string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryStoreNewRecommendProduct", languageCode, companyCode, MerchantSysNo.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            List<RecommendProduct> result = StoreDA.QueryStoreNewRecommendProduct(MerchantSysNo, CategoryCode, count, languageCode, companyCode);

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }


        /// <summary>
        /// 一周排行
        /// </summary>
        /// <param name="MerchantSysNo"></param>
        /// <param name="CategoryCode"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryWeekRankingForCategoryCode(
            int MerchantSysNo,
            string CategoryCode,
            int count,
            string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryWeekRankingForCategoryCode", CategoryCode, languageCode, companyCode, MerchantSysNo.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }
            var p1 = StoreDA.QueryWeekRankingForCategoryCode(MerchantSysNo, CategoryCode);
            if (p1.Count < count)
            {
                var p2 = StoreDA.QuerySuperSpecialProductForCategoryCode(MerchantSysNo, CategoryCode, languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });

            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);
            return result;
        }

        /// <summary>
        /// 根据二级域名查找对应的商家首页
        /// </summary>
        /// <param name="subdomain"></param>
        /// <returns></returns>
        public static StoreDomainPage GetStoreIndexPageBySubDomain(string subdomain)
        {
            List<StoreDomainPage> list = GetAllStoreDomainHomePageList();
            if (list == null || list.Count == 0)
            {
                return null;
            }
            StoreDomainPage result = list.Find(f => f.SecondDomain.Trim().ToLower() == subdomain.Trim().ToLower());
            return result;
        }

        /// <summary>
        /// 所有的有效的二级域名商家
        /// </summary>
        /// <returns></returns>
        public static List<StoreDomainPage> GetAllStoreDomainHomePageList()
        {
            string cacheKey = CommonFacade.GenerateKey("GetAllStoreDomainHomePageList");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<StoreDomainPage>)HttpRuntime.Cache[cacheKey];
            }
            List<StoreDomainPage> result = StoreDA.GetAllStoreDomainHomePageList();
            if (result != null)
            {
                HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }
            return result;
        }
        /// <summary>
        /// 检查商家是否存在
        /// </summary>
        /// <param name="vendorName"></param>
        /// <returns></returns>
        public static bool CheckExistsVendor(string vendorName)
        {
            return StoreDA.CheckExistsVendor(vendorName);
        }
        /// <summary>
        /// 创建商家
        /// </summary>
        /// <param name="vendorBasicInfo">商家基本信息</param>
        public static void CreateVendor(VendorBasicInfo vendorBasicInfo)
        {
            StoreDA.CreateVendor(vendorBasicInfo);
        }
    }
}
