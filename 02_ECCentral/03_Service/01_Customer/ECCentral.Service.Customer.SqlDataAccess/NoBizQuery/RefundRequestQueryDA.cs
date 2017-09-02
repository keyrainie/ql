using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IRefundRequestQueryDA))]
    public class RefundRequestQueryDA : IRefundRequestQueryDA
    {
        #region IRefundRequestQueryDA Members

        public virtual DataTable Query(QueryFilter.Customer.RefundRequestQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRefundRequest");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " R.SysNo DESC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "R.RequestType",
                DbType.String, "@RequestType",
                QueryConditionOperatorType.Equal,
                queryCriteria.RequestType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "R.SysNo",
                   DbType.Int32, "@SysNo",
                   QueryConditionOperatorType.Equal,
                   queryCriteria.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.SOSysNo",
                    DbType.Int32, "@SOSysNo",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.SOSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
              "R.Status",
              DbType.String, "@Status",
              QueryConditionOperatorType.Equal,
              queryCriteria.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
        "R.RefundType",
        DbType.String, "@RefundType",
        QueryConditionOperatorType.Equal,
        queryCriteria.RefundType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.CustomerID",
                    DbType.String, "@CustomerID",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.CustomerId);
                                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.EditUser",
                    DbType.String, "@EditUser",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.EditUserName);
                                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.InDate",
                    DbType.DateTime, "@DateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    queryCriteria.CreateFrom);
                                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.InDate",
                    DbType.DateTime, "@DateTo",
                    QueryConditionOperatorType.LessThan,
                    queryCriteria.CreateTo);

                                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.EditDate",
                    DbType.DateTime, "@EditDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    queryCriteria.EditFrom);
                                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "R.EditDate",
                    DbType.DateTime, "@EditDateTo",
                    QueryConditionOperatorType.LessThan,
                    queryCriteria.EditTo);

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "R.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                queryCriteria.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add(4, typeof(RefundRequestType));
                enumList.Add(12, typeof(RefundRequestStatus));
                CodeNamePairColumnList cpList = new CodeNamePairColumnList();
                cpList.Add(5, "Customer", "RefundType");
                DataTable dt = cmd.ExecuteDataTable(enumList, cpList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
