using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IAreaDeliveryQueryDA))]
    public class AreaDeliveryQueryDA:IAreaDeliveryQueryDA
    {
        #region IAreaDeliveryQueryDA Members

        public System.Data.DataTable QueryAreaDelivery(QueryFilter.Common.AreaDeliveryQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetAreaDeliveryInfo");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "Priority"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "WHArea", DbType.Int32,
                    "@WHArea", QueryConditionOperatorType.Equal,
                    filter.WHArea);

                command.CommandText = builder.BuildQuerySql();

                DataTable dt = command.ExecuteDataTable();

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion


        public DataTable QueryWHArea()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryWarehouse");
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteDataTable();
        }
    }
}
