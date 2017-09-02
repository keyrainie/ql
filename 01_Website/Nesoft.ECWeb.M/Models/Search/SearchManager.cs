using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.SolrSearch;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Facade.SearchEngine;
using Nesoft.Utility.DataAccess.SearchEngine;

namespace Nesoft.ECWeb.M.Models.Search
{
    internal class SearchManager
    {
        internal static ProductSearchResultVM Search(NameValueCollection pageQueryString)
        {
            return Search(new SearchCriteriaModel(), pageQueryString);
        }

        internal static ProductSearchResultVM Search(SearchCriteriaModel criteria, NameValueCollection pageQueryString)
        {
            var searchCondition = GetSearchCondition(criteria, pageQueryString);
            var searchResult = ProductSearchFacade.GetProductSearchResultBySolr(searchCondition);

            ProductSearchResultVM resutlVM = new ProductSearchResultVM();
            if (pageQueryString != null)
            {
                resutlVM.SortKey = pageQueryString["sort"];
            }
            if (searchResult != null)
            {
                resutlVM.ProductList = searchResult.ProductDataList;
            }
            return resutlVM;
        }

        //构造搜索引擎相关参数
        private static ProductSearchCondition GetSearchCondition(SearchCriteriaModel criteria, NameValueCollection pageQueryString)
        {
            ProductSearchCondition condition = new ProductSearchCondition();
            condition.Filters = new List<FilterBase>();
            condition.NValueList = new List<string>();

            //使用Category3ID搜索
            if (criteria.Category3ID > 0)
            {
                //将商品分类ID转换成NValue格式
                var subCatNValue = ProductSearchFacade.GetSubcategoryDimensionValues(criteria.Category3ID.Value);
                condition.NValueList.Add(subCatNValue.ToString());
            }

            //使用品牌ID搜索
            if (criteria.BrandID > 0)
            {
                //将品牌ID转换成NValue格式
                var brandNValue = ProductSearchFacade.GetBrandNValue(criteria.BrandID.Value);
                condition.NValueList.Add(brandNValue.ToString());
            }

            //使用Category1ID搜索
            if (criteria.Category1ID > 0)
            {
                condition.Filters.Add(new FieldFilter("p_tabstoreids", criteria.Category1ID.ToString()));
            }

            string keyWord = string.Empty;
            int sortKey = 0, 
                pageIndex = criteria.PageIndex.GetValueOrDefault(), 
                pageSize = criteria.PageSize.GetValueOrDefault();

            if (pageQueryString != null)
            {
                keyWord = pageQueryString["keyword"];
                int.TryParse(pageQueryString["sort"], out sortKey);

                int temp;
                if (int.TryParse(pageQueryString["pageIndex"], out temp))
                {
                    pageIndex = temp;
                }
                if (int.TryParse(pageQueryString["pageSize"], out temp))
                {
                    pageSize = temp;
                }
            }

            //使用关键字搜索
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                //解决“- +”号报错的bug
                if (!string.IsNullOrEmpty(keyWord) && keyWord.StartsWith("-"))
                {
                    keyWord = keyWord.Replace("-", "－");
                }
                if (!string.IsNullOrEmpty(keyWord) && keyWord.StartsWith("+"))
                {
                    keyWord = keyWord.Replace("+", "＋");
                }
                condition.KeyWord = keyWord.Trim();
            }

            //分页
            if (pageIndex <= 0)
            {
                pageIndex = 0;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }
            condition.PagingInfo = new Nesoft.Utility.DataAccess.SearchEngine.PagingInfo();
            condition.PagingInfo.PageNumber = (++pageIndex);
            condition.PagingInfo.PageSize = pageSize;

            //排序
            if (sortKey <= 0)
            {
                sortKey = 10;
            }
            List<SortItem> sortItems = new List<SortItem>();
            sortItems.Add(SortKeyValueMappingConfig.SortItemList.Find(f => f.Key == sortKey).Item);
            condition.SortItems = sortItems;

            return condition;
        }
    }
}