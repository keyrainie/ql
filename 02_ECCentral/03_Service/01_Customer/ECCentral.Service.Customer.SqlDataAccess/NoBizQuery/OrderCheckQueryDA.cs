using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IOrderCheckMasterQueryDA))]
    public class OrderCheckMasterQueryDA : IOrderCheckMasterQueryDA
    {

        #region IOrderCheckMasterQueryDA Members

        public virtual DataTable Query(OrderCheckMasterQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetOrderCheckMaster");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " SysNo "))
            {
                cmd.SetParameterValue("@CompanyCode", queryCriteria.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(3, typeof(OrderCheckStatus));
                totalCount = dt.Rows.Count;
                return dt;
            }
        }
        #endregion
    }

    [VersionExport(typeof(IOrderCheckItemQueryDA))]
    public class OrderCheckItemQueryDA : IOrderCheckItemQueryDA
    {
        #region IOrderCheckItemQueryDA Members

        public virtual DataTable Query(OrderCheckItemQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetOrderCheckItem");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                if (string.Compare(queryCriteria.ReferenceType, "PC3") == 0)
                {
                    if (queryCriteria.C1SysNo.HasValue)
                    {
                        string subQuerySQLC1 = @"SELECT [Category3Sysno]
                        FROM  OverseaContentManagement.dbo.V_CM_CategoryInfo with(nolock)
                        WHERE [Category1Sysno] = " + queryCriteria.C1SysNo;

                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                       "a.Description",
                                                       QueryConditionOperatorType.In,
                                                       subQuerySQLC1
                                                       );
                    }
                    else if (queryCriteria.C2SysNo.HasValue)
                    {
                        string subQuerySQLC1 = @"SELECT [Category3Sysno]
                        FROM  OverseaContentManagement.dbo.V_CM_CategoryInfo with(nolock)
                        WHERE [Category2Sysno] = " + queryCriteria.C2SysNo;

                        sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                                                       "a.Description",
                                                       QueryConditionOperatorType.In,
                                                       subQuerySQLC1
                                                       );
                    }
                }
                if (!string.IsNullOrEmpty(queryCriteria.ReferenceTypeIn) &&
                    string.Compare(queryCriteria.ReferenceTypeIn, "'DT11','DT12'") == 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.sysno",
                    DbType.Int32, "@sysno",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.SysNo);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ReferenceType",
                    DbType.String, "@ReferenceType",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.ReferenceType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ReferenceContent",
                    DbType.String, "@ReferenceContent",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.ReferenceContent);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status",
                    DbType.String, "@Status",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Description",
                    DbType.String, "@Description",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.Description);

                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                    "a.ReferenceType",
                    QueryConditionOperatorType.In,
                    queryCriteria.ReferenceTypeIn);
                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                queryCriteria.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(4, typeof(OrderCheckStatus));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        #endregion
    }
}
