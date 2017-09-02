using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICSQueryDA))]
    public class CSQueryDA : ICSQueryDA
    {
        #region ICSQueryDA Members

        public virtual DataTable Query(CSQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCSListByQuery");


            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " a.UserName ASC "))
            {
                if (queryCriteria.IsGetUnderling)
                {
                    if (queryCriteria.Role == CSRole.Leader)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "b.UserName",
                        DbType.String, "@UserName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.Name);
                    }
                    else if (queryCriteria.Role == CSRole.Manager)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "c.UserName",
                       DbType.String, "@UserName",
                       QueryConditionOperatorType.Like,
                       queryCriteria.Name);
                    }
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "a.UserName",
                       DbType.String, "@UserName",
                       QueryConditionOperatorType.Like,
                       queryCriteria.Name);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.Role",
                        DbType.Int32, "@Role",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Role);
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
              queryCriteria.CompanyCode);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(3, typeof(CSRole));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
