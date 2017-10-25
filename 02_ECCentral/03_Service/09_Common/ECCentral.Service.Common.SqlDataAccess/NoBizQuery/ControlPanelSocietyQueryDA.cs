using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;


namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IControlPanelSocietyQueryDA))]
    public class ControlPanelSocietyQueryDA
    {
        public DataTable QuerySociety(ControlPanelSocietyQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCPSocietys");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "c.InDate Desc"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "LoginName", DbType.String,
                    "@LoginName", QueryConditionOperatorType.Equal,
                    filter.LoginName);

                if (!string.IsNullOrEmpty(filter.DisplayName))
                    filter.DisplayName = EncodeSpecialChars(filter.DisplayName);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DisplayName", DbType.String,
                                                          "@DisplayName", QueryConditionOperatorType.Like,
                                                          filter.DisplayName);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DepartmentCode", DbType.AnsiStringFixedLength,
                                                          "@DepartmentCode", QueryConditionOperatorType.Equal,
                                                          filter.DepartmentCode);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.Status", DbType.String,
                                                          "@Status", QueryConditionOperatorType.Equal,
                                                          filter.Status.ToString());

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ControlPanelSocietyStatus));
                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                codeNameList.Add("DepartmentCode", CommonConst.DomainName_Common, CommonConst.Key_ControlPanelDept);
                DataTable dt = command.ExecuteDataTable(enumList, codeNameList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private static string EncodeSpecialChars(string value)
        {
            var result = value.Replace("[", "[[]");
            result = result.Replace("_", "[_]");
            result = result.Replace("%", "[%]");

            return result;
        }
    }
}
