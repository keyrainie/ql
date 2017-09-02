using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IConsignSettleRulesQueryDA))]
    public class ConsignSettleRulesQueryDA : IConsignSettleRulesQueryDA
    {
        #region IConsignSettleRulesQueryDA Members

        public System.Data.DataTable QueryConsignSettleRules(QueryFilter.PO.SettleRuleQueryFilter queryFilter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryConsignSettleRuleList");

            #region Build Sql
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "csr.[CreateDate]  DESC"))
            {
                if (queryFilter != null)
                {
                    var condition = queryFilter;
                    if (condition.SysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "csr.SysNo",
                            DbType.Int32,
                            "@SysNo",
                            QueryConditionOperatorType.Equal,
                            condition.SysNo.Value);
                    }


                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "csr.CompanyCode",
                        DbType.AnsiStringFixedLength,
                        "@CompanyCode",
                        QueryConditionOperatorType.Equal,
                        queryFilter.CompanyCode);

                    if (condition.CreateDateTo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "csr.CreateDate",
                            DbType.DateTime,
                            "@CreateDateEnd",
                            QueryConditionOperatorType.LessThanOrEqual,
                            condition.CreateDateTo.Value.AddDays(1));
                    }
                    if (condition.CreateDateFrom.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "csr.CreateDate",
                            DbType.DateTime,
                            "@CreateDateStart",
                            QueryConditionOperatorType.MoreThanOrEqual,
                            condition.CreateDateFrom.Value);
                    }
                    if (condition.ProductSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "p.SysNo",
                            DbType.Int32,
                            "@ProductSysNo",
                            QueryConditionOperatorType.Equal,
                            condition.ProductSysNo.Value);
                    }
                    if (!string.IsNullOrEmpty(condition.SettleRuleName)
                        && Regex.IsMatch(condition.SettleRuleName, @"^[\u4e00-\u9fa5\w\-]+$"))
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "csr.SettleRuleName",
                            DbType.String,
                            "@SettleRuleName",
                            QueryConditionOperatorType.Like,
                            condition.SettleRuleName);
                    }
                    if (!string.IsNullOrEmpty(condition.SettleRuleCode)
                        && Regex.IsMatch(condition.SettleRuleCode, @"^[\u4e00-\u9fa5\w\-]+$"))
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "csr.SettleRulesCode",
                            DbType.String,
                            "@SettleRulesCode",
                            QueryConditionOperatorType.Equal,
                            condition.SettleRuleCode);
                    }
                    if (condition.Status.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "csr.Status",
                            DbType.String,
                            "@Status",
                            QueryConditionOperatorType.Equal,
                            (char)condition.Status.Value);
                    }
                    if (condition.VendorSysNo.HasValue)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "v.SysNo",
                            DbType.Int32,
                            "@VendorSysNo",
                            QueryConditionOperatorType.Equal,
                            condition.VendorSysNo.Value);
                    }
                    command.CommandText = builder.BuildQuerySql();
                }
            }
            #endregion

            dt = command.ExecuteDataTable();
            totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return dt;
        }

        #endregion
    }
}
