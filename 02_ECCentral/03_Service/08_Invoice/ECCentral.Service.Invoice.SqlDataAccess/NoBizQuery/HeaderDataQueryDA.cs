using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IHeaderDataQueryDA))]
    public class HeaderDataQueryDA : IHeaderDataQueryDA
    {
        #region 查询上传SAP数据
        /// <summary>
        /// 查询上传SAP数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public DataTable QuerySAPDOCHeader(HeaderDataQueryFilter filter, out int TotalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetHeaderData");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, HelpDA.ToPagingInfo(filter.PagingInfo), "[InDate] DESC"))
            {
                AddSAPDOCHeaderParameters(filter, cmd, sb);
                DataTable dt = cmd.ExecuteDataTable();
                CodeNamePairColumnList codeNameColList = new CodeNamePairColumnList();
                codeNameColList.Add("DOC_TYPE", "Invoice", "DocumentType");

                cmd.ConvertCodeNamePairColumn(dt, codeNameColList);
                TotalCount = Convert.ToInt32(cmd.GetParameterValue("TotalCount"));

                return dt;
            }
        }

        private void AddSAPDOCHeaderParameters(HeaderDataQueryFilter filter, CustomDataCommand cmd, DynamicQuerySqlBuilder sb)
        {
            if (filter.UploadDateFrom.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "[PSTNG_DATE]",
                    DbType.DateTime,
                    "@UploadDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.UploadDateFrom);
            }
            if (filter.UploadDateTo.HasValue)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "[PSTNG_DATE]",
                    DbType.DateTime,
                    "@UploadDateTo",
                    QueryConditionOperatorType.LessThanOrEqual,
                    filter.UploadDateTo);
            }
            if (!string.IsNullOrEmpty(filter.DocType))
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "[DOC_TYPE]",
                    DbType.String,
                    "@DOC_TYPE",
                    QueryConditionOperatorType.Equal,
                    filter.DocType);
            }
            if (filter.SapCoCode != ResCommonEnum.Enum_All)
            {
                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "[COMP_CODE]",
                    DbType.String,
                    "@COMP_CODE",
                    QueryConditionOperatorType.Equal,
                    filter.SapCoCode);
            }
            cmd.CommandText = sb.BuildQuerySql();
        }

        #endregion

        #region 查询上传SAP数据明细
        /// <summary>
        /// 查询上传SAP数据明细
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="TotalCount"></param>
        /// <returns></returns>
        public DataTable QuerySAPDOCHeaderDetail(HeaderDataQueryFilter filter, out int TotalCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSAPHeaderDetail");
            command.SetParameterValue("@DOC_TYPE", filter.DocType);
            command.SetParameterValue("@COMP_CODE", filter.SapCoCode);
            command.SetParameterValue("@PSTNG_DATE", filter.UploadDate);
            TotalCount = Convert.ToInt32(command.GetParameterValue("TotalCount"));
            return command.ExecuteDataTable();
        }
        #endregion

        #region 查询公司代码(SAP)
        /// <summary>
        /// 查询公司代码(SAP)
        /// </summary>
        /// <returns></returns>
        public DataTable QueryCompany()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCompanyCode");
            return cmd.ExecuteDataTable();
        }
        #endregion
    }
}
