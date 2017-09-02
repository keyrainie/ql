using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISEOQueryDA))]
    public class SEOQueryDA : ISEOQueryDA
    {
        public DataTable QuerySEO(SEOQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SEO_GetSEODataList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "B.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                if (filter.PageID == 1 || filter.PageID == 2 || filter.PageID == 3)
                {
                    sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                    "C3.C1Sysno", DbType.Int32, "@PageID",
                    QueryConditionOperatorType.Equal, filter.PageID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                    "C3.C2Sysno", DbType.Int32, "@PageID",
                    QueryConditionOperatorType.Equal, filter.PageID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                    "C3.C3Sysno", DbType.Int32, "@PageID",
                    QueryConditionOperatorType.Equal, filter.PageID);
                    sqlBuilder.ConditionConstructor.EndGroupCondition();
                }
                else
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "cm.PageID", DbType.Int32, "@PageID", QueryConditionOperatorType.Equal, filter.PageID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "cm.PageType", DbType.Int32, "@PageType",
                    QueryConditionOperatorType.Equal, filter.PageType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "cm.PageTitle", DbType.String, "@PageTitle",
                    QueryConditionOperatorType.Like, filter.PageTitle);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "cm.PageDescription", DbType.String, "@PageDescription",
                    QueryConditionOperatorType.Like, filter.PageDescription);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "cm.PageKeywords", DbType.String, "@PageKeywords",
                    QueryConditionOperatorType.Like, filter.PageKeywords);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "cm.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "cm.Status", DbType.String, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
