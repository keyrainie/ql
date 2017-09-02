using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.PO;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICostChangeQueryDA))]
    public class CostChangeQueryDA : ICostChangeQueryDA
    {
        #region ICostChangeQueryDA Members

        public System.Data.DataTable QueryCostChangeList(CostChangeQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            #region

             var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCostChangeList");

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            pagingInfo.SortField = queryFilter.PageInfo.SortBy;
            pagingInfo.StartRowIndex =  queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize;
            pagingInfo.MaximumRows = queryFilter.PageInfo.PageSize;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, pagingInfo, "c.SysNo DESC"))
            {
                if (queryFilter.VendorSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "c.VendorSysNo", DbType.String, "@VendorSysNo",
                        QueryConditionOperatorType.Equal, queryFilter.VendorSysNo.Value);
                }

                if (queryFilter.PMSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "c.PMSysNo", DbType.String, "@PMSysNo",
                        QueryConditionOperatorType.Equal, queryFilter.PMSysNo.Value);
                }

                if (!string.IsNullOrEmpty(queryFilter.Memo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "c.Memo", DbType.String, "@Memo",
                        QueryConditionOperatorType.Like, queryFilter.Memo.Trim());
                }

                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "c.Status", DbType.Int32, "@Status",
                        QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                customCommand.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                customCommand.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                var result = customCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(customCommand.GetParameterValue("@TotalCount"));
                return result;
            }

            #endregion
        }

        #endregion ICostChangeQueryDA Members
    }
}
