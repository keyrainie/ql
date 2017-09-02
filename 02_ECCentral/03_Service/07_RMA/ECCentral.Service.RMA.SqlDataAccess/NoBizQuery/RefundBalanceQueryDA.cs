using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IRefundBalanceQueryDA))]
    public class RefundBalanceQueryDA : IRefundBalanceQueryDA
    {
        #region IRefundBalanceQueryDA Members

        public DataTable Query(RefundBalanceQueryFilter queryFilter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryFilter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryFilter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRefundBalance");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, null, "RB.[SysNo] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RB.[OrgRefundSysNo]", DbType.Int32, "@RefundSysNo", QueryConditionOperatorType.Equal, queryFilter.RefundSysNo);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                cmd.ReplaceParameterValue("#OrderType#", SOIncomeOrderType.RO_Balance);

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(RefundBalanceStatus));
                enumList.Add("AuditStatus", typeof(RefundStatus));
                DataTable dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }


        #endregion
    }
}
