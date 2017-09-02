using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICustomerPointsAddRequestQueryDA))]
    public class CustomerPointsAddRequestQueryDA : ICustomerPointsAddRequestQueryDA
    {
        #region ICustomerPointsAddRequestQueryDAL Members

        public virtual DataTable Query(CustomerPointsAddRequestFilter queryCriteria, out int totalCount)
        {
            totalCount = 0;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SelectCustomerPointAddRequest");

            PagingInfoEntity pagingInfo = BuildPagingInfoEntity(queryCriteria.PageInfo);
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                cmd.CommandText, cmd, pagingInfo, "a.SysNo DESC"))
            {

                if (queryCriteria.SystemNumber.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "a.SysNo",
                       DbType.Int32,
                       "@SysNo",
                       QueryConditionOperatorType.Equal,
                       queryCriteria.SystemNumber);
                }
                else
                {

                    //创建时间开始
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CreateTime", DbType.DateTime, "@CreateTimeFrom",
                         QueryConditionOperatorType.MoreThanOrEqual,
                         queryCriteria.CreateDateFrom);

                    //创建时间结束
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CreateTime", DbType.DateTime, "@CreateTimeTo",
                        QueryConditionOperatorType.LessThan,
                        queryCriteria.CreateDateTo);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "b.CustomerID",
                        DbType.String,
                        "@CustomerID",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.CustomerID);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "a.SOSysNo",
                        DbType.Int32,
                        "@SOSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.SOSysNo);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "a.NewEggAccount",
                        DbType.String,
                        "@NewEggAccount",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.NeweggAccountDesc);

                    sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "a.OwnByDepartment",
                       DbType.String,
                       "@OwnByDepartment",
                       QueryConditionOperatorType.Equal,
                       queryCriteria.OwnByDepartmentDesc);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "a.memo",
                        DbType.String,
                        "@OwnByReason",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.OwnByReasonDesc);

                    sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "a.Status",
                         DbType.Int32,
                         "@Status",
                         QueryConditionOperatorType.Equal,
                         queryCriteria.Status);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "a.CompanyCode",
                        DbType.AnsiStringFixedLength,
                        "@CompanyCode",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.CompanyCode);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable("status", typeof(CustomerPointsAddRequestStatus));

                totalCount = int.Parse(cmd.GetParameterValue("@TotalCount").ToString());
                return dt;
            }
        }


        public virtual DataTable QueryRequestItems(CustomerPointsAddRequestFilter queryCriteria)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SelectCustomerPointAddRequestItem");
            cmd.SetParameterValue("@PointAddRequestSysNo", queryCriteria.SystemNumber);
            cmd.SetParameterValue("@CompanyCode", queryCriteria.CompanyCode);
            cmd.SetParameterValue("@Status", "A");
            DataSet ds = cmd.ExecuteDataSet();
            return ds.Tables[0];
        }

        #endregion

        private PagingInfoEntity BuildPagingInfoEntity(dynamic pagingInfo)
        {
            return new PagingInfoEntity()
            {
                SortField = (pagingInfo.SortBy == null ? "" : pagingInfo.SortBy),
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }
    }
}
