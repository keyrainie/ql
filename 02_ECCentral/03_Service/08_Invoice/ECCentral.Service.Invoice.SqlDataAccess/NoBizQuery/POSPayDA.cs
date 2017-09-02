using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPOSPayQueryDA))]
    public class POSPayDA : IPOSPayQueryDA
    {
        #region IPOSPayDA Members

        public DataSet QueryPOSPayConfirmList(POSPayQueryFilter query, out int totalCount)
        {
            DataSet result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                MapSortField(query.PagingInfo);

                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPOSPayConfirmList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "b.SoSysNo desc"))
            {
                string OutTimeFieldName = string.Empty;
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.Source",
                  DbType.String, "@Source", QueryConditionOperatorType.Equal, "POS");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.POSTerminalID",
                  DbType.String, "@POSTerminalID", QueryConditionOperatorType.Equal, query.POSTerminalID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.PayedDate",
                  DbType.Date, "@PayedDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.PayedDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.PayedDate",
                  DbType.Date, "@PayedDateTo", QueryConditionOperatorType.LessThan, query.PayedDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.AttachInfo",
                  DbType.String, "@POSPayTypeCode", QueryConditionOperatorType.Equal, query.POSPayType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Status",
                  DbType.AnsiStringFixedLength, "@AutoConfirmStatus", QueryConditionOperatorType.Equal, query.AutoConfirmStatus);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "f.Status",
                  DbType.Int32, "@SOIncomeStatus", QueryConditionOperatorType.Equal, query.SOIncomeStatus);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.AttachInfo2",
                 DbType.String, "@CombineNumber", QueryConditionOperatorType.Equal, query.CombineNumber);

                if (!string.IsNullOrEmpty(query.SOSysNo))
                {
                    int sysNo;
                    var soSysNoList = query.SOSysNo.Split('.').ToList();
                    soSysNoList.RemoveAll(s => !int.TryParse(s, out sysNo));
                    if (soSysNoList.Count > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("b.SoSysNo IN({0})",
              string.Join(",", soSysNoList)));
                    }
                }

                //出库时间(考虑并单的出库时间)
                string fsWhere = string.Empty;
                string scWhere = string.Empty;
                string outTimeWhere = string.Empty;
                if (query.OutDateFrom.HasValue)
                {
                    fsWhere += " AND f.OutTime>=@SOOutDateFrom ";
                    scWhere += " AND g.MergeOutTime>=@SOOutDateFrom ";

                    dataCommand.AddInputParameter("@SOOutDateFrom", DbType.DateTime, query.OutDateFrom.Value);
                }

                if (query.OutDateTo.HasValue)
                {
                    fsWhere += " AND f.OutTime<@SOOutDateTo ";
                    scWhere += " AND g.MergeOutTime<@SOOutDateTo ";

                    dataCommand.AddInputParameter("@SOOutDateTo", DbType.DateTime, query.OutDateTo);
                }

                if (query.OutDateFrom.HasValue || query.OutDateTo.HasValue)
                {
                    outTimeWhere = " AND ((g.IsCombine=0 " + fsWhere + ") OR (g.IsCombine=1 " + scWhere + ")) ";
                }

                dataCommand.AddOutParameter("@OrderAmt", DbType.Double, 50);
                dataCommand.AddOutParameter("@PayedAmt", DbType.Double, 50);
                dataCommand.AddOutParameter("@ConfirmedAmt", DbType.Double, 50);
                dataCommand.AddOutParameter("@SOIncomeAmt", DbType.Double, 50);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandText = dataCommand.CommandText.Replace("/*#StrWhere1#*/", outTimeWhere.ToString());

                result = ExecuteDataCommand(dataCommand, out totalCount);
            }
            return result;
        }

        #endregion IPOSPayDA Members

        private DataSet ExecuteDataCommand(CustomDataCommand dataCommand, out int totalCount)
        {
            DataSet ds = new DataSet();
            DataTable resultDT = dataCommand.ExecuteDataTable(new EnumColumnList()
            {
              {"AutoConfirmStatus", typeof(AutoConfirmStatus)},
              {"POSPayType", typeof(POSPayType)},
              {"SOIncomeStatus", typeof(SOIncomeStatus)},
            });
            resultDT.TableName = "DataResult";
            ds.Tables.Add(resultDT);

            totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            #region 构造统计信息

            DataTable statisticDT = new DataTable("StatisticResultTable");
            statisticDT.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("StatisticType",typeof(StatisticType)),
                new DataColumn("OrderAmt",typeof(decimal)),
                new DataColumn("PayedAmt",typeof(decimal)),
                new DataColumn("ConfirmedAmt",typeof(decimal)),
                new DataColumn("SOIncomeAmt",typeof(decimal)),
                new DataColumn("DiffAmt",typeof(decimal))
            });

            #region 本页小计

            DataRow row = statisticDT.NewRow();
            row["StatisticType"] = StatisticType.Page;
            row["OrderAmt"] = resultDT.Select().Sum(r => Convert.ToDecimal(!r.IsNull("OrderAmt") ? r["OrderAmt"] : 0M));
            row["PayedAmt"] = resultDT.Select().Sum(r => Convert.ToDecimal(!r.IsNull("PayedAmt") ? r["PayedAmt"] : 0M));
            row["ConfirmedAmt"] = resultDT.Select("SOIncomeStatus=1").Sum(r => Convert.ToDecimal(!r.IsNull("ConfirmedAmt") ? r["ConfirmedAmt"] : 0M));
            row["SOIncomeAmt"] = resultDT.Select().Sum(r => Convert.ToDecimal(!r.IsNull("SOIncomeAmt") ? r["SOIncomeAmt"] : 0M));
            row["DiffAmt"] = (decimal)row["PayedAmt"] - (decimal)row["SOIncomeAmt"];
            statisticDT.Rows.Add(row);
            //row["OrderAmt"] = 0;
            //row["PayedAmt"] = 0;
            //row["ConfirmedAmt"] = 0;
            //row["SOIncomeAmt"] = 0;
            //row["DiffAmt"] = 0;
            #endregion 本页小计

            #region 全部总计

            row = statisticDT.NewRow();
            row["StatisticType"] = StatisticType.Total;
            row["OrderAmt"] = Convert.ToDecimal(!(dataCommand.GetParameterValue("OrderAmt") is DBNull) ? dataCommand.GetParameterValue("OrderAmt") : 0M);
            row["PayedAmt"] = Convert.ToDecimal(!(dataCommand.GetParameterValue("PayedAmt") is DBNull) ? dataCommand.GetParameterValue("PayedAmt") : 0M);
            row["ConfirmedAmt"] = Convert.ToDecimal(!(dataCommand.GetParameterValue("ConfirmedAmt") is DBNull) ? dataCommand.GetParameterValue("ConfirmedAmt") : 0M);
            row["SOIncomeAmt"] = Convert.ToDecimal(!(dataCommand.GetParameterValue("SOIncomeAmt") is DBNull) ? dataCommand.GetParameterValue("SOIncomeAmt") : 0M);
            row["DiffAmt"] = (decimal)row["PayedAmt"] - (decimal)row["SOIncomeAmt"];
            statisticDT.Rows.Add(row);
            //row["OrderAmt"] = 0;
            //row["PayedAmt"] = 0;
            //row["ConfirmedAmt"] = 0;
            //row["SOIncomeAmt"] = 0;
            //row["DiffAmt"] = 0;
            #endregion 全部总计

            ds.Tables.Add(statisticDT);

            #endregion 构造统计信息

            return ds;
        }

        private void MapSortField(PagingInfo pagingInfo)
        {
            if (!string.IsNullOrEmpty(pagingInfo.SortBy))
            {
                var index = 0;
                index = pagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = pagingInfo.SortBy.Substring(0, pagingInfo.SortBy.Length - index);
                var sortField = pagingInfo.SortBy;
                switch (sort)
                {
                    case "SOSysNo":
                        pagingInfo.SortBy = sortField.Replace("SOSysNo", "b.SOSysNo");
                        break;
                    case "CustomerSysNo":
                        pagingInfo.SortBy = sortField.Replace("CustomerSysNo", "c.CustomerSysNo");
                        break;
                    case "POSTerminalID":
                        pagingInfo.SortBy = sortField.Replace("POSTerminalID", "b.POSTerminalID");
                        break;
                    case "POSPayTypeCode":
                        pagingInfo.SortBy = sortField.Replace("POSPayTypeCode", "b.AttachInfo");
                        break;
                    case "PayedDate":
                        pagingInfo.SortBy = sortField.Replace("PayedDate", "b.PayedDate");
                        break;
                    case "AutoConfirmStatus":
                        pagingInfo.SortBy = sortField.Replace("AutoConfirmStatus", "a.Status");
                        break;
                    case "SOIncomeStatus":
                        pagingInfo.SortBy = sortField.Replace("SOIncomeStatus", "f.Status");
                        break;
                    case "OrderAmt":
                        pagingInfo.SortBy = sortField.Replace("OrderAmt", "f.OrderAmt");
                        break;
                    case "PayedAmount":
                        pagingInfo.SortBy = sortField.Replace("PayedAmount", "b.PayedAmt");
                        break;
                    case "PrePayAmount":
                        pagingInfo.SortBy = sortField.Replace("PrePayAmount", "a.PrepayAmt");
                        break;
                    case "GiftCardAmount":
                        pagingInfo.SortBy = sortField.Replace("GiftCardAmount", "f.GiftCardPayAmt");
                        break;
                    case "SOIncomeAmt":
                        pagingInfo.SortBy = sortField.Replace("SOIncomeAmt", "f.IncomeAmt");
                        break;
                    case "CombineNumber":
                        pagingInfo.SortBy = sortField.Replace("CombineNumber", "b.AttachInfo2");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}