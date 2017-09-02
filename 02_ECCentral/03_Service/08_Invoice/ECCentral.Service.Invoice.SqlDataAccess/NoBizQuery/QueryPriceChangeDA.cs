using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPriceChangeQueryDA))]
    public class QueryPriceChangeDA : IPriceChangeQueryDA
    {
        public DataTable QueryPriceChange(ChangePriceFilter query, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("PriceChange_QueryPriceChange");
            using (DynamicQuerySqlBuilder sql = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "SysNo DESC"))
            {
                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);

                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.ProductsysNo", DbType.Int32, "@ProductsysNo", QueryConditionOperatorType.Equal, query.ProductsysNo);

                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.Memo", DbType.String, "@Memo", QueryConditionOperatorType.Like, query.Memo);
                if(query.Status.HasValue)
                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);

                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.BeginDate", DbType.DateTime, "@BeginDate", QueryConditionOperatorType.Equal, query.BeginDate);

                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PC.EndDate", DbType.DateTime, "@EndDate", QueryConditionOperatorType.Equal, query.EndDate);


                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.C3SysNo", DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal, query.C3SysNo);

                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C3.C2SysNo", DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal, query.C2SysNo);

                sql.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C2.C1SysNo", DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal,
                    query.C1SysNo);

                cmd.CommandText = sql.BuildQuerySql();
                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("Status", typeof(RequestPriceStatus));

                result = cmd.ExecuteDataTable(enumColumns);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return result;
            }
        }

        public DataTable QUeryLastVendorSysNo()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PriceChange_GetLastVendorSysNo");
            return cmd.ExecuteDataTable();
        }
    }
}
