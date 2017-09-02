using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IKeywordQueryDA
    {
        /// <summary>
        /// 查询自动匹配关键字
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QuerySearchedKeywords(SearchedKeywordsFilter filter, out int totalCount);

        /// <summary>
        /// 查询中文词库
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QuerySegment(SegmentQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询同义词
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryThesaurusKeywords(ThesaurusKeywordsQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询阻止词
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryStopWords(StopWordsQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询跳转关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryAdvancedKeywords(AdvancedKeywordsQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询分类关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryCategoryKeywords(CategoryKeywordsQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询关键字对应商品
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryKeyWordsForProduct(KeyWordsForProductQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询产品页面关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryProductPageKeywords(ProductKeywordsQueryFilter filter, out int totalCount);
    }
}
