using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using ECommerce.Entity;
using ECommerce.Entity.SolrSearch;
using ECommerce.Facade.Product.Models;
using ECommerce.Facade.SearchEngine;
using ECommerce.Utility.DataAccess;
using ECommerce.Utility.DataAccess.SearchEngine;

namespace ECommerce.Facade.Product
{
    public class ProductSearchFacade
    {
        /// <summary>
        /// 搜索结果
        /// </summary>
        /// <param name="isSearchResultPage">是搜索结果页还是三级类别页(1为搜索结果页)</param>
        /// <returns></returns>
        public static ProductSearchResult GetProductSearchResult(SolrProductQueryVM vm)
        {
            //获取Condition
            ProductSearchCondition condition = GetSearchCondition(vm);
            if (vm.IsSearchResultPage == 1
                && string.IsNullOrEmpty(condition.KeyWord)
                && condition.NValueList == null
                && condition.NValueList.Count <= 0)
            {
                return new ProductSearchResult()
                {
                    Navigation = new NavigationContainer(),
                    ProductDataList = new Utility.PagedResult<Entity.SearchEngine.ProductSearchResultItem>()
                };
            }
            if (!string.IsNullOrWhiteSpace(vm.MidCategoryID) && Convert.ToInt32(vm.MidCategoryID) > 0)
            {
                condition.Filters.Add(new FieldFilter("p_categorysysno2", vm.MidCategoryID));
            }
            if (!string.IsNullOrWhiteSpace(vm.BrandID) && Convert.ToInt32(vm.BrandID) > 0)
            {
                condition.Filters.Add(new FieldFilter("p_brandid", vm.BrandID));
            }

            //获取当前页面Querystring
            queryStringCollection = HttpContext.Current.Request.QueryString;

            //获取搜索结果
            ProductSearchResult result = ProductSearchFacade.GetProductSearchResultBySolr(condition);
            return result;

        }

        #region  填充搜索条件
        /// <summary>
        /// View
        /// </summary>
        //private SearchPageView view;

        /// <summary>
        /// 搜索关键字
        /// </summary>
        private static string keyWord;


        /// <summary>
        /// 是否显示仅展示商品
        /// </summary>
        private static bool IsShowOnlyShowProducts = true;

        /// <summary>
        /// 小类ID
        /// </summary>
        protected static int subCategoryid;

        /// <summary>
        /// 小类Endeca ID
        /// </summary>
        private static int endecaId;

        /// <summary>
        /// N
        /// </summary>
        private static string strN;

        /// <summary>
        /// SearchWithIn 复选框Property
        /// </summary>
        private static int ep;

        /// <summary>
        /// 自定义价格开始
        /// </summary>
        private static string pf;

        /// <summary>
        /// 自定义价格截止
        /// </summary>
        private static string pt;

        /// <summary>
        /// 商品销售类型TradeType
        /// </summary>
        public static string tt;
        /// <summary>
        /// 储存运输类型
        /// </summary>
        public static string st;

        /// <summary>
        /// SearchWithin Keyword
        /// </summary>
        private static string withInKeyword;

        /// <summary>
        /// 第几页
        /// </summary>
        private static int pageNumber;

        /// <summary>
        /// 每页显示个数
        /// </summary>
        private static int pageSize;

        /// <summary>
        /// 产品总数
        /// </summary>
        private static int itemCount;

        /// <summary>
        /// Querystring
        /// </summary>
        private static NameValueCollection queryStringCollection;

        /// <summary>
        /// 排序方式
        /// </summary>
        private static int sortMode;

        /// <summary>
        /// 产品数
        /// </summary>
        protected static int totalItem;
        /// <summary>
        /// 投票总数
        /// </summary>
        protected static int totalPolls;

