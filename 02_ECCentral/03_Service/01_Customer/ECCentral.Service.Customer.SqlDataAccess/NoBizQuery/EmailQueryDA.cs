using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IEmailQueryDA))]
    public class EmailQueryDA : IEmailQueryDA
    {

        public virtual DataTable Query(QueryFilter.Customer.EmailQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            CustomDataCommand cmd = null;
            if (filter.Source.ToLower() == "maildb")
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("EmailSearchInMailDB");
            else
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("EmailSearchInIPP3");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "MailAddress",
                   DbType.String,
                   "@MailAddress",
                   QueryConditionOperatorType.Like,
                   filter.Email);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "MailSubject",
                    DbType.String,
                    "@MailSubject",
                    QueryConditionOperatorType.Like,
                    filter.Title);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Status",
                    DbType.Int32,
                    "@Status",
                    QueryConditionOperatorType.Equal,
                    filter.Status);
                if (filter.EndDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "CreateTime",
                        DbType.DateTime,
                        "@CreateDateFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.EndDateFrom);
                }
                if (filter.EndDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                             QueryConditionRelationType.AND,
                             "CreateTime",
                             DbType.DateTime,
                             "@CreateDateTo",
                             QueryConditionOperatorType.LessThan,
                             filter.EndDateTo);
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CompanyCode",
                    DbType.AnsiStringFixedLength,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    "8601");
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(4, typeof(EmailSendStatus));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

    }
}
