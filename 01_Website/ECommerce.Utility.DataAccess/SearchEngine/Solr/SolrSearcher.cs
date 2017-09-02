using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace ECommerce.Utility.DataAccess.SearchEngine.Solr
{
    /// <summary>
    /// Solr检索器，Solr检索的核心，提供查询服务
    /// </summary>
    /// <typeparam name="Record">Solr检索返回结果文档类型</typeparam>
    /// <typeparam name="Result">对外返回查询结果数据类型</typeparam>
    public abstract class  SolrSearcher<Record, Result> : Searcher<Result>
    {
        /// <summary>
        /// 真正的检索方法，需要派生类进行实现
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns>检索结果</returns>
        protected override Result GetSearchResult(SearchCondition condition)
        {
            ISolrOperations<Record> solr = null;
            try {
                solr = ServiceLocator.Current.GetInstance<ISolrOperations<Record>>();
            } 
            catch {
                throw new ConfigurationErrorsException(SolrCoreName + "，没有进行初始化");
            }
            return GetSearchResult(condition, solr);
        }

        /// <summary>
        /// 真正的检索方法，需要派生类进行实现
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="solr">Solr检索器对象</param>
        /// <returns>检索结果</returns>
        protected virtual Result GetSearchResult(SearchCondition condition, ISolrOperations<Record> solr)
        {
            QueryOptions queryOptions = BuildQueryOptions(condition);

            List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
            if (condition.KeyWord != "*:*")
            {
                keyValuePairList.Add(new KeyValuePair<string, string>("defType", "extlucene"));
            }
            //queryOptions.ExtraParams = keyValuePairList.ToArray();
            //fixbug
            if (queryOptions.ExtraParams == null || queryOptions.ExtraParams.Count() > 0)
            {
                List<KeyValuePair<string, string>> oldExtraParams = new List<KeyValuePair<string, string>>(queryOptions.ExtraParams);
                //配置以派生类为准
                keyValuePairList.RemoveAll(v => oldExtraParams.Any(f => f.Key == v.Key));
                keyValuePairList.AddRange(oldExtraParams);
            }
            queryOptions.ExtraParams = keyValuePairList.ToArray();

            ISolrQuery query = new SolrQuery(condition.KeyWord);

            SolrQueryResults<Record> result = solr.Query(query, queryOptions);

            return TransformSolrQueryResult(result, condition);
        }

        /// <summary>
        /// 构建Solr检索选项，派生类可根据需要进行重写
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual QueryOptions BuildQueryOptions(SearchCondition condition)
        {
            QueryOptions queryOptions = new QueryOptions();

            #region 搜索关键字

            if (condition.KeyWord != null && condition.KeyWord != "*:*")
            {
                condition.KeyWord = condition.KeyWord
                    .Replace("+", "\\+")
                    .Replace("-", "\\-")
                    .Replace("\"", "\\\"")
                    .Replace("'", "\\'")
                    .Replace("*", "\\*")
                    .Replace(":", "\\:")
                    .Trim();
            }

            if (String.IsNullOrEmpty(condition.KeyWord))
            {
                condition.KeyWord = "*:*";
            }

            #endregion 搜索关键字

            #region 分页信息

            if (condition.PagingInfo == null)
            {
                condition.PagingInfo = new PagingInfo()
                {
                    PageSize = 24,
                    PageNumber = 1
                };
            }
            queryOptions.Rows = condition.PagingInfo.PageSize >= 0 ? condition.PagingInfo.PageSize : 24;
            queryOptions.Start = condition.PagingInfo.PageNumber > 1 ? queryOptions.Rows * (condition.PagingInfo.PageNumber - 1) : 0;

            #endregion 分页信息

            #region 排序信息

            if (condition.SortItems != null && condition.SortItems.Count > 0)
            {
                List<SortOrder> sortList = new List<SortOrder>();

                foreach (SortItem sortItem in condition.SortItems)
                {
                    if (sortItem.SortType == SortOrderType.ASC)
                    {
                        SortOrder sortOrder = new SortOrder(sortItem.SortKey, Order.ASC);
                        sortList.Add(sortOrder);
                    }
                    else
                    {
                        SortOrder sortOrder = new SortOrder(sortItem.SortKey, Order.DESC);
                        sortList.Add(sortOrder);
                    }
                }
                queryOptions.OrderBy = sortList.ToArray();
            }

            #endregion 排序信息

            #region 一般过滤条件

            if (condition.Filters != null && condition.Filters.Count > 0)
            {
                foreach (FilterBase filter in condition.Filters)
                {
                    if (filter is RangeFilter)
                    {
                        RangeFilter rf = filter as RangeFilter;
                        if (!string.IsNullOrWhiteSpace(rf.From) || !string.IsNullOrWhiteSpace(rf.To))
                        {
                            queryOptions.FilterQueries.Add(new SolrQueryByRange<string>(rf.Field, rf.From, rf.To, rf.Inclusive, rf.Inclusive));
                        }
                    }
                    else if (filter is FieldFilter)
                    {
                        FieldFilter ff = filter as FieldFilter;
                        if (!string.IsNullOrWhiteSpace(ff.Value))
                        {
                            queryOptions.FilterQueries.Add(new SolrQueryByField(ff.Field, ff.Value));
                        }
                    }
                }
            }

            #endregion 一般过滤条件

            #region 复杂过滤条件计算

            AbstractSolrQuery expressiongResult = ComputerExpression(condition.FilterExpression);
            if (expressiongResult != null)
            {
                queryOptions.FilterQueries.Add(expressiongResult);
            }

            #endregion 复杂过滤条件计算

            return queryOptions;
        }

        /// <summary>
        /// 转换Solr返回的检索文档，需要派生类进行实现
        /// </summary>
        /// <param name="solrQueryResult">Solr检索返回的文档结果</param>
        /// <param name="condition">查询条件</param>
        /// <returns>对外返回查询结果数据</returns>
        protected abstract Result TransformSolrQueryResult(SolrQueryResults<Record> solrQueryResult, SearchCondition condition);

        #region Private Method 复杂过滤条件计算

        private static AbstractSolrQuery ComputerExpression(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            else if (expression.HasChild() == false)
            {
                return Compute(expression.NodeData, expression.Operation);
            }
            else
            {
                AbstractSolrQuery leftData = ComputerExpression(expression.LeftNode);
                AbstractSolrQuery rightData = ComputerExpression(expression.RightNode);
                return Compute(leftData, rightData, expression.Operation);
            }
        }

        private static AbstractSolrQuery Compute(AbstractSolrQuery a, AbstractSolrQuery b, Operation op)
        {
            switch (op)
            {
                case Operation.NOT:
                    if (a != null)
                        return !a;
                    else if (b != null)
                        return !b;
                    else
                        return null;

                case Operation.AND:
                    if (a != null && b != null)
                        return a && b;
                    else if (a != null)
                        return a;
                    else
                        return b;

                case Operation.OR:
                    if (a != null && b != null)
                        return a || b;
                    else if (a != null)
                        return a;
                    else
                        return b;

                default:
                    if (a != null)
                        return a;
                    else if (b != null)
                        return b;
                    else
                        return null;
            }
        }

        private static AbstractSolrQuery Compute(FilterBase filter, Operation op)
        {
            if (filter != null)
            {
                if (filter is RangeFilter)
                {
                    RangeFilter rf = filter as RangeFilter;
                    return Compute(new SolrQueryByRange<string>(rf.Field, rf.From, rf.To, rf.Inclusive), null, op);
                }
                else if (filter is FieldFilter)
                {
                    FieldFilter ff = filter as FieldFilter;
                    return Compute(new SolrQueryByField(ff.Field, ff.Value), null, op);
                }
            }
            return null;
        }

        #endregion Private Method 复杂过滤条件计算

        /// <summary>
        /// Solr Core名称，和Solr服务配置的Core名称一致，系统根据该名称和Solr服务基地址找到Solr Core服务地址
        /// </summary>
        protected abstract string SolrCoreName { get; }

        /// <summary>
        /// 是否已经初始化了SolrNet组件
        /// </summary>
        private static bool s_SolrNetComponentHasInitialized = false;

        /// <summary>
        /// 重写基类初始化方法，进行SolrNet组件初始化，将检索器注入到IoC容器中
        /// </summary>
        protected override void Init()
        {
            base.Init();

            if (!s_SolrNetComponentHasInitialized)
            {
                //初始化 SolrNet
                string serviceURL = Path.Combine(SolrSearchProvider.ServiceBaseUrl, SolrCoreName);
                Startup.Init<Record>(serviceURL);

                s_SolrNetComponentHasInitialized = true;
            }
        }
    }
}