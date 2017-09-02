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

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.EIMS
{
    [VersionExport(typeof(IComprehensiveReportDA))]
    public class ComprehensiveReportDA : IComprehensiveReportDA
    {
        #region 查询EIMS单据
        /// <summary>
        /// 查询EIMS单据
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable EIMSInvoiceQuery(EIMSInvoiceQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd;
            if (filter.IsC1Summary == null
                || filter.IsC1Summary == false)
            {
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_EIMSInvoice");
            }
            else
            {
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_EIMSInvoiceBYC1Name");
            }

            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "vendor.SysNo DESC"))
            {
                AddEIMSInvoiceParameters(filter, sb, cmd);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("EIMSType", "ExternalSYS", "EIMSType");
                codeNameColList.Add("ReceiveType", "ExternalSYS", "ReceivedType");
                codeNameColList.Add("Status", "ExternalSYS", "InvoiceStatus");

                cmd.ConvertColumn(dt, enumColList, codeNameColList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddEIMSInvoiceParameters(EIMSInvoiceQueryFilter filter, DynamicQuerySqlBuilder sb, CustomDataCommand cmd)
        {
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "vendor.SysNo",
                    DbType.String,
                    "@SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (!string.IsNullOrEmpty(filter.VendorName))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "vendor.VendorName",
                    DbType.String,
                    "@VendorName",
                    QueryConditionOperatorType.Like,
                    filter.VendorName);
            }
            if (!string.IsNullOrEmpty(filter.RuleNo))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleMaster.AssignedCode",
                    DbType.String,
                    "@RuleNo",
                    QueryConditionOperatorType.Equal,
                    filter.RuleNo);
            }
            if (!string.IsNullOrEmpty(filter.InvoiceNo))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "invoiceMaster.AssignedCode",
                   DbType.String,
                   "@InvoiceNo",
                   QueryConditionOperatorType.Equal,
                   filter.InvoiceNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "invoiceMaster.EIMSType",
                   DbType.String,
                   "@EIMSType",
                   QueryConditionOperatorType.Equal,
                   filter.EIMSType);
            }
            if (!string.IsNullOrEmpty(filter.ReceivedType))
            {
                 sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoiceMaster.ReceiveType",
                    DbType.String,
                    "@ReceivedType",
                    QueryConditionOperatorType.Equal,
                    filter.ReceivedType);
            }
            if (filter.InvoiceApprovedStart.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoiceMaster.ApproveDate",
                    DbType.DateTime,
                    "@ApproveDateStart",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.InvoiceApprovedStart);
            }
            if (filter.InvocieApprovedEnd.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoiceMaster.ApproveDate",
                    DbType.DateTime,
                    "@ApproveDateEnd",
                    QueryConditionOperatorType.LessThanOrEqual,
                    filter.InvocieApprovedEnd);
            }
            if (!string.IsNullOrEmpty(filter.InvoiceStatus))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "invoiceMaster.Status",
                   DbType.String,
                   "@InvoiceStatus",
                   QueryConditionOperatorType.Equal,
                   filter.InvoiceStatus);
            }

            cmd.AddOutParameter("@SumNoReceivedAmount", DbType.Double, 50);
            object obj = cmd.GetParameterValue("@SumNoReceivedAmount");
            if (obj == null
                || Convert.IsDBNull(obj))
            {
                obj = 0;
            }
            cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
            cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);

            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion 

        #region 查询合同与对应单据
        /// <summary>
        /// 查询合同与对应单据
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>DataTable数据集合</returns>
        public DataTable UnbilledRuleListQuery(UnbilledRuleListQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_UnbilledRuleList");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "vendor.SysNo DESC"))
            {
                AddUnbilledRuleList(filter, sb, cmd);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("SettleType", "ExternalSYS", "SettleType");
                codeNameColList.Add("BillingCycle", "ExternalSYS", "BillingCycle");
                codeNameColList.Add("SettleWeeklyDay", "ExternalSYS", "DayOfWeek");

                cmd.ConvertColumn(dt, enumColList, codeNameColList);

                //添加 合同结算周期列 
                dt.Columns.Add("RuleBalanceCycle", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["SettleType"].ToString().Equals("手动"))
                    {
                        row["RuleBalanceCycle"] = row["BillingCycle"].ToString();
                    }
                    else if (row["SettleType"].ToString().Equals("自动"))
                    {
                        switch (row["BillingCycle"].ToString())
                        {
                            case "每周":
                                row["RuleBalanceCycle"] = row["SettleType"].ToString() + "," + row["BillingCycle"].ToString() + row["SettleWeeklyDay"].ToString();
                                break;
                            case "每月":
                                row["RuleBalanceCycle"] = row["SettleType"].ToString() + "," + row["BillingCycle"].ToString() + row["SettleMonthlyDay"].ToString() + "日";
                                break;
                            default:
                                row["RuleBalanceCycle"] = row["SettleType"].ToString() + "," + row["BillingCycle"].ToString();
                                break;
                        }
                    }
                }

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddUnbilledRuleList(UnbilledRuleListQueryFilter filter, DynamicQuerySqlBuilder sb, CustomDataCommand cmd)
        {
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vendor.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "vendor.SysNo",
                    DbType.String,
                    "@SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "ruleMaster.EIMSType",
                   DbType.String,
                   "@EIMSType",
                   QueryConditionOperatorType.Equal,
                   filter.EIMSType);
            }
            if (!string.IsNullOrEmpty(filter.PMUserSysNo))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleMaster.PM",
                    DbType.String,
                    "@PMSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.PMUserSysNo);
            }

            cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
            cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);

            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 综合报表查询
        /// <summary>
        /// 综合报表查询
        /// </summary>
        /// <param name="filter">查询条件结合</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>DataTable数据集合</returns>
        public DataTable ComprehensiveQuery(EIMSComprehensiveQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_Comprehensive");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "ruleRebate.EndDate ASC"))
            {
                AddComprehensive(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("EIMSType", "ExternalSYS", "EIMSType");
                codeNameColList.Add("RuleStatus", "ExternalSYS", "RuleStatus");
                codeNameColList.Add("InvoiceStatus", "ExternalSYS", "InvoiceStatus");
                codeNameColList.Add("RebateBaseType", "ExternalSYS", "RebateBaseType");

                cmd.ConvertColumn(dt, enumColList, codeNameColList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddComprehensive(EIMSComprehensiveQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ruleInfo.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoiceInfo.VendorNumber",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
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
            if (!string.IsNullOrEmpty(filter.InvoiceNo))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "invoiceInfo.AssignedCode",
                   DbType.String,
                   "@InvoiceNo",
                   QueryConditionOperatorType.Equal,
                   filter.InvoiceNo);
            }
            if (!string.IsNullOrEmpty(filter.RuleStatus))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleInfo.Status",
                    DbType.String,
                    "@RuleStatus",
                    QueryConditionOperatorType.Equal,
                    filter.RuleStatus);
            }
            if (!string.IsNullOrEmpty(filter.InvoiceStatus))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoiceInfo.Status",
                    DbType.String,
                    "@InvoiceStatus",
                    QueryConditionOperatorType.Equal,
                    filter.InvoiceStatus);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "invoiceInfo.EIMSType",
                   DbType.String,
                   "@EIMSType",
                   QueryConditionOperatorType.Equal,
                   filter.EIMSType);
            }

            if (filter.PMUserSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoiceInfo.PM",
                    DbType.Int32,
                    "@PMSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.PMUserSysNo);
            }
            if (filter.RuleApprovedStart.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleInfo.ApproveDate",
                    DbType.DateTime,
                    "@RuleApprovedStart",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.RuleApprovedStart);
            }
            if (filter.RuleApprovedEnd.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleInfo.ApproveDate",
                    DbType.DateTime,
                    "@RuleApprovedEnd",
                    QueryConditionOperatorType.LessThanOrEqual,
                    filter.RuleApprovedEnd);
            }
            cmd.CommandText = sb.BuildQuerySql();

            string StrWhereInner = string.Format(" WHERE CompanyCode = '{0}'",filter.CompanyCode);
            if (filter.CycleStart.HasValue)
            {
                StrWhereInner += string.Format(" AND AccruePeriod >='{0}'", filter.CycleStart);
            }

            if (filter.CycleEnd.HasValue)
            {
                StrWhereInner += string.Format(" AND AccruePeriod <='{0}'", filter.CycleEnd);
            }
            cmd.CommandText = cmd.CommandText.Replace("#StrWhereInner", StrWhereInner);
        }
        #endregion
    }
}