        /// <summary>
        /// Dim展开与收起CookieKey
        /// </summary>
        protected static string cookiekey;
        /// <summary>
        /// 获取Endeca Search Condition
        /// </summary>
        /// <returns></returns>
        public static ProductSearchCondition GetSearchCondition(SolrProductQueryVM vm)
        {

            ProductSearchCondition condition = new ProductSearchCondition();

            #region  keyword withInkeyword

            keyWord = HttpContext.Current.Request.QueryString["keyword"];
            //解决“- +”号报错的bug
            if (!string.IsNullOrEmpty(keyWord) && keyWord.StartsWith("-"))
            {
                keyWord = keyWord.Replace("-", "－");
            }
            if (!string.IsNullOrEmpty(keyWord) && keyWord.StartsWith("+"))
            {
                keyWord = keyWord.Replace("+", "＋");
            }
            withInKeyword = HttpContext.Current.Request.QueryString["withInKeyword"];
            keyWord = string.IsNullOrEmpty(keyWord) ? withInKeyword : (keyWord + " " + withInKeyword);
            condition.KeyWord = string.IsNullOrEmpty(keyWord) ? string.Empty : keyWord.Trim();
            condition.WithInKeyWord = withInKeyword;

            #endregion

            #region 分类以及过滤条件

            //N值
            strN = GetNavigationKey();
            condition.NFilter = strN;
            //将N值写入NvalueList
            condition.NValueList = GetNValueList(strN);

            int.TryParse(vm.SubCategoryID, out subCategoryid);
            int.TryParse(HttpContext.Current.Request.QueryString["enid"], out endecaId);
            endecaId = endecaId <= 0 ? GetSubcategoryDimensionValues(subCategoryid) : endecaId;
            if (endecaId > 0)
            {
                condition.NValueList.Add(endecaId.ToString());
                condition.EndecaId = endecaId.ToString();
            }
            #endregion

            condition.Filters = GetFilters();
            condition.FilterExpression = GetFilterExpression();

            #region  分页排序

            //分页信息
            int.TryParse(HttpContext.Current.Request.QueryString["page"], out pageNumber);
            int.TryParse(HttpContext.Current.Request.QueryString["pageSize"], out pageSize);
            condition.PagingInfo = new ECommerce.Utility.DataAccess.SearchEngine.PagingInfo();
            condition.PagingInfo.PageNumber = pageNumber == 0 ? 1 : pageNumber;
            condition.PagingInfo.PageSize = pageSize == 0 ? 24 : pageSize;
            //排序
            int.TryParse(HttpContext.Current.Request.QueryString["sort"], out sortMode);
            sortMode = sortMode <= 0 ? 10 : sortMode;


            if (!string.IsNullOrWhiteSpace(keyWord) && sortMode <= 10)
            {
                sortMode = 9999;
            }



            List<SortItem> sortItems = new List<SortItem>();
            sortItems.Add(SortKeyValueMappingConfig.SortItemList.Find(f => f.Key == sortMode).Item);
            condition.SortItems = sortItems;

            #endregion

            return condition;
        }

        /// <summary>
        /// 获取类别Endeca nvalue id
        /// </summary>
        /// <param name="sysNo">subcategory Id</param>
        /// <returns>EndecaID</returns>
        public static int GetSubcategoryDimensionValues(int subcategory)
        {
            if (subcategory <= 0)
            {
                return 0;
            }

            return ConstValue.SINGLE_SUBCATEGORY_DMSID_SEED + subcategory;
        }

        /// <summary>
        /// 获取品牌nvalue id
        /// </summary>
        /// <param name="sysNo">brand Id</param>
        /// <returns>nvalue</returns>
        public static int GetBrandNValue(int brandSysNo)
        {
            if (brandSysNo <= 0)
            {
                return 0;
            }

            return ConstValue.SINGLE_BRAND_STORE_DMSID_SEED + brandSysNo;
        }

        /// <summary>
        /// 获取NavigationKey
        /// </summary>
        /// <returns></returns>
        private static string GetNavigationKey()
        {
            string strNValue = HttpContext.Current.Request.QueryString["N"];
            //if (endecaId == 0)
            //{
            //    return strNValue;
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(strNValue))
            //    {
            //        if (strNValue.Contains(endecaId.ToString()))
            //        {
            //            return strNValue;
            //        }
            //        else
            //        {
            //            return String.Format("{0} {1}", endecaId, strNValue).Trim();
            //        }
            //    }
            //    return null;
            //}
            return strNValue;
        }

