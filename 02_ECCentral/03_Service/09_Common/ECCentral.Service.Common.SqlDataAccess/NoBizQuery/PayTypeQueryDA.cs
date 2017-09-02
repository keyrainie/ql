using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPayTypeQueryDA))]
    public class PayTypeQueryDA : IPayTypeQueryDA
    {
        #region IPayTypeQueryDA Members

        public System.Data.DataTable QueryPayType(QueryFilter.Common.PayTypeQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPayType");
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
                    "PayTypeName", DbType.String,
                    "@PayTypeName", QueryConditionOperatorType.Like,
                    filter.PayTypeName);

                if (filter.IsOnlineShow != null)
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "IsOnlineShow", DbType.Int32,
                      "@IsOnlineShow", QueryConditionOperatorType.Equal,
                      filter.IsOnlineShow.GetHashCode());

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CompanyCode", DbType.String,
                    "@CompanyCode", QueryConditionOperatorType.Equal,
                    "8601");

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("IsOnlineShow",typeof(HYNStatus));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
