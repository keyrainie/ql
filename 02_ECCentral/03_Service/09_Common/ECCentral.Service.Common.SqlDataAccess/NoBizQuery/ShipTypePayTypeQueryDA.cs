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
    [VersionExport(typeof(IShipTypePayTypeQueryDA))]
    public class ShipTypePayTypeQueryDA : IShipTypePayTypeQueryDA
    {
        public DataTable QueryShipTypePayType(QueryFilter.Common.ShipTypePayTypeQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryShipTypePayType");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "sp.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal,
                    filter.SysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                     "ShipTypeSysNo", DbType.Int32,
                     "@ShipTypeSysNo", QueryConditionOperatorType.Equal,
                     filter.ShipTypeSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "PayTypeSysNo", DbType.Int32,
                    "@PayTypeSysNo", QueryConditionOperatorType.Equal,
                    filter.PayTypeSysNo);

                command.CommandText = builder.BuildQuerySql();

                DataTable dt = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
