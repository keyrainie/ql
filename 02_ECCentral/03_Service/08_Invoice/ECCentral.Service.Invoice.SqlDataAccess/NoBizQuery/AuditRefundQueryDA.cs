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
    [VersionExport(typeof(IAuditRefundQueryDA))]
    public class AuditRefundQueryDA : IAuditRefundQueryDA
    {
        private const int TimeOut = 300;

        #region IAuditRefundQueryDA Members

        public DataTable Query(AuditRefundQueryFilter filter, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (filter.PagingInfo != null)
            {
                MapSortField(filter.PagingInfo);

                pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
                pagingInfo.SortField = filter.PagingInfo.SortBy;
            }

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetAuditRefundList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "SOIncomeBank.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[SysNo]", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.Id);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.AuditStatus);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[HaveAutoRMA]", DbType.Int32, "@HaveAutoRMA", QueryConditionOperatorType.Equal, filter.ShipRejected);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[OrderType]", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, filter.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[RefundPayType]", DbType.Int32, "@RefundType", QueryConditionOperatorType.Equal, filter.RefundPayType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[RefundReason]", DbType.Int32, "@RMAReason", QueryConditionOperatorType.Equal, filter.RMAReasonCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[OrderSysNo]", DbType.Int32, "@RMANumber", QueryConditionOperatorType.Equal, filter.RMANumber);

                if (!string.IsNullOrEmpty(filter.OrderNumber))
                {
                    List<int> OrderNumberList = new List<int>();
                    int[] OrderNumberArray = Array.ConvertAll<string, int>(filter.OrderNumber.Split('.'),
                        new Converter<string, int>((source) =>
                        {
                            return Convert.ToInt32(string.IsNullOrEmpty(source) ? "0" : source);
                        }));
                    OrderNumberList.AddRange(OrderNumberArray);
                    sqlBuilder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "SOIncomeBank.SOSysNo", DbType.Int32, OrderNumberList);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[CreateTime]", DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateTimeFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[CreateTime]", DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThanOrEqual, filter.CreateTimeTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[AuditTime]", DbType.DateTime, "@AuditTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.AuditTimeFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[AuditTime]", DbType.DateTime, "@AuditTimeTo", QueryConditionOperatorType.LessThanOrEqual, filter.AuditTimeTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMARefund.Status", DbType.Int32, "@RMAStatus", QueryConditionOperatorType.Equal, filter.RMAStatus);
                if (filter.CashRelated)
                {
                    using (GroupCondition group = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMARefund.[CashFlag]", DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                        sqlBuilder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.OR, "RMARefund.[CashFlag]", QueryConditionOperatorType.IsNull);
                    }
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncome.[Status]", DbType.Int32, "@RefundStatus", QueryConditionOperatorType.Equal, filter.RefundStatus);

                //为提高性能改为左匹配
                if (!string.IsNullOrEmpty(filter.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(" Customer.CustomerID like '{0}%'", filter.CustomerID));
                }

                switch (filter.OperationType)
                {
                    case OperationSignType.LessThanOrEqual:
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.RefundCashAmt", DbType.Decimal, "@RefundAmount", QueryConditionOperatorType.LessThanOrEqual, filter.RefundAmount);
                        break;
                    case OperationSignType.Equal:
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.RefundCashAmt", DbType.Decimal, "@RefundAmount", QueryConditionOperatorType.Equal, filter.RefundAmount);
                        break;
                    case OperationSignType.MoreThanOrEqual:
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.RefundCashAmt", DbType.Decimal, "@RefundAmount", QueryConditionOperatorType.MoreThanOrEqual, filter.RefundAmount);
                        break;
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOIncomeBank.[CompanyCode]", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPayExt.[Status]", DbType.Int32, "@WLTRefundStatus", QueryConditionOperatorType.Equal, filter.WLTRefundStatus);

                if (!string.IsNullOrEmpty(filter.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.payTypeSysNo",
                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(filter.PayTypeSysNo));
                }

                if (filter.IsVAT.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.IsVAT",
                        DbType.Int32, "@IsVAT", QueryConditionOperatorType.Equal, filter.IsVAT.Value);
                }

                if (filter.StockSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                          @"exists (SELECT TOP 1 1 FROM OverseaOrderManagement.dbo.V_OM_SO_Item ITEM WITH(NOLOCK)
	                      WHERE item.SOSysNo = SOMaster.SysNo
                          AND item.WarehouseNumber = @WareHouseNumber)");

                    cmd.AddInputParameter("@WareHouseNumber", DbType.Int32, filter.StockSysNo.Value);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                cmd.CommandTimeout = TimeOut;

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("AuditStatus", typeof(RefundStatus));
                enumColumns.Add("OrderType", typeof(RefundOrderType));
                enumColumns.Add("RefundStatus", typeof(SOIncomeStatus));
                enumColumns.Add("RefundPayType", typeof(RefundPayType));
                enumColumns.Add("Source", typeof(NetPaySource));

                result = cmd.ExecuteDataTable(enumColumns);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            }
            return result;
        }

        #endregion IAuditRefundQueryDA Members

        private void MapSortField(PagingInfo pagingInfo)
        {
            if (pagingInfo != null && !string.IsNullOrEmpty(pagingInfo.SortBy))
            {
                string sortBy = pagingInfo.SortBy.Trim();
                int trimLen = sortBy.Contains("asc") ? 4 : 5;
                string fieldName = sortBy.Substring(0, sortBy.Length - trimLen);
                string mapped = string.Empty;
                switch (fieldName)
                {
                    /* 请参考Refund.config
                     * SOIncomeBank -- [IPP3].[dbo].[Finance_SOIncome_BankInfo]
                     * TempResult   -- 连接了多表的CTE
                     */
                    case "Id":
                        mapped = "SOIncomeBank.SysNo";
                        break;
                    case "StatusCode":
                        mapped = "SOIncomeBank.Status";
                        break;
                    case "CustomerSysNo":
                        mapped = "SOIncomeBank.CustomerSysNo";
                        break;
                    case "OrderTypeCode":
                        mapped = "SOIncomeBank.OrderType";
                        break;
                    case "RMANumber":
                        mapped = "SOIncomeBank.OrderSysNo";
                        break;
                    case "RMAReasonCode":
                        mapped = "SOIncomeBank.RefundReason";
                        break;
                    case "RMAStatusCode":
                        mapped = "TempResult.RefundStatus";
                        break;
                    case "RefundPayTypeCode":
                        mapped = "SOIncomeBank.RefundPayType";
                        break;
                    case "RefundAmount":
                        mapped = "SOIncomeBank.RefundCashAmt";
                        break;
                    case "RefundGiftCard":
                        mapped = "SOIncomeBank.RefundGiftCard";
                        break;
                    case "RefundPoint":
                        mapped = "SOIncomeBank.RefundPoint";
                        break;
                    case "OrderNumber":
                        mapped = "SOIncomeBank.SOSysNo";
                        break;
                    case "PaySourceCode":
                        mapped = "TempResult.Source";
                        break;
                    case "PayTypeCode":
                        mapped = "TempResult.PayTypeSysNo";
                        break;
                    case "CreateUser":
                        mapped = "SOIncomeBank.CreateUserSysNo";
                        break;
                    case "CreateTime":
                        mapped = "SOIncomeBank.CreateTime";
                        break;
                    case "AuditUser":
                        mapped = "SOIncomeBank.AuditUserSysNo";
                        break;
                    case "AuditTime":
                        mapped = "SOIncomeBank.AuditTime";
                        break;
                    case "Memo":
                        mapped = "SOIncomeBank.Note";
                        break;
                    case "Bank":
                        mapped = "SOIncomeBank.BankName";
                        break;
                    case "BranchBank":
                        mapped = "SOIncomeBank.BranchBankName";
                        break;
                    case "CardNumber":
                        mapped = "SOIncomeBank.CardNumber";
                        break;
                    case "CardOwner":
                        mapped = "SOIncomeBank.CardOwnerName";
                        break;
                    case "PostAddress":
                        mapped = "SOIncomeBank.PostAddress";
                        break;
                    case "PostCode":
                        mapped = "SOIncomeBank.PostCode";
                        break;
                    case "CashReceiver":
                        mapped = "SOIncomeBank.ReceiverName";
                        break;
                }

                pagingInfo.SortBy = sortBy.Replace(fieldName, mapped);
            }
        }
    }
}