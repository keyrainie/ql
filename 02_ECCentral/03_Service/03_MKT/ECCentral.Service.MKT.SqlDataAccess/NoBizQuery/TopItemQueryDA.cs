using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ITopItemQuery))]
    public class TopItemQueryDA : ITopItemQuery
    {
        #region ITopItemQuery Members

        public System.Data.DataTable QueryTopItem(QueryFilter.MKT.TopItemFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("TopItemList_Query");
            cmd.SetParameterValue("@FrontPageSize", filter.FrontPageSize);
            cmd.SetParameterValue("@CategoryType", filter.PageType);
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "OrderPriority ASC,CreateTime DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.[Status]", DbType.String, "@Status", QueryConditionOperatorType.Equal, 1);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.[ProductType]", DbType.String, "@ProductType", QueryConditionOperatorType.Equal, 0);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.C1SysNo", DbType.String, "@C1SysNo", QueryConditionOperatorType.Equal, filter.C1SysNo);
                if (filter.PageType.HasValue)
                {
                    PageTypePresentationType pType = PageTypeUtil.ResolvePresentationType(ModuleType.TopItem, filter.PageType.Value.ToString());
                    if (pType == PageTypePresentationType.Category2)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.C2SysNo", DbType.String, "@C2SysNo", QueryConditionOperatorType.Equal, filter.RefSysNo);
                    else if (pType == PageTypePresentationType.Category3)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.C3SysNo", DbType.String, "@C3SysNo", QueryConditionOperatorType.Equal, filter.RefSysNo);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.[ProductID]", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