        /// <summary>
        /// 解析NValue获取Search页面小类ID
        /// </summary>
        /// <param name="strN"></param>
        /// <returns></returns>
        public static int GetSubCategoryIDbyStrN(string strN)
        {
            if (!string.IsNullOrEmpty(strN))
            {
                string[] nKey = strN.Split('-');
                for (int i = 0; i < nKey.Length; i++)
                {
                    if (nKey[i].Length == 9 && nKey[i].Substring(0, 3) == "400")
                    {
                        int key = 0;
                        int.TryParse(nKey[i], out key);
                        if (key <= 0) { key = 400000000; }
                        return key - 40000 * 10000;
                    }

                    if (nKey[i].Length == 10)
                    {
                        int key2 = 0;
                        int.TryParse(nKey[i].Substring(5, 4), out key2);
                        return key2;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取Filters
        /// </summary>
        /// <returns></returns>
        private static List<FilterBase> GetFilters()
        {
            List<FilterBase> filters = new List<FilterBase>();

            int withDiscount = 0;
            int.TryParse(HttpContext.Current.Request.QueryString["withDiscount"], out withDiscount);

            int.TryParse(HttpContext.Current.Request.QueryString["ep"], out ep);
            pf = HttpContext.Current.Request.QueryString["pf"];
            pt = HttpContext.Current.Request.QueryString["pt"];
            //tt = HttpContext.Current.Request.QueryString["tt"];
            //复选框Property Search
            //有返现
            //if ((ep & (int)ProductSearchPropertyType.Cashrebate) == (int)ProductSearchPropertyType.Cashrebate)
            //{
            //    filters.Add(new RangeFilter("p_cashrebate", "0", "*", false));
            //    filters.Add(new RangeFilter("p_pointtype", "*", "2", false));
            //    IsShowOnlyShowProducts = false;
            //}
            ////限时抢
            //if ((ep & (int)ProductSearchPropertyType.CountDown) == (int)ProductSearchPropertyType.CountDown)
            //{
            //    filters.Add(new FieldFilter("p_countdown", "countdown"));
            //    IsShowOnlyShowProducts = false;
            //}
            ////有赠品
            //if ((ep & (int)ProductSearchPropertyType.Ishavevalidgift) == (int)ProductSearchPropertyType.Ishavevalidgift)
            //{
            //    filters.Add(new RangeFilter("p_ishavevalidgift", "0", "*", false));
            //    IsShowOnlyShowProducts = false;
            //}
            ////送积分
            //if ((ep & (int)ProductSearchPropertyType.Point) == (int)ProductSearchPropertyType.Point)
            //{
            //    filters.Add(new RangeFilter("p_point", "0", "*", false));
            //    IsShowOnlyShowProducts = false;
            //}
            ////有库存
            //if ((ep & (int)ProductSearchPropertyType.HasOnlineQty) == (int)ProductSearchPropertyType.HasOnlineQty)
            //{
            //    filters.Add(new RangeFilter("p_onlineqty", "0", "*", false));
            //    IsShowOnlyShowProducts = false;
            //}

            //有折扣
            //后来确认是否有折扣用返现来表示 poseidon
            //if (withDiscount > 0)
            //{
            //    //filters.Add(new RangeFilter("p_discount", "0", "*", false)); 
            //    filters.Add(new RangeFilter("p_cashrebate", "0", "*", false));
            //}

            //销售类型
            //if (!string.IsNullOrWhiteSpace(tt))
            //{
            //    filters.Add(new FieldFilter("p_producttradetype", tt));
            //}

            //自定义价格
            decimal i;
            if (!String.IsNullOrEmpty(pf))
            {
                if (Decimal.TryParse(pf, out i))
                {
                    filters.Add(new RangeFilter("p_pricesort", pf, "*", true));
                    IsShowOnlyShowProducts = false;
                }
            }
            if (!String.IsNullOrEmpty(pt))
            {
                if (Decimal.TryParse(pt, out i))
                {
                    filters.Add(new RangeFilter("p_pricesort", "*", pt, true));
                    IsShowOnlyShowProducts = false;
                }
            }

            //限定在某个大类中搜索
            int tabStoreID = 0;
            int.TryParse(HttpContext.Current.Request.QueryString["TabStore"], out tabStoreID);
            if (tabStoreID > 0)
            {
                filters.Add(new FieldFilter("p_tabstoreids", tabStoreID.ToString()));
            }
            if (subCategoryid > 0)
            {
                filters.Add(new FieldFilter("Nvalue", GetSubcategoryDimensionValues(subCategoryid).ToString()));
            }

            return filters;
        }

        /// <summary>
        /// 获取Expression
        /// </summary>
        /// <returns></returns>
        private static Expression GetFilterExpression()
        {
            Expression exp = new Expression();

            st = HttpContext.Current.Request.QueryString["st"];
            if (!String.IsNullOrEmpty(st))
            {
                string[] parts = st.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts != null && parts.Length > 0)
                {
                    if (parts.Length == 1)
                    {
                        exp = new Expression(exp, new Expression(new FieldFilter("p_productStoreType", parts[0])), Operation.AND);
                    }
                    else
                    {
                        Expression stExp = new Expression(new FieldFilter("p_productStoreType", parts[0]));
                        for (int i = 1; i < parts.Length; i++)
                        {
                            stExp = new Expression(stExp, new FieldFilter("p_productStoreType", parts[i]), Operation.OR);
                        }
                        exp = new Expression(exp, stExp, Operation.AND);
                    }
                }
            }
            return exp;
        }

        public static List<string> GetNValueList(string strN)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(strN) == true)
            {
                return result;
            }
            string[] strArray = strN.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray == null || strArray.Length <= 0)
            {
                return result;
            }
            foreach (string str in strArray)
            {
                //if (int.TryParse(str, out tmp))
                {
                    result.Add(str);
                }
            }
            return result;
        }
        #endregion


