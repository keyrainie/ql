using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using c = ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ILogQueryDA))]
    public class LogQueryDA:ILogQueryDA
    {
        public DataTable QuerySysLogWithOutCancelOutStore(QueryFilter.Common.LogQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSysLogWithOutCancelOutStore");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
               command.CommandText, command, pagingInfo, "[LOG].[SysNo] DESC"))
            {
                SetTheSearchCondition(builder, filter);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(LogSOStatus));
                enumList.Add("TicketType", typeof(c.BizLogType));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QuerySysLog(QueryFilter.Common.LogQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSysLog");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
               command.CommandText, command, pagingInfo, "[LOG].[SysNo] DESC"))
            {
                SetTheSearchCondition(builder, filter);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(LogSOStatus));
                enumList.Add("TicketType", typeof(c.BizLogType));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QuerySOLog(QueryFilter.Common.LogQueryFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOLog");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
               command.CommandText, command, pagingInfo, "[LOG].[SysNo] DESC"))
            {
                SetTheSearchCondition(builder, filter);

                command.CommandText = builder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(LogSOStatus));
                enumList.Add("TicketType", typeof(c.BizLogType));
                DataTable dt = command.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }


        /// <summary>
        /// set the ticketsysno, startdata and enddate
        /// </summary>
        /// <param name="command"></param>
        private void SetTheSearchCondition(DynamicQuerySqlBuilder sqlBuilder,
                                           QueryFilter.Common.LogQueryFilter logquery)
        {
            if (logquery.ISSOLog)
            {
                sqlBuilder.ConditionConstructor.AddCondition
                    (QueryConditionRelationType.AND, "[LOG].SOSysNo",
                    DbType.Int32, "@SOSysNo",
                    QueryConditionOperatorType.Equal,
                    logquery.TicketSysNo);
            }
            else
            {
                sqlBuilder.ConditionConstructor.AddCondition
                    (QueryConditionRelationType.AND, "[LOG].TicketSysNo",
                    DbType.Int32, "@TicketSysNo",
                    QueryConditionOperatorType.Equal,
                    logquery.TicketSysNo);
            }

            sqlBuilder.ConditionConstructor.AddCondition
                (QueryConditionRelationType.AND, "[LOG].OPtTime",
                DbType.DateTime, "@StartOPtTime",
                QueryConditionOperatorType.MoreThanOrEqual,
                logquery.StartDate);

            if (logquery.EndDate.HasValue)
            {
                var endDate = Convert.ToDateTime
                    (logquery.EndDate.Value.ToShortDateString() + " 23:59:59");
                sqlBuilder.ConditionConstructor.AddCondition
                    (QueryConditionRelationType.AND, "[LOG].OPtTime",
                    DbType.String, "@EndOPtTime",
                    QueryConditionOperatorType.LessThanOrEqual,
                    endDate);
            }

            if (logquery.CancelOutStore)
            {
                sqlBuilder.ConditionConstructor.AddCondition
                    (QueryConditionRelationType.AND, "[LOG].TicketType",
                    DbType.Int32, "@TicketType",
                    QueryConditionOperatorType.Equal,
                    (int)c.BizLogType.Sale_SO_CancelOutStock);
            }
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "LOG.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, "8601");
        }
    }
}
