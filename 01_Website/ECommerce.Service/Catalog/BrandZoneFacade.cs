using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.Catalog;
using ECommerce.DataAccess.Product;
using ECommerce.Entity;
using ECommerce.Entity.Product;
using ECommerce.Entity.SolrSearch;
using ECommerce.Enums;
using ECommerce.Facade.Product;
using ECommerce.Facade.Recommend;
using ECommerce.Facade.SearchEngine;
using ECommerce.Utility.DataAccess.SearchEngine;

namespace ECommerce.Facade.Catalog
{
    /// <summary>
    /// 品牌专区facade
    /// </summary>
    public class BrandZoneFacade
    {
        /// <summary>
        /// 取得品牌专区数据
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static BrandZoneVM QueryBrandZoneVM(BrandZoneQueryVM queryInfo)
        {
            BrandZoneVM result = new BrandZoneVM() 
            { 
                BrandSysNo = queryInfo.BrandSysNo 
            };
            result.ProductSearchResult = GetProductSearchResult(queryInfo);
            result.BannerList = RecommendFacade.GetBannerInfoByPositionID(queryInfo.BrandSysNo, PageType.BrandZone, BannerPosition.BrandZone_TopRight);
            result.HotProductList = GetBrandHotProductList(queryInfo);
            result.BrandInfo = GetBrandInfo(queryInfo);

            return result;
        }

        /// <summary>
        /// 查询品牌下商品
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        private static ProductSearchResult GetProductSearchResult(BrandZoneQueryVM queryInfo)
        {
            ProductSearchCondition condition = new ProductSearchCondition()
            {
                IsGroupQuery = true,
                KeyWord = queryInfo.Keyword
            };

            //分页
            if (queryInfo.PageNumber <= 1)
            {
                queryInfo.PageNumber = 1;
            }
            condition.PagingInfo = new Utility.DataAccess.SearchEngine.PagingInfo()
            {
                PageNumber = queryInfo.PageNumber,
                PageSize = 15
            };
            //排序
            if (queryInfo.SortMode <= 0)
            {
                queryInfo.SortMode = 10;
            }
            SortEntity sortEntity = SortKeyValueMappingConfig.SortItemList.Find(f => f.Key == queryInfo.SortMode);
            if (sortEntity == null)
            {
                sortEntity = SortKeyValueMappingConfig.SortItemList[0];
            }
            condition.SortItems = new List<SortItem>() { sortEntity.Item };

            condition.NValueList = new List<string>();
            condition.Filters = new List<FilterBase>();
            if (queryInfo.BrandSysNo > 0)
            {
                condition.Filters.Add(new FieldFilter("p_brandid_n", (ConstValue.SINGLE_BRAND_STORE_DMSID_SEED + queryInfo.BrandSysNo).ToString()));
            }
            if (!String.IsNullOrWhiteSpace(queryInfo.SubCategoryEnID))
            {
                condition.NValueList.Add(queryInfo.SubCategoryEnID);
            }

            var result = ProductSearchFacade.GetProductSearchResultBySolr(condition);
            return result;
        }

        /// <summary>
        /// 获取品牌信息
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        private static BrandInfoExt GetBrandInfo(BrandZoneQueryVM queryInfo)
        {
            List<BrandInfoExt> allBrands = ProductFacade.GetAllBrands();
            if (allBrands != null && allBrands.Count > 0)
            {
                BrandInfoExt brandInfo = allBrands.Find(x => x.SysNo == queryInfo.BrandSysNo);
                return brandInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取品牌热销商品
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        private static List<ProductItemInfo> GetBrandHotProductList(BrandZoneQueryVM queryInfo)
        {
            List<ProductItemInfo> allHotProducts = GetAllHotProductList();
            if (allHotProducts != null && allHotProducts.Count > 0)
            {
                List<ProductItemInfo> hotProductList = allHotProducts.FindAll(x => x.BrandSysNo == queryInfo.BrandSysNo);
                return hotProductList;
            }
            return null;
        }

        /// <summary>
        /// 获取品牌专区所有热销商品
        /// </summary>
        /// <returns></returns>
        private static List<ProductItemInfo> GetAllHotProductList()
        {
            string cacheKey = CommonFacade.GenerateKey("GetHotProductList");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<ProductItemInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<ProductItemInfo> allHotProducts = BrandZoneDA.GetAllHotProductList();

            HttpRuntime.Cache.Insert(cacheKey, allHotProducts, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return allHotProducts;
        }
    }
}
