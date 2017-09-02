using ECommerce.Entity.Common;
using ECommerce.Entity.Invoice;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.Invoice
{
    public class FinancialReportDA
    {
        /// <summary>
        /// Table[0]: Result,
        /// Table[1]: Statistics
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static SalesStatisticsReport SalesStatisticsReportQuery(SalesStatisticsReportQueryFilter filter)
        {

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("FinancialReportSalesStatisticsReportQuery");
            DataSet result = null;
            int totalCount = 0;
            SalesStatisticsReport adjustInfo = new SalesStatisticsReport();
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, filter, string.IsNullOrEmpty(filter.SortFields) ? "B.ProductID DESC, A.PayTypeSysNo" : filter.SortFields))
            {
                #region Set dynamic codition for where

                if (filter.SOStatusList != null && filter.SOStatusList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition<int>(
                        QueryConditionRelationType.AND,
                        "SOStatus",
                        System.Data.DbType.Int32,
                        filter.SOStatusList);
                }

                if (filter.SODateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.OrderDate",
                        DbType.DateTime,
                        "@OrderDateFrom_query",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.SODateFrom.Value);
                }

                if (filter.SODateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.OrderDate",
                        DbType.DateTime,
                        "@SODateTo_query",
                        QueryConditionOperatorType.LessThan,
                        filter.SODateTo.Value);
                }


                if (filter.C1SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.C1SysNo",
                        DbType.Int32,
                        "@C1SysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.C1SysNo);
                }

                if (filter.C2SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.C2SysNo",
                        DbType.Int32,
                        "@C2SysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.C2SysNo);
                }

                if (filter.C3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.C3SysNo",
                        DbType.Int32,
                        "@C3SysNo_query",
                        QueryConditionOperatorType.Equal,
                        filter.C3SysNo);
                }

                if (!string.IsNullOrWhiteSpace(filter.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rpt.ProductID",
                        DbType.StringFixedLength,
                        "@ProductID_query",
                        QueryConditionOperatorType.Equal,
                        filter.ProductID);
                }
                if (!string.IsNullOrWhiteSpace(filter.BrandName))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format(" (brand.BrandName_Ch = N'{0}' OR brand.BrandName_En = N'{0}') ", filter.BrandName.Replace("'", "''")));
                }

                sqlBuilder.ConditionConstructor.AddInCondition(
                       QueryConditionRelationType.AND,
                       "rpt.VendorSysNo",
                       DbType.Int32,
                       filter.VendorSysNoList);

                sqlBuilder.ConditionConstructor.AddInCondition(
                       QueryConditionRelationType.AND,
                       "rpt.WarehouseNumber",
                       DbType.StringFixedLength,
                       filter.WarehouseNumberList);

                #endregion

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteDataSet();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }

            adjustInfo.SalesStatisticsResult.PageInfo = new PageInfo()
            {
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                SortBy = filter.SortFields
            };
            adjustInfo.SalesStatisticsResult.ResultList = new List<SalesStatistics>();

            if (null != result && result.Tables.Count > 0)
            {
                if (result.Tables.Count >= 1)
                {
                    DataTable mainInfoDt = result.Tables[0];
                    if (mainInfoDt.Rows.Count > 0)
                    {
                        
                        foreach (DataRow dr in mainInfoDt.Rows)
                        {
                            SalesStatistics aa = DataMapper.GetEntity<SalesStatistics>(dr);
                            adjustInfo.SalesStatisticsResult.ResultList.Add(aa);
                        }
                    }
                }
                if (result.Tables.Count >= 2)
                {
                    DataTable itemsInfoDt = result.Tables[1];
                    if (itemsInfoDt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in itemsInfoDt.Rows)
                        {
                            adjustInfo.CostReportStatisticList.Add(DataMapper.GetEntity<IncomeCostReportStatistic>(dr));
                        }
                    }
                }
            }

            return adjustInfo;


        }

        
    }
}
