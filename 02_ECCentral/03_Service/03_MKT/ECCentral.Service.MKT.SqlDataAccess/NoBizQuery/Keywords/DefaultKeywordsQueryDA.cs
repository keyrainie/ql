using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IDefaultKeywordsQueryDA))]
    public class DefaultKeywordsQueryDA : IDefaultKeywordsQueryDA
    {
        /// <summary>
        /// 查询默认关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryDefaultKeywords(DefaultKeywordsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
            {
                pagingEntity = null;
            }
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Keyword_QueryDefaultKeywordsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "TransactionNumber DESC"))
            {
                //TODO:添加渠道查询条件
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DefaultKeyword", DbType.String, "@DefaultKeyword", QueryConditionOperatorType.Like, filter.Keywords);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PageType", DbType.Int32, "@PageType", QueryConditionOperatorType.Equal, filter.PageType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PageID", DbType.Int32, "@PageID", QueryConditionOperatorType.Equal, filter.PageID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "BeginDate", DbType.DateTime, "@BeginDate", 
                    QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.BeginDateFrom, filter.BeginDateTo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "EndDate", DbType.DateTime, "@EndDate",
                    QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.EndDateFrom, filter.EndDateTo);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
