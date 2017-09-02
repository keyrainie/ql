using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IConsignToAccountLogQueryDA))]
    public class ConsignToAccountLogQueryDA : IConsignToAccountLogQueryDA
    {
        #region IConsignToAccountLogQueryDA Members
          
        public System.Data.DataTable QueryConsignToAccountLog(QueryFilter.PO.ConsignToAccountLogQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsinToAccountLog");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "Consign.SysNo DESC"))
            {
                if (pagingInfo != null && (queryFilter.SysNoList == null || queryFilter.SysNoList.Count <= 0))
                {
                    if (queryFilter.CreateTimeFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime", DbType.DateTime,
                            "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateTimeFrom.Value);
                    }
                    if (queryFilter.CreateTimeTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime", DbType.DateTime,
                        "@CreateTimTo", QueryConditionOperatorType.LessThan, queryFilter.CreateTimeTo.Value.AddDays(1));
                    }
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.Product", DbType.String,
                        "@ProductName", QueryConditionOperatorType.Like, queryFilter.ProductName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.SysNo", DbType.Int32,
                        "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.StockSysNo", DbType.Int32,
                        "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.SettleType", DbType.String,
                        "@SettleType", QueryConditionOperatorType.Equal, queryFilter.SettleType == null ? null : queryFilter.SettleType.ToString());
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.VendorName", DbType.String,
                        "@VendorName", QueryConditionOperatorType.Like, queryFilter.VendorName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.VendorSysNo", DbType.Int32,
                        "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);

                    if (queryFilter.ConsignToAccType.HasValue)
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.ConsignToAccType", DbType.String,
                          "@ConsignToAccType", QueryConditionOperatorType.Equal, (int)queryFilter.ConsignToAccType.Value);

                    if (queryFilter.Status.HasValue)
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.Status", DbType.String,
                            "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
                }
                else if (queryFilter != null && queryFilter.SysNoList != null && queryFilter.SysNoList.Count > 0)
                {
                    builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND
                                  , "Consign.sysno", DbType.Int32, queryFilter.SysNoList);
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CompanyCode", DbType.String,
                        "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList
                {
                    { "Status", typeof(ConsignToAccountLogStatus) },
                    { "ReferenceType", typeof(ConsignToAccountType) },
                    { "SettleType", typeof(SettleType) }
                };
                dt = command.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                if (null != dt && dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["InDate"] != null && dt.Rows[i]["InDate"] != DBNull.Value)
                        {
                            dt.Rows[i]["InDateString"] = string.Format("{0}[{1}]", EnumHelper.GetDisplayText((ConsignToAccountType)Enum.Parse(typeof(ConsignToAccountType), dt.Rows[i]["ReferenceType"].ToString())), dt.Rows[i]["InDate"]);
                        }
                    }
                }


            }
            return dt;
        }

        public DataTable QueryConsignToAccountLogTotalAmt(QueryFilter.PO.ConsignToAccountLogQueryFilter queryFilter)
        {
            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryAccountLogTotalAmt");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
           {
               SortField = queryFilter.PageInfo.SortBy,
               StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
               MaximumRows = queryFilter.PageInfo.PageSize
           };
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "Consign.SysNo DESC"))
            {
                if (queryFilter != null && (queryFilter.SysNoList == null || queryFilter.SysNoList.Count <= 0))
                {
                    if (queryFilter.CreateTimeFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime", DbType.DateTime,
                            "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateTimeFrom.Value);
                    }
                    if (queryFilter.CreateTimeTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime", DbType.DateTime,
                        "@CreateTimTo", QueryConditionOperatorType.LessThan, queryFilter.CreateTimeTo.Value.AddDays(1));
                    }
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.Product", DbType.String,
                        "@ProductName", QueryConditionOperatorType.Like, queryFilter.ProductName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Product.SysNo", DbType.Int32,
                        "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.StockSysNo", DbType.Int32,
                        "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.SettleType", DbType.String,
                        "@SettleType", QueryConditionOperatorType.Equal, queryFilter.SettleType == null ? null : queryFilter.SettleType.ToString());
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Vendor.VendorName", DbType.String,
                        "@VendorName", QueryConditionOperatorType.Like, queryFilter.VendorName);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.VendorSysNo", DbType.Int32,
                        "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);

                    if (queryFilter.ConsignToAccType.HasValue)
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.ConsignToAccType", DbType.String,
                          "@ConsignToAccType", QueryConditionOperatorType.Equal, (int)queryFilter.ConsignToAccType.Value);

                    if (queryFilter.Status.HasValue)
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.Status", DbType.String,
                            "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
                }
                else if (queryFilter != null && queryFilter.SysNoList != null && queryFilter.SysNoList.Count > 0)
                {
                    builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND
                                               , "Consign.sysno", DbType.Int32, queryFilter.SysNoList);
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CompanyCode", DbType.String,
                      "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                command.CommandText = builder.BuildQuerySql();
                dt = command.ExecuteDataTable();

                return dt;
            }
        }

        #endregion

    }
}
