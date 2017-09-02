using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Facade.Keyword;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.SolrSearch;
using Nesoft.Utility.DataAccess.SearchEngine;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.Utility;
using Nesoft.ECWeb.Entity.SearchEngine;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.Facade.SearchEngine;

namespace Nesoft.ECWeb.MobileService.Models.Search
{
    public class SearchManager
    {
        /// <summary>
        /// 获取热门搜索关键字
        /// </summary>
        /// <returns></returns>
        public List<HotSearchKeywordModel> GetHotKeywords(int pageType = 0, int pageID = 0)
        {
            var entityList = KeywordFacade.GetHotSearchKeyword(pageType, pageID);
            List<HotSearchKeywordModel> modelList = new List<HotSearchKeywordModel>();
            foreach (var entity in entityList)
            {
                HotSearchKeywordModel model = new HotSearchKeywordModel();
                model.SysNo = entity.SysNo;
                model.Keyword = entity.Keyword;
                modelList.Add(model);
            }

            return modelList;
        }

        /// <summary>
        /// 搜索商品
        /// </summary>
        /// <param name="criteria">搜索条件</param>
        /// <returns></returns>
        public SearchResultModel Search(SearchCriteriaModel criteria)
        {
            var searchCondition = MapSearchCondition(criteria);
            var searchResult = ProductSearchFacade.GetProductSearchResultBySolr(searchCondition);

            SearchResultModel model = new SearchResultModel();
            model.ProductListItems = TransformResultItemList(searchResult.ProductDataList);
            model.PageInfo = MapPageInfo(searchResult.ProductDataList);
            model.PageInfo.PageIndex = criteria.PageIndex;
            model.Filters = MapSearchFilter(searchResult.Navigation);

            return model;
        }

        //构造搜索引擎相关参数
        private ProductSearchCondition MapSearchCondition(SearchCriteriaModel criteria)
        {
            ProductSearchCondition condition = new ProductSearchCondition();
            condition.Filters = new List<FilterBase>();
            condition.Filters.Add(new FieldFilter("p_productstatus", "1"));
            condition.NValueList = ProductSearchFacade.GetNValueList(criteria.FilterValue);

            //使用关键字搜索
            string keyWord = criteria.Keywords;
            if (string.IsNullOrEmpty(keyWord))
            {
                keyWord = criteria.Barcode;
            }
            if (!string.IsNullOrEmpty(keyWord))
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

            //使用CategoryID搜索
            if (criteria.CategoryID > 0)
            {
                //将商品分类ID转换成NValue格式
                var subCatNValue = ProductSearchFacade.GetSubcategoryDimensionValues(criteria.CategoryID.Value);
                condition.NValueList.Add(subCatNValue.ToString());
                condition.Filters.Add(new FieldFilter("Nvalue", subCatNValue.ToString()));
            }

            //使用品牌ID搜索
            if (criteria.BrandID > 0)
            {
                //将品牌ID转换成NValue格式
                var brandNValue = ProductSearchFacade.GetBrandNValue(criteria.BrandID.Value);
                condition.NValueList.Add(brandNValue.ToString());
                condition.Filters.Add(new FieldFilter("Nvalue", brandNValue.ToString()));
            }

            //分页
            if (criteria.PageIndex <= 0)
            {
                criteria.PageIndex = 1;
            }
            if (criteria.PageSize <= 0)
            {
                criteria.PageSize = 10;
            }
            condition.PagingInfo = new Nesoft.Utility.DataAccess.SearchEngine.PagingInfo();
            condition.PagingInfo.PageNumber = criteria.PageIndex;
            condition.PagingInfo.PageSize = criteria.PageSize;

            //排序
            if (criteria.SortValue <= 0)
            {
                criteria.SortValue = 10;
            }
            List<SortItem> sortItems = new List<SortItem>();
            sortItems.Add(SortKeyValueMappingConfig.SortItemList.Find(f => f.Key == criteria.SortValue).Item);
            condition.SortItems = sortItems;

            return condition;
        }

        private PageInfo MapPageInfo(PagedResult<ProductSearchResultItem> solrItemList)
        {
            var pageInfo = new PageInfo();
            pageInfo.PageIndex = solrItemList.PageNumber;
            pageInfo.PageSize = solrItemList.PageSize;
            pageInfo.TotalCount = solrItemList.TotalRecords;

            return pageInfo;
        }

        private List<SearchFilterModel> MapSearchFilter(NavigationContainer navigation)
        {
            List<SearchFilterModel> result = new List<SearchFilterModel>();
            if (navigation != null && navigation.NavigationItems != null)
            {
                foreach (var navItem in navigation.NavigationItems)
                {
                    //如果没有筛选项或数量过少就丢弃
                    if (navItem.SubNavigationItems == null
                            || navItem.SubNavigationItems.Count == 0)
                    {
                        continue;
                    }

                    SearchFilterModel filterModel = new SearchFilterModel();
                    filterModel.ID = (int)navItem.ItemType;
                    filterModel.Name = navItem.Name;

                    foreach (var subItem in navItem.SubNavigationItems)
                    {
                        SearchFilterItemModel itemModel = new SearchFilterItemModel();
                        itemModel.EnId = subItem.Value;
                        itemModel.Name = subItem.Name;
                        itemModel.ProductCount = subItem.NumberOfItem;
                        filterModel.Items.Add(itemModel);
                    }

                    result.Add(filterModel);
                }
            }

            return result;
        }

        private List<ProductItemModel> TransformResultItemList(PagedResult<ProductSearchResultItem> solrItemList)
        {
            List<ProductItemModel> result = new List<ProductItemModel>();
            ImageSize imageSizeItemList = ImageUrlHelper.GetImageSize(ImageType.Middle);
            foreach (var item in solrItemList)
            {
                ProductItemModel model = new ProductItemModel();
                //基本信息
                model.ID = item.ProductSysNo;
                model.Code = item.ProductID;
                model.ProductTitle = item.ProductDisplayName;
                model.PromotionTitle = item.PromotionTitle;
                model.ImageUrl = ProductFacade.BuildProductImage(imageSizeItemList, item.ProductDefaultImage);

                //价格相关信息
                SalesInfoModel salesInfo = new SalesInfoModel();
                salesInfo.BasicPrice = item.MarketPrice;
                salesInfo.CurrentPrice = item.SalesPrice;
                salesInfo.CashRebate = item.CashRebate;
                salesInfo.TariffPrice = item.ProductTariffAmt;
                salesInfo.FreeEntryTax = item.ProductTariffAmt <= ConstValue.TariffFreeLimit;
                salesInfo.TotalPrice = item.TotalPrice;
                //赠送积分数量
                salesInfo.PresentPoint = item.Point;
                salesInfo.OnlineQty = item.OnlineQty;
                salesInfo.IsHaveValidGift = item.IsHaveValidGift;
                salesInfo.IsCountDown = item.IsCountDown;
                salesInfo.IsNewProduct = item.IsNewproduct;
                salesInfo.IsGroupBuyProduct = item.IsGroupBuyProduct;
                model.SalesInfo = salesInfo;

                //其它信息
                model.ReviewCount = item.ReviewCount;
                model.ReviewScore = item.ReviewScore;

                result.Add(model);
            }

            return result;
        }
    }
}