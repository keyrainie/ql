using System;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICategoryKPIQueryDA))]
    public class CategoryKPIQueryDA : ICategoryKPIQueryDA
    {
        /// <summary>
        /// 查询类别KPI指标
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryCategoryKPIList(CategoryKPIQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            CustomDataCommand dataCommand;
            if (queryCriteria.CategoryType == CategoryType.CategoryType3)
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategoryKPIList");
                using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "A.SysNo DESC"))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.CompanyCode",
                        DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal,
                        "8601");

                    if (queryCriteria.Status != -999)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.Status",
                        DbType.Int32, "@Status", QueryConditionOperatorType.Equal,
                        queryCriteria.Status);
                    }
                    if (queryCriteria.C3SysNo != null && queryCriteria.C3SysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo",
                       DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal,
                       queryCriteria.C3SysNo);
                    }
                    if (queryCriteria.C2SysNo != null && queryCriteria.C2SysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.C2SysNo",
                       DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal,
                       queryCriteria.C2SysNo);
                    }
                    if (queryCriteria.C1SysNo != null && queryCriteria.C1SysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.C1SysNo",
                       DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal,
                       queryCriteria.C1SysNo);
                    }
                    dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                    DataTable dt = dataCommand.ExecuteDataTable();
                    totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                    return dt;
                }
            }
            else
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategoryKPIList2");
                using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "A.SysNo DESC"))
                {
                    if (queryCriteria.Status != -999)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.Status",
                        DbType.Int32, "@Status", QueryConditionOperatorType.Equal,
                        queryCriteria.Status);
                    }
                    if (queryCriteria.C2SysNo != null && queryCriteria.C2SysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.SysNo",
                       DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal,
                       queryCriteria.C2SysNo);
                    }
                    if (queryCriteria.C1SysNo != null && queryCriteria.C1SysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.C1SysNo",
                       DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal,
                       queryCriteria.C1SysNo);
                    }
                    dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                    DataTable dt = dataCommand.ExecuteDataTable();
                    totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                    return dt;
                }
            }
          

            //StringBuilder joinTableBuilder = new StringBuilder();
            //            if (queryCriteria.PMUserSysNo > 0 && queryCriteria.C3SysNo>0)
            //            {
            //                joinTableBuilder.AppendFormat(@"     INNER JOIN (
            //                    SELECT DISTINCT(C3.SysNo) FROM IPP3.dbo.Category3 C3 WITH(NOLOCK)
            //			        LEFT JOIN IPP3.dbo.Product P WITH(NOLOCK) ON P.C3SysNo=C3.SysNo AND C3.CompanyCode=P.CompanyCode
            //			        WHERE  P.PMUserSysNo={0} AND C3.CompanyCode=@CompanyCode
            //                )P ON A.SysNo=P.SysNo", queryCriteria.PMUserSysNo);
            //            }
            // dataCommand.CommandText = dataCommand.CommandText.Replace("#StrJoinTable#", joinTableBuilder.ToString());

          
        }
    }
}
