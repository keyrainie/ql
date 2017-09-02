using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ECCentral.BizEntity.RMA;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRefundQueryDA))]
    public class RefundQueryDA : IRefundQueryDA
    {
        #region IRefundQueryDA Members

        public DataTable QueryRefund(RefundQueryFilter filter, out int totalCount)
        {
            if (!string.IsNullOrEmpty(filter.PagingInfo.SortBy))
            {
                string sortCondition = filter.PagingInfo.SortBy.Trim();

                Match match = Regex.Match(sortCondition, @"^(?<SortColumn>[\S]+)(?:\s+(?<SortType>ASC|DESC))?$", RegexOptions.IgnoreCase);
                if (match.Groups["SortColumn"].Success)
                {
                    string sortColumn = match.Groups["SortColumn"].Value;
                    string sortType = match.Groups["SortType"].Success ? match.Groups["SortType"].Value : "DESC";

                    switch (sortColumn)
                    {
                        case "RefundID":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.RefundID", sortType);
                            break;
                        case "SOSysNo":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.SOSysNo", sortType);
                            break;
                        case "SOInvoiceNo":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.SOInvoiceNo", sortType);
                            break;
                        case "CreateTime":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.CreateTime", sortType);
                            break;
                        case "AuditTime":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.AuditTime", sortType);
                            break;
                        case "RefundTime":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.RefundTime", sortType);
                            break;
                        case "RefundStatus":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "RMA.Status", sortType);
                            break;
                        case "AuditStatus":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "D.Status", sortType);
                            break;
                        case "Customer":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "E.CustomerName", sortType);
                            break;
                        case "ShippedWarehouse":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "G.ShippedWarehouse", sortType);
                            break;
                        case "InvoiceWarehouse":
                            filter.PagingInfo.SortBy = String.Format("{0} {1}", "F.StockName", sortType);
                            break;
                    }
                }
            }

            string commandName = (!string.IsNullOrEmpty(filter.SOSysNoString)) ?
                "QueryRefundWithSoSysNOParam" :
                "QueryRefund";
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig(commandName);
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "RMA.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "RMA.RefundID", DbType.String,
                    "@RefundID", QueryConditionOperatorType.Like,
                    filter.RefundID);
                builder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND,
                    "RMA.CreateTime", DbType.DateTime, "@CreateTime",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    QueryConditionOperatorType.LessThan,
                    filter.CreateTimeFrom,
                    filter.CreateTimeTo);
                builder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND,
                    "RMA.RefundTime", DbType.DateTime,
                    "@RefundTime", QueryConditionOperatorType.MoreThanOrEqual,
                    QueryConditionOperatorType.LessThan,
                    filter.RefundTimeFrom,
                    filter.RefundTimeTo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMA.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMA.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status", DbType.Int32, "@AuditStatus", QueryConditionOperatorType.Equal, filter.AuditStatus);
                if (filter.ProductSysNo != null)
                {
                    ConditionConstructor subQueryBuilder = builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, "SELECT TOP 1 1 FROM dbo.RMA_Register WITH (NOLOCK) INNER JOIN RMA_Refund_item WITH (NOLOCK) ON RMA_Register.[SysNo] = RMA_Refund_item.[RegisterSysNo]");
                    subQueryBuilder.AddCustomCondition(QueryConditionRelationType.AND, "RMA_Refund_item.[RefundSysNo] = RMA.[SysNo]");
                    subQueryBuilder.AddCondition(QueryConditionRelationType.AND, "ProductSysNo", DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);
                }
                if (filter.IsVIP != null)
                {
                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "E.VIPRank", DbType.Int32, "@VIPRank1", QueryConditionOperatorType.Equal, filter.IsVIP.Value ? VIPRank.VIPAuto : VIPRank.NormalAuto);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "E.VIPRank", DbType.Int32, "@VIPRank2", QueryConditionOperatorType.Equal, filter.IsVIP.Value ? VIPRank.VIPManual : VIPRank.NormalManual);
                    builder.ConditionConstructor.EndGroupCondition();
                }
                if (filter.WarehouseCreated != null && filter.WarehouseCreated.Trim() != String.Empty)
                {
                    ConditionConstructor subQueryBuilder = builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, "SELECT TOP 1 1 FROM dbo.RMA_Register  WITH (NOLOCK) INNER JOIN RMA_Refund_item WITH (NOLOCK) ON RMA_Register.[SysNo] = RMA_Refund_item.[RegisterSysNo]");
                    subQueryBuilder.AddCustomCondition(QueryConditionRelationType.AND, "RMA_Refund_item.[RefundSysNo] = RMA.[SysNo]");
                    subQueryBuilder.AddCondition(QueryConditionRelationType.AND, "LocationWarehouse", DbType.AnsiStringFixedLength, "@LocationWarehouse", QueryConditionOperatorType.Equal, filter.WarehouseCreated);
                }
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMA.InvoiceLocation", DbType.AnsiStringFixedLength, "@InvoiceLocation", QueryConditionOperatorType.Equal, filter.InvoiceLocation);
                if (filter.WarehouseShipped != null && filter.WarehouseShipped != String.Empty)
                {
                    ConditionConstructor subQueryBuilder = builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist, "SELECT TOP 1 1 FROM dbo.RMA_Register  WITH (NOLOCK) INNER JOIN RMA_Refund_item WITH (NOLOCK) ON RMA_Register.[SysNo] = RMA_Refund_item.[RegisterSysNo]");
                    subQueryBuilder.AddCustomCondition(QueryConditionRelationType.AND, "RMA_Refund_item.[RefundSysNo] = RMA.[SysNo]");
                    subQueryBuilder.AddCondition(QueryConditionRelationType.AND, "ShippedWarehouse", DbType.AnsiStringFixedLength, "@ShippedWarehouse", QueryConditionOperatorType.Equal, filter.WarehouseShipped);
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMA.[CompanyCode]", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                if (!string.IsNullOrEmpty(filter.SOSysNoString))
                {
                    command.AddInputParameter("@SoSysNoStr", DbType.AnsiString,
                        filter.SOSysNoString);
                }

                command.CommandText = builder.BuildQuerySql();

                command.ReplaceParameterValue("#OrderType#", SOIncomeOrderType.RO);

                DataTable dt = command.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(RMARefundStatus)},
                    { "AuditStatus", typeof(RefundStatus)} 
                });

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable GetWaitingRegisters(int? soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryWaitingRegisterForRefund");

            if (soSysNo == null)
            {
                List<int> waitingSO = ObjectFactory<IRefundDA>.Instance.GetWaitingSOForRefund();
                if (waitingSO != null && waitingSO.Count > 0)
                {
                    string sql = "C.[SOSysNo] IN(";

                    waitingSO.ForEach(item => sql += item + ",");

                    sql = sql.TrimEnd(',');
                    sql += ")";

                    command.ReplaceParameterValue("#SOSysNo#", sql);
                }
                else
                {
                    command.ReplaceParameterValue("#SOSysNo#", "C.[SOSysNo] IS NULL");
                }
            }
            else
            {
                command.ReplaceParameterValue("#SOSysNo#", "C.[SOSysNo] = @SOSysNo");
            }

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@RefundStatus", RMARefundStatus.Abandon);

            return command.ExecuteDataTable();
        }

        public DataTable GetRefundPrintDetail(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundPrintDetail");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteDataTable();
        }

        public DataTable GetRefundPrintItems(int refundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundPrintItems");
            command.SetParameterValue("@RefundSysNo", refundSysNo);

            return command.ExecuteDataTable();
        }
        #endregion
    }
}
