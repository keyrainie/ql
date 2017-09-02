using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IRefundAdjustQueryDA))]
    public class RefundAdjustQueryDA : IRefundAdjustQueryDA
    {
        #region 查询补偿退款单
        public DataTable RefundAdjustQuery(RefundAdjustQueryFilter filter, out int totalCount)
        {
            MapSortField(filter);
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Query_RefundAdjust");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "R.[SysNo] DESC"))
            {
                AddRefundAdjustParameters(filter, cmd, sb);
                //EnumColumnList colList = new EnumColumnList();
                //colList.Add("AdjustType", typeof(RefundAdjustType));
                //colList.Add("RefundPayType", typeof(RefundPayType));
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddRefundAdjustParameters(RefundAdjustQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "R.CompanyCode",
                DbType.StringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
            if (!string.IsNullOrEmpty(filter.SysNo))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.SysNo",
                    DbType.String,
                    "@SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.SysNo);
            }
            if (filter.CreateDateFrom.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.CreateTime",
                    DbType.DateTime,
                    "@CreateTimeFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.CreateDateFrom);
            }
            if (filter.CreateDateTo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.CreateTime",
                    DbType.DateTime,
                    "@CreateTimeTo",
                    QueryConditionOperatorType.LessThan,
                    filter.CreateDateTo);
            }
            if (filter.RefundDateFrom.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.RefundTime",
                    DbType.DateTime,
                    "@RefundTimeFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.RefundDateFrom);
            }
            if (filter.RefundDateTo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.RefundTime",
                    DbType.DateTime,
                    "@RefundTimeTo",
                    QueryConditionOperatorType.LessThan,
                    filter.RefundDateTo);
            }
            if (!string.IsNullOrEmpty(filter.CustomerID))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "c.customerid",
                    DbType.String,
                    "@CustomerID",
                    QueryConditionOperatorType.Equal,
                    filter.CustomerID);
            }
            if (filter.AdjustType.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.AdjustOrderType",
                    DbType.Int32,
                    "@AdjustOrderType",
                    QueryConditionOperatorType.Equal,
                    filter.AdjustType);
            }
            if (!string.IsNullOrEmpty(filter.SoSysNo))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.SOSysNo",
                    DbType.String,
                    "@SOSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.SoSysNo);
            }
            if (filter.RefundPayType.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.RefundPayType",
                    DbType.Int32,
                    "@RefundPayType",
                    QueryConditionOperatorType.Equal,
                    filter.RefundPayType);
            }
            if (filter.RefundAdjustStatus.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.Status",
                    DbType.Int32,
                    "@Status",
                    QueryConditionOperatorType.Equal,
                    filter.RefundAdjustStatus);
            }
            if (!string.IsNullOrEmpty(filter.RequestID))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ref.RequestID",
                    DbType.String,
                    "@RequestID",
                    QueryConditionOperatorType.Equal,
                    filter.RequestID);
            }
            cmd.CommandText = sb.BuildQuerySql();

        }
        #endregion

        #region 节能补贴相关

        /// <summary>
        /// 导出节能补贴
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DataTable QueryEnergySubsidyExport(RefundAdjustQueryFilter filter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_EnergySubsidyExport");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(command, "R.SysNo DESC"))
            {
                AddEnergySubsidyExportParameters(filter, command, sqlBuilder);
                EnumColumnList colList = new EnumColumnList();
                DataTable dt = command.ExecuteDataTable();

                colList.Add("Status", typeof(RefundAdjustStatus));
                command.ConvertEnumColumn(dt, colList);
                return dt;
            }
        }

        private void AddEnergySubsidyExportParameters(RefundAdjustQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.SysNo", DbType.String, "@SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.SysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "c.CustomerID", DbType.String, "@CustomerID",
                QueryConditionOperatorType.Equal,
                filter.CustomerID);
            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "R.SOSysNo", DbType.Int32, "@SOSysNo",
               QueryConditionOperatorType.Equal,
               filter.SoSysNo);


            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "R.Status", DbType.String, "@Status",
                QueryConditionOperatorType.Equal,
                filter.RefundAdjustStatus);


            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "R.RefundPayType", DbType.String, "@RefundPayType",
                QueryConditionOperatorType.Equal,
                filter.RefundPayType);

            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "R.AdjustOrderType", DbType.String, "@AdjustOrderType",
               QueryConditionOperatorType.Equal,
               filter.AdjustType);


            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "R.CreateTime", DbType.DateTime, "@DateForm",
                  QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "R.CreateTime", DbType.DateTime, "@DateTo",
                QueryConditionOperatorType.LessThan, filter.CreateDateTo);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "R.RefundTime", DbType.DateTime, "@RefundDateFrom",
                  QueryConditionOperatorType.MoreThanOrEqual, filter.RefundDateFrom);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "R.RefundTime", DbType.DateTime, "@RefundDateTo",
                QueryConditionOperatorType.LessThan, filter.RefundDateTo);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "ProductInfo.VendorID", DbType.String, "@VendorID",
                QueryConditionOperatorType.Equal, filter.VendorID);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "ProductInfo.ProductID", DbType.String, "@ProductID",
                QueryConditionOperatorType.Equal, filter.ProductID);

            cmd.CommandText = sqlBuilder.BuildQuerySql();
        }

        /// <summary>
        /// 节能补贴查询  增加ProductID 和 VendorID
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryEnergySubsidy(RefundAdjustQueryFilter filter, out int totalCount)
        {
            MapSortFieldForEnergySubsidy(filter);
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_EnergySubsidySimple");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingEntity, "Result.SysNo DESC"))
            {
                AddEnergySubsidyParameters(filter, command, sqlBuilder);
                DataTable dt = command.ExecuteDataTable();
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddEnergySubsidyParameters(RefundAdjustQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "R.SysNo", DbType.String, "@SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.SysNo);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "c.CustomerID", DbType.String, "@CustomerID",
                QueryConditionOperatorType.Equal,
                filter.CustomerID);
            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "R.SOSysNo", DbType.Int32, "@SOSysNo",
               QueryConditionOperatorType.Equal,
               filter.SoSysNo);


            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "R.Status", DbType.String, "@Status",
                QueryConditionOperatorType.Equal,
                filter.RefundAdjustStatus);


            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "R.RefundPayType", DbType.String, "@RefundPayType",
                QueryConditionOperatorType.Equal,
                filter.RefundPayType);

            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "R.AdjustOrderType", DbType.String, "@AdjustOrderType",
               QueryConditionOperatorType.Equal,
               filter.AdjustType);


            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "R.CreateTime", DbType.DateTime, "@DateForm",
                  QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "R.CreateTime", DbType.DateTime, "@DateTo",
                QueryConditionOperatorType.LessThan, filter.CreateDateTo);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "R.RefundTime", DbType.DateTime, "@RefundDateFrom",
                  QueryConditionOperatorType.MoreThanOrEqual, filter.RefundDateFrom);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "R.RefundTime", DbType.DateTime, "@RefundDateTo",
                QueryConditionOperatorType.LessThan, filter.RefundDateTo);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "ProductInfo.VendorID", DbType.String, "@VendorID",
                QueryConditionOperatorType.Equal, filter.VendorID);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "ProductInfo.ProductID", DbType.String, "@ProductID",
                QueryConditionOperatorType.Equal, filter.ProductID);

            cmd.CommandText = sqlBuilder.BuildQuerySql();
        }
        #endregion

        #region 排序列映射
        private static void MapSortField(RefundAdjustQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortFiled = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("SysNo", "R.SysNo");
                        break;
                    case "SOSysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("SOSysNo", "R.SOSysNo");
                        break;
                    case "RequestID":
                        filter.PagingInfo.SortBy = sortFiled.Replace("RequestID", "ref.RequestID");
                        break;
                    case "CustomerID":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CustomerID", "c.customerid");
                        break;
                    case "CashAmt":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CashAmt", "R.CashAmt");
                        break;
                    case "CreateTime":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CreateTime", "R.CreateTime");
                        break;
                    case "RefundTime":
                        filter.PagingInfo.SortBy = sortFiled.Replace("RefundTime", "R.RefundTime");
                        break;
                    case "RefundPayType":
                        filter.PagingInfo.SortBy = sortFiled.Replace("RefundPayType", "R.RefundPayType");
                        break;
                    case "AdjustType":
                        filter.PagingInfo.SortBy = sortFiled.Replace("AdjustType", "R.AdjustOrderType");
                        break;
                    case "Status":
                        filter.PagingInfo.SortBy = sortFiled.Replace("Status", "R.Status");
                        break;
                }
            }
        }

        //节能补贴排序字段映射
        private static void MapSortFieldForEnergySubsidy(RefundAdjustQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortFiled = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("SysNo", "Result.SysNo");
                        break;
                    case "SOSysNo":
                        filter.PagingInfo.SortBy = sortFiled.Replace("SOSysNo", "Result.SOSysNo");
                        break;
                    case "CustomerID":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CustomerID", "Result.customerid");
                        break;
                    case "CashAmt":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CashAmt", "Result.CashAmt");
                        break;
                    case "CreateTime":
                        filter.PagingInfo.SortBy = sortFiled.Replace("CreateTime", "Result.CreateTime");
                        break;
                    case "RefundTime":
                        filter.PagingInfo.SortBy = sortFiled.Replace("RefundTime", "Result.RefundTime");
                        break;
                    case "RefundPayType":
                        filter.PagingInfo.SortBy = sortFiled.Replace("RefundPayType", "Result.RefundPayType");
                        break;
                    case "AdjustType":
                        filter.PagingInfo.SortBy = sortFiled.Replace("AdjustType", "Result.AdjustOrderType");
                        break;
                    case "Status":
                        filter.PagingInfo.SortBy = sortFiled.Replace("Status", "Result.Status");
                        break;
                    case "RequestID":
                        filter.PagingInfo.SortBy = sortFiled.Replace("RequestID", "Result.SysNo");
                        break;
                }
            }
        }
        #endregion
    }
}
