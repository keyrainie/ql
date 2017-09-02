using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Entity.SolrSearch;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.Utility.DataAccess.SearchEngine;

namespace Nesoft.ECWeb.UI.Controllers
{
    public class ProductSearchController : WWWControllerBase
    {
        //
        // GET: /Web/Product/

        /// <summary>
        /// 搜索结果
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchResult()
        {
            //获取Condition
            ProductSearchCondition condition = GetSearchCondition();
            //获取当前页面Querystring
            queryStringCollection = Request.QueryString;

            //获取搜索结果
            ProductSearchResult result = ProductSearchFacade.GetProductSearchResultBySolr(condition, queryStringCollection);
            return RedirectToAction("SearchResult", "Product");


        }

        #region  填充搜索条件
        /// <summary>
        /// View
        /// </summary>
        //private SearchPageView view;

        /// <summary>
        /// 搜索关键字
        /// </summary>
        private string keyWord;


        /// <summary>
        /// 是否显示仅展示商品
        /// </summary>
        private bool IsShowOnlyShowProducts = true;

        /// <summary>
        /// 小类ID
        /// </summary>
        protected int subCategoryid;

        /// <summary>
        /// 小类Endeca ID
        /// </summary>
        private int endecaId;

        /// <summary>
        /// N
        /// </summary>
        private string strN;

        /// <summary>
        /// SearchWithIn 复选框Property
        /// </summary>
        private int ep;

        /// <summary>
        /// 自定义价格开始
        /// </summary>
        private string pf;

        /// <summary>
        /// 自定义价格截止
        /// </summary>
        private string pt;

        /// <summary>
        /// SearchWithin Keyword
        /// </summary>
        private string withInKeyword;

        /// <summary>
        /// 第几页
        /// </summary>
        private int pageNumber;

        /// <summary>
        /// 每页显示个数
        /// </summary>
        private int pageSize;

        /// <summary>
        /// 产品总数
        /// </summary>
        private int itemCount;

        /// <summary>
        /// Querystring
        /// </summary>
        private NameValueCollection queryStringCollection;

        /// <summary>
        /// 排序方式
        /// </summary>
        private int sortMode;

        /// <summary>
        /// 产品数
        /// </summary>
        protected int totalItem;
        /// <summary>
        /// 投票总数
        /// </summary>
        protected int totalPolls;

        /// <summary>
        /// Dim展开与收起CookieKey
        /// </summary>
        protected string cookiekey;
        /// <summary>
        /// 获取Endeca Search Condition
        /// </summary>
        /// <returns></returns>
        private ProductSearchCondition GetSearchCondition()
        {

            this.keyWord = Request.QueryString["keyword"];
            int.TryParse(Request.QueryString["enid"], out this.endecaId);
            int.TryParse(Request.QueryString["sort"], out this.sortMode);
            this.withInKeyword = Request.QueryString["withInKeyword"];
            this.strN = GetNavigationKey();
            this.subCategoryid = GetSubCategoryIDbyStrN(this.strN);
            int.TryParse(Request.QueryString["page"], out pageNumber);
            int.TryParse(Request.QueryString["pageSize"], out pageSize);

            ProductSearchCondition condition = new ProductSearchCondition();
            condition.Filters = GetFilters();
            condition.FilterExpression = GetFilterExpression();

            //是否展示“仅展示”商品
            if (this.IsShowOnlyShowProducts == false)
            {
                condition.Filters.Add(new FieldFilter("p_productstatus", "1"));
            }
            condition.KeyWord = string.IsNullOrEmpty(keyWord)?string.Empty:keyWord.Trim();
            condition.WithInKeyWord = withInKeyword;
            condition.NValueList = GetNValueList(strN);

            
            condition.PagingInfo = new Nesoft.Utility.DataAccess.SearchEngine.PagingInfo();
            condition.PagingInfo.PageNumber = this.pageNumber;
            condition.PagingInfo.PageSize = this.pageSize;

            //condition.SortingInfo = new global::Newegg.Framework.SortingInfo();
            //condition.SortingInfo.SortField = this.sortMode.ToString();

            if (!string.IsNullOrWhiteSpace(this.keyWord) && this.sortMode<=10)
            {
                this.sortMode = 9999;
            }

            return condition;
        }

        /// <summary>
        /// 获取NavigationKey
        /// </summary>
        /// <returns></returns>
        private string GetNavigationKey()
        {
            string strNValue = Request.QueryString["N"];
            if (endecaId == 0)
            {
                return strNValue;
            }
            else
            {
                if (strNValue.Contains(endecaId.ToString()))
                {
                    return strNValue;
                }
                else
                {
                    return String.Format("{0} {1}", endecaId, strNValue).Trim();
                }
            }
        }

