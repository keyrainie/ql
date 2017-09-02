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
    [VersionExport(typeof(IHolidayQueryDA))]
    public class HolidayQueryDA:IHolidayQueryDA
    {
        #region IHolidayQueryDA Members

        public System.Data.DataTable QueryHoliday(QueryFilter.Common.HolidayQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetHoliday");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "HolidayDate DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "HolidayDate", DbType.String,
                    "@HolidayDate", QueryConditionOperatorType.Equal,
                    filter.HolidayDate);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "BlockedService", DbType.String,
                   "@BlockedService", QueryConditionOperatorType.Equal,
                   filter.BlockedService);
                if (filter.IsUntilNow==SYNStatus.Yes)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Holiday.CreateDate",
                   DbType.DateTime, "@StartDate", QueryConditionOperatorType.MoreThanOrEqual, new DateTime(System.DateTime.Now.Year, 1, 1));
                }
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "ShipTypeSysNo", DbType.String,
                "@ShipTypeSysNo", QueryConditionOperatorType.Equal,
                filter.ShipTypeSysNo);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("BlockedService", typeof(BlockedServiceType));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
