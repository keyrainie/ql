using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPrePayQueryDA))]
    public class PrepayDA : IPrePayQueryDA
    {

        public virtual DataTable QueryPrePayLogIncome(PrePayQueryFilter filter, out int totalCount)
        {
            return QueryPrePayLog(filter, "QueryPrePayLogIncome", out totalCount);
        }

        public virtual DataTable QueryPrePayLogPayment(PrePayQueryFilter filter, out int totalCount)
        {
            return QueryPrePayLog(filter, "QueryPrePayLogPayment", out totalCount);
        }
        private static DataTable QueryPrePayLog(PrePayQueryFilter queryEntity, string sqlName, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig(sqlName);

            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryEntity.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryEntity.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryEntity.PagingInfo.PageIndex * queryEntity.PagingInfo.PageSize;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                   cmd.CommandText,
                   cmd,
                   pagingEntity,
                   "CustomerSysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "CustomerSysNo", DbType.Int32, "@CustomerSysNo",
                   QueryConditionOperatorType.Equal, queryEntity.CustomerSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CreateTime", DbType.DateTime, "@CreateTimeFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, queryEntity.CreateDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CreateTime", DbType.DateTime, "@CreateTimeTo",
                    QueryConditionOperatorType.LessThan, queryEntity.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "OrderSysNo", DbType.Int32, "@OrderSysNo",
                    QueryConditionOperatorType.Equal, queryEntity.OrderSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "PrePayType", DbType.Int32, "@PrePayType",
                   QueryConditionOperatorType.Equal, queryEntity.PrePayType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "Status", DbType.AnsiStringFixedLength, "@Status",
                   QueryConditionOperatorType.Equal, queryEntity.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CompanyCode", DbType.AnsiString, "@CompanyCode",
                        QueryConditionOperatorType.Equal,
                        queryEntity.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
