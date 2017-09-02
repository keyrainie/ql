using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBatchManagementQueryDA))]

    public class BatchManagementQueryDA : IBatchManagementQueryDA
    {
        #region IAdventProductsQueryDA Members

        public System.Data.DataTable QueryAdventProductsList(QueryFilter.Inventory.AdventProductsQueryFilter queryFilter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryFilter.PageInfo.SortBy;
            pagingEntity.MaximumRows = queryFilter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryAdventProductsList");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "ring.SysNo DESC"))
            {
                if (queryFilter.BrandSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ring.BrandSysNo", DbType.Int32, "@BrandSysNo", QueryConditionOperatorType.Equal, queryFilter.BrandSysNo);
                }
                if (queryFilter.CategoryC3SysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ring.C3SysNo", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, queryFilter.CategoryC3SysNo);
                }
                cmd.CommandText = sb.BuildQuerySql();
                var resultData = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }

        #endregion
    }
}
