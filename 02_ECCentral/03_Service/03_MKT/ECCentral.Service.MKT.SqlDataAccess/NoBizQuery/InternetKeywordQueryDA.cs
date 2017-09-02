using System;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;


namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IInternetKeywordQueryDA))]
    public class InternetKeywordQueryDA : IInternetKeywordQueryDA
    {

        /// <summary>
        /// 查询外网搜素
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryKeyword(InternetKeywordQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryKeyword");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                if (!string.IsNullOrWhiteSpace(queryCriteria.SearchKeyword))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "SearchKeyword",
                        DbType.String, "@SearchKeyword",
                        QueryConditionOperatorType.Like,
                        queryCriteria.SearchKeyword);
                }

                if (queryCriteria.Status != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.Status",
                    DbType.String, "@Status",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.Status.Value);
                }

                if (queryCriteria.BeginDate!=null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "InDate",
                        DbType.String, "@BeginDate",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryCriteria.BeginDate.Value);
                }
                if (queryCriteria.EndDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "InDate",
                        DbType.String, "@EndDate",
                        QueryConditionOperatorType.LessThan,
                        queryCriteria.EndDate.Value);
                }

               

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(IsDefaultStatus) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
