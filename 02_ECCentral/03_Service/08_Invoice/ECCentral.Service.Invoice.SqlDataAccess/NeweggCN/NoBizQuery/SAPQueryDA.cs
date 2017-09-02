using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.SqlDataAccess.NeweggCN.NoBizQuery
{
    [VersionExport(typeof(ISAPQueryDA))]
    public class SAPQueryDA : ISAPQueryDA
    {
        /// <summary>
        /// SAP供应商查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryVendor(SAPVendorQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PagingInfo.SortBy;
            pagingEntity.MaximumRows = query.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySAPVendor");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "SV.VendorSysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SV.VendorSysNo",
                     DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, query.VendorSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SV.CompanyCode",
                     DbType.StringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// SAP公司查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryCompany(SAPCompanyQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PagingInfo.SortBy;
            pagingEntity.MaximumRows = query.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySAPCompany");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "SC.WarehouseNumber desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SC.WarehouseNumber",
                     DbType.Int32, "@WarehouseNumber", QueryConditionOperatorType.Equal, query.StockID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SC.WarehouseName",
                     DbType.StringFixedLength, "@WarehouseName", QueryConditionOperatorType.Equal, query.StockName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SC.SapCoCode",
                     DbType.StringFixedLength, "@SapCoCode", QueryConditionOperatorType.Equal, query.SapCompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SC.CompanyCode",
                     DbType.StringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("WorkStatus", typeof(SAPStatus));
                DataTable dt = dataCommand.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// SAP确认人查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryIPPUser(SAPIPPUserQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PagingInfo.SortBy;
            pagingEntity.MaximumRows = query.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySAPIPPUser");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "I.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "I.PayType",
                    DbType.Int32, "@PayType", QueryConditionOperatorType.Equal, query.PayType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "I.CustID",
                    DbType.String, "@CustID", QueryConditionOperatorType.Equal, query.CustID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "I.SystemConfirmID",
                    DbType.String, "@SystemConfirmID", QueryConditionOperatorType.Equal, query.SystemConfirmID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "I.CompanyCode",
                    DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("Status", typeof(SAPStatus));
                DataTable dt = dataCommand.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
