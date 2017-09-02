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
    [VersionExport(typeof(IReceivedReportDA))]
    public class ReceivedReportDA : IReceivedReportDA
    {
        #region 年度收款报表查询
        public DataTable ReceiveByYearQuery(ReceivedReportQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_ReceiveByYear");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "vendor.SysNo ASC"))
            {
                AddReceiveByYearParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddReceiveByYearParameters(ReceivedReportQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "accbyrule.CompanyCode",
                DbType.AnsiStringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accbyrule.VendorNumber",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "accbyrule.EIMSType",
                    DbType.String,
                    "@EIMSType",
                    QueryConditionOperatorType.Equal,
                    filter.EIMSType);
            }
            if (filter.Year.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "substring(accbyrule.AccruePeriod,0,5)",
                    DbType.String,
                    "@Year",
                    QueryConditionOperatorType.Equal,
                    filter.Year);
            }
            cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
            cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 供应商对账单查询
        public DataTable ReceiveByVendorQuery(ReceivedReportQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_ReceiveByVendor");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "vendor.SysNo ASC"))
            {
                AddReceiveByVendorParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                EnumColumnList enumColList = new EnumColumnList();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("EIMSType", "ExternalSYS", "EIMSType");

                cmd.ConvertColumn(dt, enumColList, codeNameColList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddReceiveByVendorParameters(ReceivedReportQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "invoice.CompanyCode",
                DbType.AnsiStringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoice.VendorNumber",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (filter.PMUserSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ruleinfo.PM",
                    DbType.Int32,
                    "@PMUserSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.PMUserSysNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoice.EIMSType",
                    DbType.String,
                    "@EIMSType",
                    QueryConditionOperatorType.Equal,
                    filter.EIMSType);
            }
            if (filter.ExpiredDays.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "DATEDIFF(DAY,invoice.DueDate,GETDATE())",
                    DbType.Int32,
                    "@ExpriedDays",
                    QueryConditionOperatorType.MoreThan,
                    filter.ExpiredDays);
            }

            cmd.AddOutParameter("@SumNoReceivedAmount", DbType.Double, sizeof(double));
            cmd.AddOutParameter("@SumOverDueAmount", DbType.Double, sizeof(double));
            cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
            cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);
            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion

        #region 应收账单（单据）查询
        public DataTable ARReceiveQuery(ReceivedReportQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_ReportByItem");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "VendorNumber"))
            {
                cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
                cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);
                AddARReceiveParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddARReceiveParameters(ReceivedReportQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            string strSql = " WHERE ruleinfo.Status = 'O' AND invoice.Status = 'O' AND invoice.CompanyCode = @CompanyCode";
            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, filter.CompanyCode);
            if (filter.VendorSysNo.HasValue)
            {
                strSql += " AND invoice.VendorNumber = @VendorSysNo ";
                cmd.AddInputParameter("@VendorSysNo", DbType.Int32, filter.VendorSysNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                strSql += " AND invoice.EIMSType = @EIMSType ";
                cmd.AddInputParameter("@EIMSType", DbType.String, filter.EIMSType);
            }
            if (filter.ProductCategory1.HasValue)
            {
                strSql += " AND item.ItemCategory1 = @ItemCategory1 ";
                cmd.AddInputParameter("@ItemCategory1", DbType.Int32, filter.ProductCategory1);
            }

            if (filter.ProductCategory2.HasValue)
            {
                strSql += " AND item.ItemCategory2 = @ItemCategory2 ";
                cmd.AddInputParameter("@ItemCategory2", DbType.Int32, (filter.ProductCategory2));
            }

            cmd.CommandText = cmd.CommandText.Replace("#WhereStr1#", strSql);
            strSql += " AND item.AccruePeriod = @AccruePeriod";
            cmd.AddInputParameter("@AccruePeriod", DbType.String, GetEffectiveAccruePeriod());
            cmd.CommandText = cmd.CommandText.Replace("#WhereStr2#", strSql);
            cmd.AddOutParameter("@TotalCount", DbType.Int32, 4);
        }

        /// <summary>
        /// 获取有效的应计周期值
        /// </summary>
        /// <returns></returns>
        private string GetEffectiveAccruePeriod()
        {
            DateTime dateNow = DateTime.Now;
            if (dateNow.Day == 1)
            {
                dateNow = dateNow.AddDays(-1);
            }

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString().PadLeft(2, '0');
            return year + month;
        }
        #endregion

        #region 账单明细查询

        public DataTable ARReceiveDetialsQuery(ReceivedReportQueryFilter filter, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_InvoiceDetailByVendor");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "VendorNumber,InvoiceNumber"))
            {
                AddARReceiveDetialsParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private void AddARReceiveDetialsParameters(ReceivedReportQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "invoice.CompanyCode",
                DbType.AnsiStringFixedLength,
                "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "ruleinfo.Status",
                DbType.String,
                "@RuleStatus",
                QueryConditionOperatorType.Equal,
                "O");
            sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "invoice.Status",
                DbType.String,
                "@InvoiceStatus",
                QueryConditionOperatorType.Equal,
                "O");
            if (filter.VendorSysNo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoice.VendorNumber",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
            }
            if (!string.IsNullOrEmpty(filter.EIMSType))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "invoice.EIMSType",
                    DbType.String,
                    "@EIMSType",
                    QueryConditionOperatorType.Equal,
                    filter.EIMSType);
            }
            if (filter.ProductCategory1.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "item.ItemCategory1",
                    DbType.Int32,
                    "@ProductCategory1",
                    QueryConditionOperatorType.Equal,
                    filter.ProductCategory1);
            }
            if (filter.ProductCategory2.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "item.ItemCategory2",
                    DbType.Int32,
                    "@ProductCategory2",
                    QueryConditionOperatorType.Equal,
                    filter.ProductCategory2);
            }
            cmd.AddInputParameter("@PageSize", DbType.Int32, filter.PagingInfo.PageSize);
            cmd.AddInputParameter("@PageIndex", DbType.Int32, filter.PagingInfo.PageIndex);

            cmd.CommandText = sb.BuildQuerySql();
        }
        #endregion
    }
}
