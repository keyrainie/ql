using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Entity.SearchEngine;
using ECommerce.Entity.SolrSearch;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess.SearchEngine;
using ECommerce.Utility.DataAccess.SearchEngine.Solr;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace ECommerce.Facade.SearchEngine
{
    public class ProductSolrSearcher : SolrSearcher<ProductSearchRecord, ProductSearchResult>
    {
        protected override string SolrCoreName
        {
            get { return "Core.ProductSearch.ReplicateData"; }
        }

        protected override QueryOptions BuildQueryOptions(SearchCondition condition)
        {
            QueryOptions queryOptions = base.BuildQueryOptions(condition);

            #region 分组(自定义属性)

            List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();

            ProductSearchCondition pCondition = condition as ProductSearchCondition;
            if (pCondition != null && !string.IsNullOrWhiteSpace(pCondition.WithInKeyWord))
            {
                keyValuePairList.Add(new KeyValuePair<string, string>("mm", "2<75% 5<-1"));
            }

            keyValuePairList.Add(new KeyValuePair<string, string>("group", "true"));
            keyValuePairList.Add(new KeyValuePair<string, string>("group.main", "false"));
            keyValuePairList.Add(new KeyValuePair<string, string>("group.ngroups", "true"));

            if (condition.IsGroupQuery == true)
            {
                keyValuePairList.Add(new KeyValuePair<string, string>("group.sort", "p_pricesort asc"));
                keyValuePairList.Add(new KeyValuePair<string, string>("group.field", "p_productgroupvalue"));
            }
            else
            {
                keyValuePairList.Add(new KeyValuePair<string, string>("group.field", "p_sysno"));
            }

            queryOptions.ExtraParams = keyValuePairList.ToArray();

            #endregion

            return queryOptions;
        }

        protected override ProductSearchResult TransformSolrQueryResult(SolrQueryResults<ProductSearchRecord> solrQueryResult, SearchCondition condition)
        {
            ProductSearchResult result = new ProductSearchResult();
            int totalGroupItemCount;

            #region 获取产品列表
            List<ProductSearchResultItem> productList = GetGroupItemList(solrQueryResult, out totalGroupItemCount);
            result.NumberOfProduct = totalGroupItemCount;
            result.ProductDataList = new PagedResult<ProductSearchResultItem>(totalGroupItemCount, condition.PagingInfo.PageSize, condition.PagingInfo.PageNumber, productList);
            #endregion

            #region 获取筛选条件
            result.Navigation = GetNavigation(solrQueryResult, (ProductSearchCondition)condition);
            #endregion

            return result;
        }

        private NavigationContainer GetNavigation(SolrQueryResults<ProductSearchRecord> solrQueryResult, ProductSearchCondition condition)
        {
            var brandFacets = solrQueryResult.FacetFields["p_brandname_facet"];
            var originFacets = solrQueryResult.FacetFields["p_originName_facet"];
            var propertyValueFacets = solrQueryResult.FacetFields["p_propertyvalue_facet"];
            var categoryName3Facets = solrQueryResult.FacetFields["p_categoryname3_facet"];
            var priceFacet = solrQueryResult.FacetFields["p_price_n"];
            var storeCategoryFacets = solrQueryResult.FacetFields["p_storecategory_facet"];

            NavigationContainer NaviRet = new NavigationContainer();
            NaviRet.NavigationItems = new List<NavigationItem>();

            //当前页面N值
            string NValue = string.Empty;
            if (condition.NValueList != null && condition.NValueList.Count > 0)
            {
                foreach (string str in condition.NValueList)
                {
                    NValue += str + ' ';
                }
            }

            #region 分类
            KeyValuePair<NavigationItemType, string> NavSubcategory = new KeyValuePair<NavigationItemType, string>(NavigationItemType.SubCategory, "平台类别");
            NavigationItem NavSubcategoryItem = new NavigationItem();
            NavSubcategoryItem.SubNavigationItems = new List<NavigationItem>();
            foreach (var facet in categoryName3Facets)
            {
                int num = facet.Value;

                string[] facetItemArray = facet.Key.Split(new string[] { "@!@" }, StringSplitOptions.RemoveEmptyEntries);

                bool isExists = NavSubcategoryItem.SubNavigationItems.Exists(delegate(NavigationItem eachItem)
                {
                    return eachItem.Value == facetItemArray[1].ToString();
                });
                if (isExists)
                {
                    continue;
                }

                NavSubcategoryItem.ItemType = NavSubcategory.Key;
                NavSubcategoryItem.Name = NavSubcategory.Value;


                NavigationItem subNavItem = new NavigationItem();
                subNavItem.Name = facetItemArray[0].ToString();
                subNavItem.NumberOfItem = num;
                subNavItem.Value = facetItemArray[1].ToString();//(NValue + facetItemArray[1].ToString()).Trim();

                subNavItem.ItemType = NavigationItemType.SubCategory;
                NavSubcategoryItem.SubNavigationItems.Add(subNavItem);
            }
            NaviRet.NavigationItems.Add(NavSubcategoryItem);
            #endregion

            #region 店铺分类
            NavigationItem NavStoreCateItem = new NavigationItem();
            NavStoreCateItem.SubNavigationItems = new List<NavigationItem>();
            KeyValuePair<NavigationItemType, string> NavStoreCate = new KeyValuePair<NavigationItemType, string>(NavigationItemType.StoreCategory, "店铺类别");
            foreach (var facet in storeCategoryFacets)
            {
                int num = facet.Value;

                string[] facetItemArray = facet.Key.Split(new string[] { "@!@" }, StringSplitOptions.RemoveEmptyEntries);

                bool isExists = NavStoreCateItem.SubNavigationItems.Exists(delegate(NavigationItem eachItem)
                {
                    return eachItem.Value == facetItemArray[1].ToString();
                });
                if (isExists)
                {
                    continue;
                }

                NavStoreCateItem.ItemType = NavStoreCate.Key;
                NavStoreCateItem.Name = NavStoreCate.Value;


                NavigationItem subNavItem = new NavigationItem();
                subNavItem.Name = facetItemArray[0].ToString();
                subNavItem.NumberOfItem = num;
                subNavItem.Value = facetItemArray[1].ToString();//(NValue + facetItemArray[1].ToString()).Trim();
                subNavItem.ItemType = NavigationItemType.StoreCategory;
                NavStoreCateItem.SubNavigationItems.Add(subNavItem);
            }
            NaviRet.NavigationItems.Add(NavStoreCateItem);
            #endregion

            #region 品牌
            NavigationItem NavBrandItem = new NavigationItem();
            NavBrandItem.SubNavigationItems = new List<NavigationItem>();
            KeyValuePair<NavigationItemType, string> NavBrand = new KeyValuePair<NavigationItemType, string>(NavigationItemType.Brand, "品牌");
            foreach (var facet in brandFacets)
            {
                int num = facet.Value;

                string[] facetItemArray = facet.Key.Split(new string[] { "@!@" }, StringSplitOptions.RemoveEmptyEntries);

                bool isExists = NavBrandItem.SubNavigationItems.Exists(delegate(NavigationItem eachItem)
                {
                    return eachItem.Value == facetItemArray[1].ToString();
                });
                if (isExists)
                {
                    continue;
                }

                NavBrandItem.ItemType = NavBrand.Key;
                NavBrandItem.Name = NavBrand.Value;


                NavigationItem subNavItem = new NavigationItem();
                subNavItem.Name = facetItemArray[0].ToString();
                subNavItem.NumberOfItem = num;
                subNavItem.Value = facetItemArray[1].ToString();//(NValue + facetItemArray[1].ToString()).Trim();
                subNavItem.ItemType = NavigationItemType.Brand;
                NavBrandItem.SubNavigationItems.Add(subNavItem);
            }
            NaviRet.NavigationItems.Add(NavBrandItem);
            #endregion

            #region 价格
            NavigationItem NavPriceItem = new NavigationItem();
            NavPriceItem.SubNavigationItems = new List<NavigationItem>();
            KeyValuePair<NavigationItemType, string> NavPrice = new KeyValuePair<NavigationItemType, string>(NavigationItemType.Price, "价格");
            foreach (var facet in priceFacet)
            {
                int num = facet.Value;

                string facetItem = facet.Key;

                bool isExists = NavPriceItem.SubNavigationItems.Exists(delegate(NavigationItem eachItem)
                {
                    return eachItem.Value == facetItem;
                });
                if (isExists)
                {
                    continue;
                }

                NavPriceItem.ItemType = NavPrice.Key;
                NavPriceItem.Name = NavPrice.Value;

                NavigationItem subNavItem = new NavigationItem();

                PriceRange priceRangeInfo = ProductPriceRangeConfig.PriceRangeList.Find(delegate(PriceRange eachPriceRange) { return eachPriceRange.Key == facetItem; });

                if (priceRangeInfo != null)
                {
                    subNavItem.Name = priceRangeInfo.Text;
                }

                subNavItem.NumberOfItem = num;
                subNavItem.Value = facetItem.TrimEnd();//(NValue + facetItem.TrimEnd()).Trim();
                subNavItem.ItemType = NavigationItemType.Price;
                NavPriceItem.SubNavigationItems.Add(subNavItem);
            }
            //价格筛选条件按照价格升序
            NavPriceItem.SubNavigationItems.Sort(delegate(NavigationItem a, NavigationItem b)
            {
                return a.Value.CompareTo(b.Value);
            });
            NaviRet.NavigationItems.Add(NavPriceItem);
            #endregion

            #region 产地
            NavigationItem NavOriginItem = new NavigationItem();
            NavOriginItem.SubNavigationItems = new List<NavigationItem>();
            KeyValuePair<NavigationItemType, string> NavOrigin = new KeyValuePair<NavigationItemType, string>(NavigationItemType.Origin, "产地");
            foreach (var facet in originFacets)
            {
                int num = facet.Value;

                string[] facetItemArray = facet.Key.Split(new string[] { "@!@" }, StringSplitOptions.RemoveEmptyEntries);

                bool isExists = NavOriginItem.SubNavigationItems.Exists(delegate(NavigationItem eachItem)
                {
                    return eachItem.Value == facetItemArray[1].ToString();
                });
                if (isExists)
                {
                    continue;
                }

                NavOriginItem.ItemType = NavOrigin.Key;
                NavOriginItem.Name = NavOrigin.Value;


                NavigationItem subNavItem = new NavigationItem();
                subNavItem.Name = facetItemArray[0].ToString();
                subNavItem.NumberOfItem = num;
                subNavItem.Value = facetItemArray[1].ToString();//(NValue + facetItemArray[1].ToString()).Trim();
                subNavItem.ItemType = NavigationItemType.Origin;
                NavOriginItem.SubNavigationItems.Add(subNavItem);
            }
            NaviRet.NavigationItems.Add(NavOriginItem);
            #endregion

            #region 产品属性
            List<NavigationItem> tempNavList = new List<NavigationItem>();
            foreach (var facet in propertyValueFacets)
            {
                int num = facet.Value;

                string[] facetItemArray = facet.Key.Split(new string[] { "@!@" }, StringSplitOptions.RemoveEmptyEntries);

                NavigationItem NavItem = new NavigationItem();
                NavItem.Name = Utils.removePrefix(facetItemArray[0]);
                NavItem.Priority = Convert.ToInt32(facetItemArray[4]);
                NavItem.ItemType = NavigationItemType.Attribute;
                NavItem.SubNavigationItems = new List<NavigationItem>();

                NavigationItem subNavItem = new NavigationItem();
                subNavItem.Name = facetItemArray[1];
                subNavItem.NumberOfItem = num;
                subNavItem.Value = facetItemArray[2];//(NValue + facetItemArray[2]).Trim();
                subNavItem.Priority = Convert.ToInt32(facetItemArray[5]);

                NavigationItem tempNavItem = tempNavList.Find(delegate(NavigationItem eachItem) { return eachItem.Name == NavItem.Name; });
                if (tempNavItem == null)
                {
                    NavItem.SubNavigationItems.Add(subNavItem);
                    tempNavList.Add(NavItem);
                }
                else
                {
                    var tempSubNavItem = tempNavItem.SubNavigationItems.Find(delegate(NavigationItem eachItem)
                    {
                        return eachItem.Value == subNavItem.Value;
                    });
                    if (tempSubNavItem != null)
                    {
                        tempSubNavItem.NumberOfItem += num;
                        continue;
                    }
                    tempNavItem.SubNavigationItems.Add(subNavItem);

                }
            }
            tempNavList.Sort(delegate(NavigationItem a, NavigationItem b)
            {
                return a.Priority.CompareTo(b.Priority);
            });
            foreach (NavigationItem eachNavItem in tempNavList)
            {
                eachNavItem.Name = Utils.GetEndSplitName(eachNavItem.Name);
                eachNavItem.SubNavigationItems.Sort(delegate(NavigationItem a, NavigationItem b)
                {
                    int tmp = a.Priority.CompareTo(b.Priority);
                    return tmp == 0 ? b.NumberOfItem.CompareTo(a.NumberOfItem) : tmp;
                });

                NaviRet.NavigationItems.Add(eachNavItem);
            }
            #endregion

            return NaviRet;
        }

        private List<ProductSearchResultItem> GetGroupItemList(SolrQueryResults<ProductSearchRecord> solrQueryResult, out int totalGroupItemCount)
        {
            List<ProductSearchResultItem> productList = new List<ProductSearchResultItem>();
            GroupedResults<ProductSearchRecord> grouInfo = null;
            if (solrQueryResult.Grouping.Keys.Contains("p_productgroupvalue"))
            {
                grouInfo = solrQueryResult.Grouping["p_productgroupvalue"];
            }
            else if (solrQueryResult.Grouping.Keys.Contains("p_sysno"))
            {
                grouInfo = solrQueryResult.Grouping["p_sysno"];
            }

            if (grouInfo == null)
            {
                totalGroupItemCount = 0;
                return productList;
            }

            totalGroupItemCount = grouInfo.Ngroups.GetValueOrDefault();
            foreach (Group<ProductSearchRecord> group in grouInfo.Groups)
            {
                productList.Add(ReadProductFromSolrDocument(((IList<ProductSearchRecord>)group.Documents)[0]));
            }

            return productList;
        }

        private ProductSearchResultItem ReadProductFromSolrDocument(ProductSearchRecord record)
        {
            ProductSearchResultItem productItem = new ProductSearchResultItem();

            productItem.CashRebate = Convert.ToDecimal(record.CashRebate);
            productItem.IsCountDown = record.CountDown == "countdown";
            productItem.IsHaveValidGift = record.IsHaveValidGift == "1";
            productItem.MarketPrice = Convert.ToDecimal(record.BasicPrice);
            productItem.MerchantBriefName = record.VendorBriefName;
            productItem.MerchantSysNo = record.MerchantSysNO;
            productItem.Point = record.Point;
            if (record.Categorysysno3Nvalue.TrimEnd().Contains(" "))
            {
                string[] Categorysysno3Nvalue = record.Categorysysno3Nvalue.TrimEnd().Split(' ');
                productItem.ProductCategoryID = Convert.ToInt32(Categorysysno3Nvalue[0]) - ConstValue.SINGLE_SUBCATEGORY_DMSID_SEED;
            }
            else
            {
                productItem.ProductCategoryID = Convert.ToInt32(record.Categorysysno3Nvalue) - ConstValue.SINGLE_SUBCATEGORY_DMSID_SEED;
            }
            productItem.ProductDefaultImage = record.DefaultImage;
            productItem.ProductGroupName = record.ProductGroupName;
            productItem.ProductSysNo = Convert.ToInt32(record.Sysno);
            productItem.ProductID = record.ProductID;
            productItem.ProductName = record.ProductTitle;
            productItem.ProductShortDescription = record.ProductDescription;
            productItem.ProductTariffAmt = Convert.ToDecimal(record.ProductTariffAmt);
            productItem.PromotionTitle = record.PromotionTitle;
            productItem.SalesPrice = Convert.ToDecimal(record.CurrentPrice);
            productItem.OnlineQty = record.OnlineQty;
            //productItem.ProductTariffAmtWithRebate = Convert.ToDecimal(record.ProductTariffAmtWithRebate);
            productItem.TotalPrice = Convert.ToDecimal(record.TotalPrice);
            productItem.IsNewproduct = record.NewProduct == "newproduct";
            productItem.IsGroupBuyProduct = record.ProductGroupBuyingSysno > 0;

            productItem.PolymericProductCount = Convert.ToInt32(record.PolymericProductCount);
            if (productItem.PolymericProductCount > 1)
            {
                StringBuilder sbItemName = new StringBuilder();
                sbItemName.Append(productItem.ProductGroupName);
                if (record.IsPolymeric1 == "N")
                {
                    sbItemName.Append(" ");
                    sbItemName.Append(record.ValueDescription1);
                }
                if (record.IsPolymeric2 == "N")
                {
                    sbItemName.Append(" ");
                    sbItemName.Append(record.ValueDescription2);
                }
                //聚合商品商品名称
                productItem.ProductDisplayName = sbItemName.ToString();
            }
            else
            {
                productItem.ProductDisplayName = productItem.ProductName;
            }

            productItem.ReviewScore = Convert.ToDecimal(record.ReviewScore);
            productItem.ReviewCount = Convert.ToInt32(record.ReviewCount);
            productItem.Discount = Convert.ToDecimal(record.Discount);
            productItem.ProductTradeType = Utils.GetEnumByValue<TradeType>(record.ProductTradeType.ToString(), TradeType.DirectMail);
            productItem.Status = Utils.GetEnumByValue<ProductStatus>(record.ProductStatus, ProductStatus.OnlyShow);

            return productItem;
        }


    }
}
