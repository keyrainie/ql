using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBalanceRefundQueryDA))]
    public class BalanceRefundQueryDA : IBalanceRefundQueryDA
    {
        #region IBalanceRefundDA Members

        public DataSet Query(BalanceRefundQueryFilter filter, out int totalCount)
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

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryBalanceRefundList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "ReturnPrepay.SysNo DESC"))
            {
                AddParameter(filter, dataCommand, sqlBuilder);

                result = new DataSet();
                dataCommand.AddOutParameter("@TotalAmount", DbType.String, 50);

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("RefundPayType", typeof(RefundPayType));
                enumColumns.Add("Status", typeof(BalanceRefundStatus));

                DataTable resultDT = dataCommand.ExecuteDataTable(enumColumns);
                resultDT.TableName = "DataResult";
                result.Tables.Add(resultDT);

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                DataTable statisticDT = new DataTable("StatisticResult");
                statisticDT.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("TotalAmount",typeof(decimal))
                });
                statisticDT.Rows.Add(dataCommand.GetParameterValue("@TotalAmount"));
                result.Tables.Add(statisticDT);
            }
            return result;
        }

        #endregion IBalanceRefundDA Members

        private void AddParameter(BalanceRefundQueryFilter filter, CustomDataCommand dataCommand, DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.SysNo",
               DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.CreateTime",
                DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateTimeFrom);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.CreateTime",
                DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThanOrEqual, filter.CreateTimeTo);
            if (!string.IsNullOrEmpty(filter.CustomerID))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("Customer.CustomerID like '{0}%'", filter.CustomerID));
            }
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.Status",
                DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.RefundPayType",
                DbType.Int32, "@RefundPayType", QueryConditionOperatorType.Equal, filter.RefundType);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.BankName",
                DbType.String, "@BankName", QueryConditionOperatorType.Like, filter.Bank);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.BranchBankName",
                DbType.String, "@BranchBankName", QueryConditionOperatorType.Like, filter.BranchBank);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.CardOwnerName",
                DbType.String, "@CardOwnerName", QueryConditionOperatorType.Like, filter.CardOwner);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.CompanyCode",
               DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.CSAuditTime",
                DbType.DateTime, "@CSAuditTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CSAuditTimeFrom);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.CSAuditTime",
                DbType.DateTime, "@CSAuditTimeTo", QueryConditionOperatorType.LessThanOrEqual, filter.CSAuditTimeTo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.AuditTime",
                DbType.DateTime, "@AuditTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.FinAuditTimeFrom);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.AuditTime",
                DbType.DateTime, "@AuditTimeTo", QueryConditionOperatorType.LessThanOrEqual, filter.FinAuditTimeTo);
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReturnPrepay.ReferenceID",
                DbType.String, "@ReferenceID", QueryConditionOperatorType.Like, filter.ReferenceID);

            dataCommand.CommandText = sqlBuilder.BuildQuerySql();
        }

        private void MapSortField(BalanceRefundQueryFilter filter)
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
                        filter.PagingInfo.SortBy = sortField.Replace("SysNo", "ReturnPrepay.SysNo");
                        break;
                    case "CustomerSysNo":
                        filter.PagingInfo.SortBy = sortField.Replace("CustomerSysNo", "ReturnPrepay.CustomerSysNo");
                        break;
                    case "CustomerID":
                        filter.PagingInfo.SortBy = sortField.Replace("CustomerID", "Customer.CustomerID");
                        break;
                    case "CustomerName":
                        filter.PagingInfo.SortBy = sortField.Replace("CustomerName", "Customer.CustomerName");
                        break;

                    case "RefundType":
                        filter.PagingInfo.SortBy = sortField.Replace("RefundType", "ReturnPrepay.RefundPayType");
                        break;
                    case "RefundAmt":
                        filter.PagingInfo.SortBy = sortField.Replace("RefundAmt", "ReturnPrepay.ReturnPrepayAmt");
                        break;
                    case "Status":
                        filter.PagingInfo.SortBy = sortField.Replace("Status", "ReturnPrepay.Status");
                        break;
                    case "CreateUser":
                        filter.PagingInfo.SortBy = sortField.Replace("CreateUser", "UserInfo1.DisplayName");
                        break;
                    case "CreateTime":
                        filter.PagingInfo.SortBy = sortField.Replace("CreateTime", "ReturnPrepay.CreateTime");
                        break;
                    case "CSAuditUser":
                        filter.PagingInfo.SortBy = sortField.Replace("CSAuditUser", "UserInfo3.DisplayName");
                        break;
                    case "CSAuditTime":
                        filter.PagingInfo.SortBy = sortField.Replace("CSAuditTime", "ReturnPrepay.CSAuditTime");
                        break;
                    case "FinAuditUser":
                        filter.PagingInfo.SortBy = sortField.Replace("FinAuditUser", "UserInfo2.DisplayName");
                        break;
                    case "FinAuditTime":
                        filter.PagingInfo.SortBy = sortField.Replace("FinAuditTime", "ReturnPrepay.AuditTime");
                        break;
                    case "Note":
                        filter.PagingInfo.SortBy = sortField.Replace("Note", "ReturnPrepay.Note");
                        break;
                    case "Bank":
                        filter.PagingInfo.SortBy = sortField.Replace("Bank", "ReturnPrepay.BankName");
                        break;
                    case "BranchBank":
                        filter.PagingInfo.SortBy = sortField.Replace("BranchBank", "ReturnPrepay.BranchBankName");
                        break;
                    case "CardNumber":
                        filter.PagingInfo.SortBy = sortField.Replace("CardNumber", "ReturnPrepay.CardNumber");
                        break;
                    case "CardOwner":
                        filter.PagingInfo.SortBy = sortField.Replace("CardOwner", "ReturnPrepay.CardOwnerName");
                        break;
                    case "PostAddress":
                        filter.PagingInfo.SortBy = sortField.Replace("PostAddress", "ReturnPrepay.PostAddress");
                        break;
                    case "PostCode":
                        filter.PagingInfo.SortBy = sortField.Replace("PostCode", "ReturnPrepay.PostCode");
                        break;
                    case "CashReceiver":
                        filter.PagingInfo.SortBy = sortField.Replace("CashReceiver", "ReturnPrepay.ReceiverName");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}