        public static ProductSearchResult GetProductSearchResultBySolr(ProductSearchCondition condition)
        {
            ProductSearchResult result = new ProductSearchResult();

            //商品聚合显示
            //condition.IsGroupQuery = true;

            #region  第一次搜索(初始化左导航，需要排除条件endecaId)
            //第一次搜索(初始化左导航，需要排除条件endecaId)
            ProductSearchCondition initialCondition = new ProductSearchCondition()
            {
                PagingInfo = condition.PagingInfo,
                KeyWord = condition.KeyWord,
                WithInKeyWord = condition.WithInKeyWord,
                Filters = condition.Filters,
                IsGroupQuery = condition.IsGroupQuery
            };
            if (condition.NValueList != null && condition.NValueList.Count > 0)
            {
                initialCondition.Filters = initialCondition.Filters == null ? new List<FilterBase>(0) : initialCondition.Filters;
                foreach (string item in condition.NValueList)
                {
                    if (!condition.Filters.Exists(x =>
                    {
                        if (x is FieldFilter)
                        {
                            return ((FieldFilter)x).Value.Equals(item, StringComparison.CurrentCultureIgnoreCase);
                        }
                        return false;
                    }) && !string.Equals(item, condition.EndecaId))
                    {

                        initialCondition.Filters.Add(new FieldFilter("Nvalue", item));
                    }
                }
            }

            ProductSearchResult initialResult = SearchEngineManager.Query<ProductSearchResult>(initialCondition);
            result.Navigation = initialResult.Navigation;
            #endregion

            #region  第二次搜索(初始化过滤条件，需要排除N值)
            //第二次搜索(初始化过滤条件，需要排除N值)
            ProductSearchCondition filterCondition = new ProductSearchCondition()
            {
                PagingInfo = condition.PagingInfo,
                KeyWord = condition.KeyWord,
                WithInKeyWord = condition.WithInKeyWord,
                Filters = condition.Filters == null ? new List<FilterBase>(0) : /*是否有折扣不影响条件选择区*/
                                                       condition.Filters.Where(x => x.Field != "Nvalue" && x.Field != "p_cashrebate").ToList(),
                IsGroupQuery = condition.IsGroupQuery
            };
            if (condition.NValueList != null && condition.NValueList.Count > 0)
            {
                filterCondition.Filters = filterCondition.Filters == null ? new List<FilterBase>(0) : filterCondition.Filters;
                foreach (string item in condition.NValueList)
                {
                    List<string> NValue = GetNValueList(condition.NFilter);
                    if (NValue != null && NValue.Count > 0)
                    {
                        if (!NValue.Contains(item))
                        {
                            filterCondition.Filters.Add(new FieldFilter("Nvalue", item));
                        }
                    }
                    else
                    {
                        filterCondition.Filters.Add(new FieldFilter("Nvalue", item));
                    }
                }
            }
            ProductSearchResult filterResult = SearchEngineManager.Query<ProductSearchResult>(filterCondition);
            result.FilterNavigation = filterResult.Navigation;
            #endregion

            #region 第三次搜索(搜商品，加上所有搜索条件)

            //第三次搜索(搜商品，加上所有搜索条件) 
            if (condition.NValueList != null && condition.NValueList.Count > 0)
            {
                condition.Filters = condition.Filters == null ? new List<FilterBase>(0) : condition.Filters;
                foreach (string item in condition.NValueList)
                {
                    if (!condition.Filters.Exists(x =>
                    {
                        if (x is FieldFilter)
                        {
                            return ((FieldFilter)x).Value.Equals(item, StringComparison.CurrentCultureIgnoreCase);
                        }
                        return false;
                    }))
                    {
                        condition.Filters.Add(new FieldFilter("Nvalue", item));
                    }
                }
            }
            //商品二维码搜索条件
            if (!string.IsNullOrWhiteSpace(condition.Barcode))
            {
                if (!condition.Filters.Exists(x => x.Field.Equals("p_barcode", StringComparison.CurrentCultureIgnoreCase)))
                {
                    condition.Filters.Add(new FieldFilter("p_barcode", condition.Barcode));
                }
            }
            ProductSearchResult productResult = SearchEngineManager.Query<ProductSearchResult>(condition);
            result.ProductDataList = productResult.ProductDataList;
            #endregion

            return result;
        }

        /// <summary>
        /// 商品关键字搜索自动完成功能
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ProductACSearchResult GetProductACSearchResultBySolr(string keywordPrefix)
        {
            SearchCondition condition = new SearchCondition()
            {
                KeyWord = keywordPrefix
            };
            ProductACSearchResult result = SearchEngineManager.Query<ProductACSearchResult>(condition);

            return result;
        }
    }
}
