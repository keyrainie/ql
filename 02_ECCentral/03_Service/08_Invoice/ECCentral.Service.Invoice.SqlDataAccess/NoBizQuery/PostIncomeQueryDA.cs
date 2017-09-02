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
    [VersionExport(typeof(IPostIncomeQueryDA))]
    public class PostIncomeQueryDA : IPostIncomeQueryDA
    {
        #region IPostIncomeQueryDA Members

        public DataTable Query(PostIncomeQueryFilter query, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                MapSortField(query.PagingInfo);

                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPostIncomeList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "a.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SOSysNo",
                   DbType.Int32, "@SOSysNo", QueryConditionOperatorType.Equal, query.SOSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.IncomeAmt",
                  DbType.Decimal, "@IncomeAmt", QueryConditionOperatorType.Equal, query.IncomeAmt);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateDate",
                  DbType.Date, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateDate",
                  DbType.Date, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.IncomeDate",
                  DbType.Date, "@IncomeDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.IncomeDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.IncomeDate",
                  DbType.Date, "@IncomeDateTo", QueryConditionOperatorType.LessThan, query.IncomeDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.PayBank",
                  DbType.String, "@PayBank", QueryConditionOperatorType.Like, query.PayBank);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.IncomeBank",
                  DbType.String, "@IncomeBank", QueryConditionOperatorType.Like, query.IncomeBank);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.DisplayName",
                  DbType.String, "@CreateUser", QueryConditionOperatorType.Like, query.CreateUser);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "e.DisplayName",
                  DbType.String, "@AuditUser", QueryConditionOperatorType.Like, query.AuditUser);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.PayUser",
                  DbType.String, "@PayUser", QueryConditionOperatorType.Like, query.PayUser);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                  DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (!string.IsNullOrEmpty(query.ConfirmedSOSysNoList))
                {
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "a.SysNo",
                        QueryConditionOperatorType.In, string.Format(@"SELECT a.PostIncomeSysNo FROM
         	        [OverseaInvoiceReceiptManagement].[dbo].[PostIncomeConfirm] a WITH(NOLOCK)
         	        WHERE a.ConfirmedSoSysNo IN ({0}) AND a.Status <> 'C'", query.ConfirmedSOSysNoList.Replace('.', ',')));
                }
                if (query.HandleStatus != null)
                {
                    switch (query.HandleStatus.Value)
                    {
                        case PostIncomeHandleStatusUI.UnConfirmed:
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ConfirmStatus",
                                DbType.Int32, "@ConfirmStatus", QueryConditionOperatorType.NotEqual, PostIncomeStatus.Confirmed);
                            break;
                        case PostIncomeHandleStatusUI.UnHandled:
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.HandleStatus",
                                DbType.Int32, "@HandleStatus", QueryConditionOperatorType.Equal, PostIncomeHandleStatus.WaitingHandle);
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ConfirmStatus",
                               DbType.Int32, "@ConfirmStatus", QueryConditionOperatorType.Equal, PostIncomeStatus.Confirmed);
                            break;
                        case PostIncomeHandleStatusUI.Handled:
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.HandleStatus",
                                DbType.Int32, "@HandleStatus", QueryConditionOperatorType.Equal, PostIncomeHandleStatus.Handled);
                            break;
                        default:
                            break;
                    }
                }
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("HandleStatus", typeof(PostIncomeHandleStatus));
                enumColumns.Add("ConfirmStatus", typeof(PostIncomeStatus));

                result = dataCommand.ExecuteDataTable(enumColumns);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return result;
        }

        #endregion IPostIncomeQueryDA Members

        private static void MapSortField(PagingInfo pagingInfo)
        {
            if (pagingInfo != null && !string.IsNullOrEmpty(pagingInfo.SortBy))
            {
                var index = 0;
                index = pagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = pagingInfo.SortBy.Substring(0, pagingInfo.SortBy.Length - index);
                var sortField = pagingInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        pagingInfo.SortBy = sortField.Replace("SysNo", "a.SysNo");
                        break;
                    case "OrderTime":
                        pagingInfo.SortBy = sortField.Replace("OrderTime", "d.OrderDate");
                        break;
                    case "CreateUser":
                        pagingInfo.SortBy = sortField.Replace("CreateUser", "b.DisplayName");
                        break;
                    case "ConfirmUser":
                        pagingInfo.SortBy = sortField.Replace("ConfirmUser", "c.DisplayName");
                        break;
                    case "AuditUser":
                        pagingInfo.SortBy = sortField.Replace("AuditUser", "e.DisplayName");
                        break;
                    case "AuditDate":
                        pagingInfo.SortBy = sortField.Replace("AuditDate", "a.ConfirmDate");
                        break;
                    case "OutTime":
                        pagingInfo.SortBy = sortField.Replace("OutTime", "d.OutTime");
                        break;
                    case "OrderSysNo":
                        pagingInfo.SortBy = sortField.Replace("OrderSysNo", "d.SysNo");
                        break;
                }
            }
        }
    }
}