using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using System.Web;
using ECommerce.DataAccess.Keyword;
using System.Web.Caching;

namespace ECommerce.Facade.Keyword
{
    public class KeywordFacade
    {
        public static SEOInfo GetSEO(int pageTypeID, int pageID)
        {
            string cacheKey = "GetAllSEOInfoList";
            List<SEOInfo> list = new List<SEOInfo>();
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                list = (List<SEOInfo>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                list = KeywordDA.GetAllSEOInfoList();
                if(list==null)
                {
                    list=new List<SEOInfo>();
                }
                HttpRuntime.Cache.Insert(cacheKey, list, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);
            }
            SEOInfo seo = list.Find(f => f.PageType == pageTypeID && f.PageID == pageID);
            return seo;
            
        }


        public static List<HotSearchKeyword> GetHotSearchKeyword(int pageType, int pageID)
        {
            string cacheKey = String.Format("GetHotSearchKeyword_{0}_{1}", pageType, pageID);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<HotSearchKeyword>)HttpRuntime.Cache[cacheKey];
            }

            List<HotSearchKeyword> keywordList = KeywordDA.GetHotSearchKeyword(pageType, pageID);    //获取关键字信息

            HttpRuntime.Cache.Insert(cacheKey, keywordList, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);
            return keywordList;
        }

        /// <summary>
        /// 获取搜索框中默认的搜索关键字
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultSearchKeyword(int pageType, int pageID, string languageCode = "zh-CN", string companyCode = "8601", string storeCompanyCode = "8601")
        {
            return KeywordDA.GetDefaultSearchKeyword(pageType,pageID,languageCode,companyCode,storeCompanyCode);
        }
    }
}
