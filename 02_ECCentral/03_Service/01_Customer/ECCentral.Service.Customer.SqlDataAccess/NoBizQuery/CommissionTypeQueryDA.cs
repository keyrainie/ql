using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICommissionTypeQueryDA))]
    public class CommissionTypeQueryDA : ICommissionTypeQueryDA
    {
        public DataTable QueryCommissionType(CommissionTypeQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCommissionType");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal,
                    filter.SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CommissionTypeName", DbType.String,
                    "@CommissionTypeName", QueryConditionOperatorType.Like,
                    filter.CommissionTypeName);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "CommissionTypeID", DbType.String,
                   "@CommissionTypeID", QueryConditionOperatorType.Like,
                   filter.CommissionTypeID);

                //if (filter.IsOnlineShow != null)
                //    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //      "IsOnlineShow", DbType.Int32,
                //      "@IsOnlineShow", QueryConditionOperatorType.Equal,
                //      filter.IsOnlineShow.GetHashCode());

                //builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //    "CompanyCode", DbType.String,
                //    "@CompanyCode", QueryConditionOperatorType.Equal,
                //    "8601");

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("IsOnlineShow", typeof(HYNStatus));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        #region 扩展
        public DataTable SocietyCommissionQuery(CommissionTypeQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SocietyCommissionQuery");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal,
                    filter.SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "OrganizationID", DbType.String,
                    "@OrganizationID", QueryConditionOperatorType.Like,
                    filter.OrganizationID);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "OrganizationName", DbType.String,
                   "@OrganizationName", QueryConditionOperatorType.Like,
                   filter.OrganizationName);
                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("IsOnlineShow", typeof(HYNStatus));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        #endregion
    }
}
