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
    [VersionExport(typeof(IPOVendorInvoiceQueryDA))]
    public class POVendorInvoiceQueryDA : IPOVendorInvoiceQueryDA
    {
        #region IPOVendorInvoiceQueryDA Members

        public DataTable QueryPOVendorInvoice(POVendorInvoiceQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PagingInfo.SortBy;
            pagingEntity.MaximumRows = query.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPOInvoiceList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "SysNo"))
            {
                #region Condition

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.VendorSysNo",
                DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, query.VendorSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InvoiceNumber",
                   DbType.AnsiString, "@InvoiceNumber", QueryConditionOperatorType.Like, query.InvoiceNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InvoiceTime",
               DbType.DateTime, "@InvoiceDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InvoiceDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InvoiceTime",
               DbType.DateTime, "@InvoiceDateTo", QueryConditionOperatorType.LessThan, query.InvoiceDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InputTime",
               DbType.DateTime, "@InvoiceCreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InvoiceCreateDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InputTime",
               DbType.DateTime, "@InvoiceCreateDateTo", QueryConditionOperatorType.LessThan, query.InvoiceCreateDateTo);

                if (query.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.Status",
                 DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                }
                else
                {
                    if (query.IsFilterAbandon.Value)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.Status",
                        DbType.Int32, "@Status", QueryConditionOperatorType.MoreThan, -1);
                    }
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.StockSysNo",
             DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, query.StockSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.CompanyCode",
    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                #endregion Condition

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(InvoiceStatus));
                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QueryTotalAmountByVendor(POVendorInvoiceQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PagingInfo.SortBy;
            pagingEntity.MaximumRows = query.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetTotalAmountByVendor");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "VendorSysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.VendorSysNo",
                DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, query.VendorSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InvoiceNumber",
                   DbType.AnsiString, "@InvoiceNumber", QueryConditionOperatorType.Like, query.InvoiceNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InvoiceTime",
               DbType.DateTime, "@InvoiceDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InvoiceDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InvoiceTime",
               DbType.DateTime, "@InvoiceDateTo", QueryConditionOperatorType.LessThan, query.InvoiceDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InputTime",
               DbType.DateTime, "@InvoiceCreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.InvoiceCreateDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.InputTime",
               DbType.DateTime, "@InvoiceCreateDateTo", QueryConditionOperatorType.LessThan, query.InvoiceCreateDateTo);

                if (query.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.Status",
                 DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                }
                else
                {
                    if (query.IsFilterAbandon.Value)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.Status",
                        DbType.Int32, "@Status", QueryConditionOperatorType.MoreThan, -1);
                    }
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vi.StockSysNo",
             DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, query.StockSysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion IPOVendorInvoiceQueryDA Members
    }
}