using System;
using System.Data;
using System.Text.RegularExpressions;

using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IVendorSystemInfoDA))]
    public class VendorSystemInfoDA : IVendorSystemInfoDA
    {
        public DataTable LogQuery(VendorSystemQueryFilter filter, out int dataCount)
        {
            if (filter.PagingInfo.SortBy != null)
            {
                string sortCondition = filter.PagingInfo.SortBy.Trim();

                Match match = Regex.Match(sortCondition, @"^(?<SortColumn>[\S]+)(?:\s+(?<SortType>ASC|DESC))?$", RegexOptions.IgnoreCase);
                if (match.Groups["SortColumn"].Success)
                {
                    string sortColumn = match.Groups["SortColumn"].Value;
                    string sortType = match.Groups["SortType"].Success ?
                        match.Groups["SortType"].Value : "DESC";

                    switch (sortColumn)
                    {
                        case "Region":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "l.RegionSysNo", sortType);
                            break;
                        case "Category":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "l.CategorySysNo", sortType);
                            break;
                        case "Date":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "l.LogCreateDate", sortType);
                            break;
                        case "ReferenceKey":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "l.ReferenceKey", sortType);
                            break;
                        case "LogType":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "l.LogType", sortType);
                            break;
                    }
                }
            }

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("ExternalSYS_Query_VendorLogs");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
            command.CommandText, command, HelpDA.ToPagingInfo(filter.PagingInfo), "l.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "l.RegionSysNo", DbType.Int32,
                    "@RegionSysNo", QueryConditionOperatorType.Equal,
                    filter.RegionSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "l.CategorySysNo", DbType.Int32,
                    "@CategorySysNo", QueryConditionOperatorType.Equal,
                    filter.CategorySysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "l.LogCreateDate", DbType.DateTime,
                    "@DateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                    filter.DateFrom);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "l.LogCreateDate", DbType.DateTime,
                    "@DateTo", QueryConditionOperatorType.LessThanOrEqual,
                    filter.DateTo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "l.LogType", DbType.AnsiStringFixedLength,
                      "@LogType", QueryConditionOperatorType.Equal,
                      filter.LogType);

                command.CommandText = builder.BuildQuerySql();

                var dt = command.ExecuteDataTable();

                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }


        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(BizEntity.ExternalSYS.VendorPortalLog log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WriteVendorPortalLog");

            command.SetParameterValue("@RegionName", log.RegionName);
            command.SetParameterValue("@CategoryName", log.CategoryName);
            command.SetParameterValue("@CategoryDescription", log.CategoryDescription);
            command.SetParameterValue("@InUser", log.LogUserName);
            command.SetParameterValue("@Content", log.Content);
            command.SetParameterValue("@LogUserName", log.LogUserName);
            command.SetParameterValue("@ServerIP", log.ServerIP);
            command.SetParameterValue("@ServerName", log.ServerName);
            command.SetParameterValue("@ExtendedProperties", log.ExtendedProperties);
            command.SetParameterValue("@ReferenceKey", log.ReferenceKey);
            command.SetParameterValue("@LogType", log.LogType);

            command.ExecuteNonQuery();
        }

    }
}