        /// <summary>
        /// 解析NValue获取Search页面小类ID
        /// </summary>
        /// <param name="strN"></param>
        /// <returns></returns>
        private int GetSubCategoryIDbyStrN(string strN)
        {
            if (!string.IsNullOrEmpty(strN))
            {
                string[] nKey = strN.Split(' ');
                for (int i = 0; i < nKey.Length; i++)
                {
                    if (nKey[i].Length == 9 && nKey[i].Substring(0, 5) == "40000")
                    {
                        int key = 0;
                        int.TryParse(nKey[i], out key);
                        if (key <= 0) { key = 400000000; }
                        return key - 40000 * 10000;
                    }
                    //Todo

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
        private List<FilterBase> GetFilters()
        {
            List<FilterBase> filters = new List<FilterBase>();
            int.TryParse(Request.QueryString["ep"], out this.ep);
            this.pf = Request.QueryString["fp"];
            this.pt = Request.QueryString["pt"];
            //复选框Property Search
            //有返现
            //if ((ep & (int)ProductSearchPropertyType.Cashrebate) == (int)ProductSearchPropertyType.Cashrebate)
            //{
            //    filters.Add(new RangeFilter("p_cashrebate", "0", "*", false));
            //    filters.Add(new RangeFilter("p_pointtype", "*", "2", false));
            //    this.IsShowOnlyShowProducts = false;
            //}
            ////限时抢
            //if ((ep & (int)ProductSearchPropertyType.CountDown) == (int)ProductSearchPropertyType.CountDown)
            //{
            //    filters.Add(new FieldFilter("p_countdown", "countdown"));
            //    this.IsShowOnlyShowProducts = false;
            //}
            ////有赠品
            //if ((ep & (int)ProductSearchPropertyType.Ishavevalidgift) == (int)ProductSearchPropertyType.Ishavevalidgift)
            //{
            //    filters.Add(new RangeFilter("p_ishavevalidgift", "0", "*", false));
            //    this.IsShowOnlyShowProducts = false;
            //}
            ////送积分
            //if ((ep & (int)ProductSearchPropertyType.Point) == (int)ProductSearchPropertyType.Point)
            //{
            //    filters.Add(new RangeFilter("p_point", "0", "*", false));
            //    this.IsShowOnlyShowProducts = false;
            //}
            ////有库存
            //if ((ep & (int)ProductSearchPropertyType.HasOnlineQty) == (int)ProductSearchPropertyType.HasOnlineQty)
            //{
            //    filters.Add(new RangeFilter("p_onlineqty", "0", "*", false));
            //    this.IsShowOnlyShowProducts = false;
            //}

            //自定义价格
            decimal i;
            if (!String.IsNullOrEmpty(pf))
            {
                if (Decimal.TryParse(pf, out i))
                {
                    filters.Add(new RangeFilter("p_pricesort", pf, "*", true));
                    this.IsShowOnlyShowProducts = false;
                }
            }
            if (!String.IsNullOrEmpty(pt))
            {
                if (Decimal.TryParse(pt, out i))
                {
                    filters.Add(new RangeFilter("p_pricesort", "*", pt, true));
                    this.IsShowOnlyShowProducts = false;
                }
            }

            //限定在某个大类中搜索
            int tabStoreID = 0;
            int.TryParse(Request.QueryString["TabStore"], out tabStoreID);
            if (tabStoreID > 0)
            {
                filters.Add(new FieldFilter("p_tabstoreids", tabStoreID.ToString()));
            }

            if (this.endecaId > 0)
            {
                filters.Add(new FieldFilter("Nvalue", this.endecaId.ToString()));
            }
            return filters;
        }

        private Expression GetFilterExpression()
        {
            Expression result = new Expression();
            int.TryParse(Request.QueryString["ep"], out this.ep);

            //免运费
            //if ((ep & (int)ProductSearchPropertyType.NoShipFreeShipType) == (int)ProductSearchPropertyType.NoShipFreeShipType)
            //{
            //    Expression expA = new Expression(new RangeFilter("p_currentprice", ParamHelper.FreeShippingFeeSOAmountLimit.ToString(), "*"));
            //    expA = new Expression(expA, new RangeFilter("p_weight", "*", ParamHelper.FreeShippingFeeWeightLimit.ToString(), false), Operation.AND);
            //    expA = new Expression(expA, new FieldFilter("p_isnegshippingtype", "1"), Operation.AND);

            //    if (ParamHelper.IsShowVenderFreeShippingICON == true)
            //    {
            //        Expression expB = new Expression(new FieldFilter("p_invoicetype", VendorInfo.MET),
            //                                            new FieldFilter("p_shippingtype", VendorInfo.MET),
            //                                            Operation.AND);

            //        result = new Expression(expA, expB, Operation.OR);
            //    }
            //    else
            //    {
            //        result = expA;
            //    }
            //    this.IsShowOnlyShowProducts = false;
            //}

            return result;
        }


        private List<string> GetNValueList(string strN)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(strN) == true)
            {
                return result;
            }
            string[] strArray = strN.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

    }
}
