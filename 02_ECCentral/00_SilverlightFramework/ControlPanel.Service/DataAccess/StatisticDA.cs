using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;
using System.Threading;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess
{
    public class StatisticDA
    {
        public void InsertEventLog(EventLog log)
        {
            if (log == null)
            {
                return;
            }

            var dataCommand = DataCommandManager.GetDataCommand("InsertEventLog");

            dataCommand.SetParameterValue("@EventLogID", string.IsNullOrWhiteSpace(log.EventLogID) ? Guid.NewGuid().ToString() : log.EventLogID);
            dataCommand.SetParameterValue("@UserID", log.UserID);
            dataCommand.SetParameterValue("@IP", log.IP);
            dataCommand.SetParameterValue("@EventDate", log.EventDate);
            dataCommand.SetParameterValue("@Url", log.Url);
            dataCommand.SetParameterValue("@Action", log.Action);
            dataCommand.SetParameterValue("@Label", log.Label);

            dataCommand.ExecuteNonQuery();
        }

        public List<EventLogView> QueryEventLog(QueryEventLogCriteria criteria)
        {
            if (criteria == null)
            {
                return new List<EventLogView>();
            }
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryEventLog");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand,new Framework.Entity.PagingInfoEntity(), "A.EventDate DESC"))
            {

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.UserID", DbType.AnsiString,
                                                            "@UserID", QueryConditionOperatorType.Equal,
                                                            criteria.UserID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.DisplayName", DbType.AnsiString,
                                                             "@DisplayName", QueryConditionOperatorType.Like,
                                                             criteria.Page);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Action", DbType.String,
                                                             "@Action", QueryConditionOperatorType.Equal,
                                                             criteria.Action);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Label", DbType.String,
                                                             "@Label", QueryConditionOperatorType.Equal,
                                                             criteria.Label);

                if (!string.IsNullOrWhiteSpace(criteria.EventDateFrom))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EventDate", DbType.DateTime,
                                                                 "@EventDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                                                                 Convert.ToDateTime(criteria.EventDateFrom));
                }
                if (!string.IsNullOrWhiteSpace(criteria.EventDateTo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.EventDate", DbType.DateTime,
                                                                 "@EventDateTo", QueryConditionOperatorType.LessThan,
                                                                 Convert.ToDateTime(criteria.EventDateTo).AddDays(1));
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                customCommand.ReplaceParameterValue("#TopCount#", criteria.TopCount.ToString());
                customCommand.ReplaceParameterValue("#LanguageCode#", string.Format("'{0}'", Thread.CurrentThread.CurrentCulture.Name));
                return customCommand.ExecuteEntityList<EventLogView>();
            }
        }

        public List<PVStatisticItem> QueryPVStatistic(QueryPVStatisticCriteria criteria)
        {
            if (criteria == null)
            {
                return new List<PVStatisticItem>();
            }
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPVStatistic");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, new Framework.Entity.PagingInfoEntity(), "InDate DESC"))
            {
                if (criteria.DateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                                                                 criteria.DateFrom.Value);
                }
                if (criteria.DateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateTo", QueryConditionOperatorType.LessThan,
                                                                  criteria.DateTo.Value.AddDays(1));
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                customCommand.ReplaceParameterValue("#LanguageCode#", string.Format("'{0}'", Thread.CurrentThread.CurrentCulture.Name));
                return customCommand.ExecuteEntityList<PVStatisticItem>();
            }
        }


        public List<UserPVStatisticItem> QueryUserPVStatistic(QueryUserPVStatisticCriteria criteria)
        {
            if (criteria == null)
            {
                return new List<UserPVStatisticItem>();
            }
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryUserPVStatistic");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, new Framework.Entity.PagingInfoEntity(), "InDate DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Url", DbType.AnsiString,
                                                                "@Url", QueryConditionOperatorType.Equal,
                                                                criteria.Url);

                if (criteria.DateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                                                                 criteria.DateFrom.Value);
                }
                if (criteria.DateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateTo", QueryConditionOperatorType.LessThan,
                                                                  criteria.DateTo.Value.AddDays(1));
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                return customCommand.ExecuteEntityList<UserPVStatisticItem>();
            }
        }

       
        public List<ActionStatisticItem> QueryActionStatistic(QueryActionStatisticCriteria criteria)
        {
            if (criteria == null)
            {
                return new List<ActionStatisticItem>();
            }
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryActionStatistic");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, new Framework.Entity.PagingInfoEntity(), "InDate DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Url", DbType.AnsiString,
                                                                "@Url", QueryConditionOperatorType.Equal,
                                                                criteria.Url);

                if (criteria.DateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                                                                 criteria.DateFrom.Value);
                }
                if (criteria.DateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateTo", QueryConditionOperatorType.LessThan,
                                                                  criteria.DateTo.Value.AddDays(1));
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                return customCommand.ExecuteEntityList<ActionStatisticItem>();
            }
        }

        public List<LoginStatisticItem> QueryLoginStatistic(QueryLoginStatisticCriteria criteria)
        {
            if (criteria == null)
            {
                return new List<LoginStatisticItem>();
            }
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryLoginStatistic");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, new Framework.Entity.PagingInfoEntity(), "InDate DESC"))
            {

                if (criteria.DateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                                                                 criteria.DateFrom.Value);
                }
                if (criteria.DateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateTo", QueryConditionOperatorType.LessThan,
                                                                  criteria.DateTo.Value.AddDays(1));
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                if (criteria.TopCount > 0)
                {
                    customCommand.ReplaceParameterValue("#TopCount#", string.Format(" TOP {0} ", criteria.TopCount.ToString()));
                }
                else
                {
                    customCommand.ReplaceParameterValue("#TopCount#", string.Empty);
                }
                return customCommand.ExecuteEntityList<LoginStatisticItem>();
            }
        }

        public List<LoginStatisticItem> QueryUniqueLoginStatistic(QueryLoginStatisticCriteria criteria)
        {
            if (criteria == null)
            {
                return new List<LoginStatisticItem>();
            }
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryUniqueLoginStatistic");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, new Framework.Entity.PagingInfoEntity(), "InDate DESC"))
            {

                if (criteria.DateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                                                                 criteria.DateFrom.Value);
                }
                if (criteria.DateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                                                                 "@InDateTo", QueryConditionOperatorType.LessThan,
                                                                  criteria.DateTo.Value.AddDays(1));
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                if (criteria.TopCount > 0)
                {
                    customCommand.ReplaceParameterValue("#TopCount#", string.Format(" TOP {0} ", criteria.TopCount.ToString()));
                }
                else
                {
                    customCommand.ReplaceParameterValue("#TopCount#", string.Empty);
                }
                return customCommand.ExecuteEntityList<LoginStatisticItem>();
            }
        }
    }
}
