using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ILotteryQueryDA))]
    public class LotteryQueryDA : ILotteryQueryDA
    {
        public DataSet Query(LotteryQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetLotteryList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "lr.TransactionNumber DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "li.LotteryLabel", DbType.String, "@LotteryLabel",
                    QueryConditionOperatorType.LeftLike, filter.LotteryLabel);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "li.Title", DbType.String, "@LotteryTitle",
                    QueryConditionOperatorType.Like, filter.LotteryName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "li.BeginDate", DbType.DateTime, "@BeginDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.BeginDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "li.BeginDate", DbType.DateTime, "@BeginDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.BeginDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "li.EndDate", DbType.DateTime, "@EndDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.EndDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "li.EndDate", DbType.DateTime, "@EndDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.EndDateTo);


                if (filter.IsLucky != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "lr.IsLucky",
                       DbType.Int32,
                       "@IsLucky",
                       QueryConditionOperatorType.Equal,
                       filter.IsLucky == NYNStatus.Yes ? 1 : 0);
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "li.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();

                cmd.ConvertEnumColumn(ds.Tables[0], new EnumColumnList { { "IsLucky", typeof(NYNStatus) } });
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }
        }
    }
}
