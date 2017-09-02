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
    [VersionExport(typeof(IBalanceAccountQueryDA))]
    public class BalanceAccountQueryDA : IBalanceAccountQueryDA
    {
        #region IBalanceAccountQueryDA Members

        public DataSet Query(BalanceAccountQueryFilter filter, out int totalCount)
        {
            DataSet result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (filter.PagingInfo != null)
            {
                MapSortField(filter);

                pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
                pagingInfo.SortField = filter.PagingInfo.SortBy;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryBalanceAccount");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "CP.SysNo desc"))
            {
                StringBuilder t1 = new StringBuilder();
                StringBuilder t2 = new StringBuilder();
                StringBuilder t3 = new StringBuilder();

                t1.Append("WHERE PL.Status = @Status AND PL.CompanyCode = @CompanyCode ");
                t2.Append("WHERE PL.Status = @Status AND PL.CompanyCode = @CompanyCode ");
                t3.Append("WHERE PL.Status = @Status AND PL.CompanyCode = @CompanyCode ");

                if (filter.CustomerSysNo.HasValue)
                {
                    t1.Append(" AND PL.CustomerSysNo = @CustomerSysNo ");
                    t2.Append(" AND PL.CustomerSysNo = @CustomerSysNo ");
                    t3.Append(" AND PL.CustomerSysNo = @CustomerSysNo ");
                }

                if (!string.IsNullOrEmpty(filter.CustomerID))
                {
                    t1.Append(" AND C.CustomerID = @CustomerID ");
                    t2.Append(" AND C.CustomerID = @CustomerID ");
                    t3.Append(" AND C.CustomerID = @CustomerID ");
                }

                if (filter.CreateTimeFrom.HasValue)
                {
                    t1.Append(" AND PL.CreateTime < @CreateDateFrom ");
                    t3.Append(" AND PL.CreateTime > @CreateDateFrom ");
                }

                if (filter.CreateTimeTo.HasValue)
                {
                    t2.Append(" AND PL.CreateTime <= @CreateDateTo ");
                    t3.Append(" AND PL.CreateTime <= @CreateDateTo ");
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CP.CustomerSysNo",
                     DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.CustomerID",
                     DbType.String, "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CP.CreateTime",
                   DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateTimeFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CP.CreateTime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThanOrEqual, filter.CreateTimeTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CP.CompanyCode",
                       DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CP.Status",
                       DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, 'A');

                if (filter.DetailType.HasValue)
                {
                    if (filter.DetailType.Value > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cp.PrepayAmt", DbType.Decimal, "@PrepayAmt", QueryConditionOperatorType.MoreThanOrEqual, 0);
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cp.PrepayAmt", DbType.Decimal, "@PrepayAmt", QueryConditionOperatorType.LessThan, 0);
                    }
                }

                dataCommand.AddOutParameter("@StartBalance", DbType.Double, 50);
                dataCommand.AddOutParameter("@EndBalance", DbType.Double, 50);
                dataCommand.AddOutParameter("@PayedIn", DbType.Double, 50);
                dataCommand.AddOutParameter("@PayedOut", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandText = dataCommand.CommandText.Replace("#ITWHERE01#", t1.ToString());
                dataCommand.CommandText = dataCommand.CommandText.Replace("#ITWHERE02#", t2.ToString());
                dataCommand.CommandText = dataCommand.CommandText.Replace("#ITWHERE03#", t3.ToString());

                result = ExecuteDataTable(dataCommand, filter, out totalCount);
            }
            return result;
        }

        #endregion IBalanceAccountQueryDA Members

        private void MapSortField(BalanceAccountQueryFilter filter)
        {
            if (filter.PagingInfo != null && !string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                var index = 0;
                index = filter.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = filter.PagingInfo.SortBy.Substring(0, filter.PagingInfo.SortBy.Length - index);
                var sortField = filter.PagingInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("SysNo", "CP.SysNo");
                        break;
                    case "CreateTimeStr":
                        filter.PagingInfo.SortBy = sortField.Replace("CreateTimeStr", "CP.CreateTime");
                        break;
                    case "CustomerID":
                        filter.PagingInfo.SortBy = sortField.Replace("CustomerID", "C.CustomerID");
                        break;
                    case "CustomerSysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("CustomerSysNo", "CP.CustomerSysNo");
                        break;
                    case "prepayTypeDesc":
                        filter.PagingInfo.SortBy = sortField.Replace("prepayTypeDesc", "CP.prepayType");
                        break;
                    case "OrderSysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("OrderSysNo", "CP.OrderSysNo");
                        break;
                    case "PrepayAmt":
                        filter.PagingInfo.SortBy = sortField.Replace("PrepayAmt", "CP.PrepayAmt");
                        break;
                    case "PayTypeName":
                        filter.PagingInfo.SortBy = sortField.Replace("PayTypeName", "P.PayTypeName");
                        break;
                }
            }
        }

        private DataSet ExecuteDataTable(CustomDataCommand dataCommand, BalanceAccountQueryFilter filter, out int totalCount)
        {
            DataSet result = new DataSet();
            EnumColumnList enumColumns = new EnumColumnList();
            enumColumns.Add("PrepayType", typeof(BalanceAccountPrepayType));

            DataTable resultDT = dataCommand.ExecuteDataTable(enumColumns);
            resultDT.TableName = "DataResult";
            result.Tables.Add(resultDT);

            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            #region 计算统计结果

            decimal TotalStartBalance = Convert.ToDecimal(dataCommand.GetParameterValue("@StartBalance"));
            decimal TotalEndBalance = Convert.ToDecimal(dataCommand.GetParameterValue("@EndBalance"));
            decimal PayedIn = Convert.ToDecimal(dataCommand.GetParameterValue("@PayedIn"));
            decimal PayedOut = Convert.ToDecimal(dataCommand.GetParameterValue("@PayedOut"));

            if (resultDT != null && resultDT.Rows.Count > 0)
            {
                //没有开始时间时每位客户的期初以及所有客户期初的总额都为0.00
                if (!filter.CreateTimeFrom.HasValue)
                {
                    TotalStartBalance = 0.00M;
                    foreach (DataRow row in resultDT.Rows)
                    {
                        row["StartBalance"] = 0.00M;
                    }
                }
            }
            DataTable statisticDT = new DataTable("StatisticResult");
            statisticDT.Columns.AddRange(new DataColumn[]
                    {
                        new DataColumn("TotalStartBalance",typeof(decimal)),
                        new DataColumn("TotalEndBalance",typeof(decimal)),
                        new DataColumn("PayedIn",typeof(decimal)),
                        new DataColumn("PayedOut",typeof(decimal))
                    });
            statisticDT.Rows.Add(TotalStartBalance, TotalEndBalance, PayedIn, PayedOut);
            result.Tables.Add(statisticDT);

            #endregion 计算统计结果

            return result;
        }
    }
}