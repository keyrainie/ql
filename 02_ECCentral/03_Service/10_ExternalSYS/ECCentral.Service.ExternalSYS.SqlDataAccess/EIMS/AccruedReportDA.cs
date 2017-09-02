using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.ExternalSYS.IDataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IAccruedReportDA))]
    public class AccruedReportDA : IAccruedReportDA
    {
        #region 应计返利报表查询（周期）
        public DataTable AccruedByPeriod(AccruedQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_AccruedByPeriod");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "accrued.AccruePeriod ASC"))
            {
                AddAccruedByPeriodParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddAccruedByPeriodParameters(AccruedQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            string dateConvert = string.Empty;
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "accrued.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            if (filter.CycleStart.HasValue)
            {
                dateConvert = filter.CycleStart.Value.Year.ToString() + filter.CycleStart.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleStart",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    dateConvert);
            }
            if (filter.CycleEnd.HasValue)
            {
                dateConvert = filter.CycleEnd.Value.Year.ToString() + filter.CycleEnd.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleEnd",
                    QueryConditionOperatorType.LessThanOrEqual,
                    dateConvert);
            }
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 应计返利报表查询（供应商）
        public DataTable AccruedByVendor(AccruedQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_AccruedByVendor");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "accrued.AccruePeriod ASC"))
            {
                AddAccruedByVendorParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddAccruedByVendorParameters(AccruedQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            string dateConvert = string.Empty;
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "accrued.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            if (filter.CycleStart.HasValue)
            {
                dateConvert = filter.CycleStart.Value.Year.ToString() + filter.CycleStart.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleStart",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    dateConvert);
            }
            if (filter.CycleEnd.HasValue)
            {
                dateConvert = filter.CycleEnd.Value.Year.ToString() + filter.CycleEnd.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleEnd",
                    QueryConditionOperatorType.LessThanOrEqual,
                    dateConvert);
            }
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.VendorNumber",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (filter.EndBalance == null ||
                (filter.EndBalance.HasValue && filter.EndBalance == 0))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.EndBalance",
                     DbType.Int32,
                     "@EndBalance",
                     QueryConditionOperatorType.NotEqual, 0);
            }
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 应计返利报表查询（合同）
        public DataTable AccruedByRule(AccruedQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_AccruedByRule");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "accrued.AccruePeriod ASC,accrued.AssignedCode ASC"))
            {
                AddAccruedByRuleParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("EIMSType", "ExternalSYS", "EIMSType");
                codeNameColList.Add("RuleStatus", "ExternalSYS", "RuleStatus");
                cmd.ConvertColumn(dt, enumColList, codeNameColList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddAccruedByRuleParameters(AccruedQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            string dateConvert = string.Empty;
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "accrued.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            if (filter.CycleStart.HasValue)
            {
                dateConvert = filter.CycleStart.Value.Year.ToString() + filter.CycleStart.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleStart",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    dateConvert);
            }
            if (filter.CycleEnd.HasValue)
            {
                dateConvert = filter.CycleEnd.Value.Year.ToString() + filter.CycleEnd.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleEnd",
                    QueryConditionOperatorType.LessThanOrEqual,
                    dateConvert);
            }
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.VendorNumber",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.EIMSType",
                    DbType.String,
                    "@EIMSType",
                    QueryConditionOperatorType.Equal,
                    filter.EIMSType);
            }
            if (!string.IsNullOrEmpty(filter.RuleNo))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleInfo.AssignedCode",
                    DbType.String,
                    "@RuleNo",
                    QueryConditionOperatorType.Equal,
                    filter.RuleNo);
            }
            if (filter.EndBalance == null ||
                (filter.EndBalance.HasValue && filter.EndBalance == 0))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.EndBalance",
                     DbType.Int32,
                     "@EndBalance",
                     QueryConditionOperatorType.NotEqual, 0);
            }
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 应计返利报表查询（PM）
        public DataTable AccruedByPM(AccruedQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_AccruedByPM");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "accrued.AccruePeriod ASC,accrued.PM ASC"))
            {
                AddAccruedByPMParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddAccruedByPMParameters(AccruedQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            string dateConvert = string.Empty;
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "accrued.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            if (filter.CycleStart.HasValue)
            {
                dateConvert = filter.CycleStart.Value.Year.ToString() + filter.CycleStart.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleStart",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    dateConvert);
            }
            if (filter.CycleEnd.HasValue)
            {
                dateConvert = filter.CycleEnd.Value.Year.ToString() + filter.CycleEnd.Value.Month.ToString().PadLeft(2, '0');
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.AccruePeriod",
                    DbType.String,
                    "@CycleEnd",
                    QueryConditionOperatorType.LessThanOrEqual,
                    dateConvert);
            }
            if (filter.PMUserSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accrued.PM",
                    DbType.Int32,
                    "@PM",
                    QueryConditionOperatorType.Equal,
                    filter.PMUserSysNo);
            }
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

    }
}
