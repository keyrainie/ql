using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery.Keywords
{
    [VersionExport(typeof(IHotKeywordsQueryDA))]
    public class HotKeywordsQueryDA : IHotKeywordsQueryDA
    {

        /// <summary>
        /// 查询热门关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryHotKeywords(HotKeywordsQueryFilter filter, out int totalCount)
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

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Keyword_QueryHotKeywordsList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //TODO:添加渠道查询条件
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.PageType", DbType.Int32, "@PageType", QueryConditionOperatorType.Equal, filter.PageType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.PageID", DbType.Int32, "@PageID", QueryConditionOperatorType.Equal, filter.PageID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.Keyword", DbType.String, "@Keyword", QueryConditionOperatorType.Like, filter.Keywords);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.Priority", DbType.Int32, "@Priority", QueryConditionOperatorType.Equal, filter.Priority);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.EditUserSysNo", DbType.Int32, "@EditUserSysNo", QueryConditionOperatorType.Equal, filter.EditUserSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.IsOnlineShow", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.IsOnlineShow);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sk.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "sk.EditDate", DbType.DateTime, "@EditDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.EditDateFrom, filter.EditDateTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "sk.HiddenDate", DbType.DateTime, "@HiddenDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan, filter.InvalidDateFrom, filter.InvalidDateTo);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.NYNStatus>("IsOnlineShow");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
