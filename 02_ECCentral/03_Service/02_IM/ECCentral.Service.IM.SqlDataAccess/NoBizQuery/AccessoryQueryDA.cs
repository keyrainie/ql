using System;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IAccessoryQueryDA))]
    public class AccessoryQueryDA : IAccessoryQueryDA
    {
        /// <summary>
        /// 查询配件
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryAccessory(AccessoryQueryFilter queryCriteria, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryAccessory");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "AccessoryInfo.SysNo DESC"))
            {
                if (!String.IsNullOrEmpty(queryCriteria.AccessoryName))
                {
                    dataCommand.AddInputParameter("@AccessoryName", DbType.String, queryCriteria.AccessoryName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "AccessoriesName",
                        DbType.String, "@AccessoryName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.AccessoryName);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
