using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICanBePayOrderQueryDA))]
    public class CanBePayOrderQueryDA : ICanBePayOrderQueryDA
    {
        #region IPOOrderQueryDA Members

        public DataTable Query(CanBePayOrderQueryFilter filter, out int totalCount)
        {
            if (!filter.OrderType.HasValue)
            {
                return GetAllCanbePayList(filter, out totalCount);
            }

            switch (filter.OrderType)
            {
                case PayableOrderType.PO:
                    return GetPOCanbePayList(filter, out totalCount);
                case PayableOrderType.VendorSettleOrder:
                    return GetVendorSettleCanbePayList(filter, out totalCount);
                default:
                    throw new ArgumentOutOfRangeException("Invalid OrderType");
            }
        }

        #endregion IPOOrderQueryDA Members

        private DataTable GetAllCanbePayList(CanBePayOrderQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetAllCanBePayList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "OrderSysNo desc"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                StringBuilder sb1 = new StringBuilder();
                sb1.Append(" WHERE 1=1");
                sb1.Append(" AND po.IsConsign <> 1");
                StringBuilder sb2 = new StringBuilder();
                sb2.Append(" WHERE 1=1");

                sb1.Append(" AND po.CompanyCode = @CompanyCode");
                sb2.Append(" AND vendorsettle.CompanyCode = @CompanyCode");
                dataCommand.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);

                if (!string.IsNullOrEmpty(filter.OrderID))
                {
                    sb1.Append(" AND po.SysNo LIKE @OrderID");
                    sb2.Append(" AND vendorsettle.SettleID LIKE @OrderID");
                    dataCommand.AddInputParameter("@OrderID", DbType.String, filter.OrderID.Trim() + "%");
                }
                dataCommand.CommandText = dataCommand.CommandText
                   .Replace("#StrWhere1#", sb1.ToString())
                   .Replace("#StrWhere2#", sb2.ToString());

                result = ExecuteDataTable(dataCommand, out totalCount);
            }
            return result;
        }

        private DataTable GetVendorSettleCanbePayList(CanBePayOrderQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetVendorSettleCanBePayList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "vendorSettle.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorsettle.SettleID",
                   DbType.String, "@SettleID", QueryConditionOperatorType.Like, filter.OrderID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorsettle.Status",
                   DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.VendorSettleStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendorsettle.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataTable(dataCommand, out totalCount);
            }
            return result;
        }

        private DataTable GetPOCanbePayList(CanBePayOrderQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOCanBePayList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "po.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.IsConsign",
                   DbType.Int32, "@IsConsign", QueryConditionOperatorType.NotEqual, 1);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.POID",
                   DbType.String, "@POID", QueryConditionOperatorType.Like, filter.OrderID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.ETP",
                   DbType.DateTime, "@ETPFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.POETPFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.ETP",
                   DbType.DateTime, "@ETPTo", QueryConditionOperatorType.LessThan, filter.POETPTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.Status",
                   DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.POStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "po.CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = ExecuteDataTable(dataCommand, out totalCount);
            }
            return result;
        }

        private DataTable ExecuteDataTable(CustomDataCommand dataCommand, out int totalCount)
        {
            EnumColumnList enumColumns = new EnumColumnList();
            enumColumns.Add("OrderType", typeof(PayableOrderType));

            var result = dataCommand.ExecuteDataTable(enumColumns);
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            return result;
        }

        /// <summary>
        /// 构造PagingInfo对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private PagingInfoEntity CreatePagingInfo(CanBePayOrderQueryFilter query)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }
            return pagingInfo;
        }
    }
}