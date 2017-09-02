using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IGroupBuyingLotteryQueryDA))]
    public class GroupBuyingLotteryQueryDA : IGroupBuyingLotteryQueryDA
    {
        public DataSet Query(GroupBuyingLotteryQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetGroupBuyingLotteryList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "M.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.[ProductGroupBuyingSysNo]", DbType.Int32, "@ProductGroupBuyingSysNo",
                    QueryConditionOperatorType.Equal, filter.GroupBuyingSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.LotteryDate", DbType.DateTime, "@BeginDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.BeginDateFrom);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //    "M.LotteryDate", DbType.DateTime, "@BeginDateTo",
                //     QueryConditionOperatorType.LessThan,
                //     filter.BeginDateTo);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //    "M.LotteryDate", DbType.DateTime, "@EndDateFrom",
                //     QueryConditionOperatorType.MoreThanOrEqual,
                //     filter.EndDateFrom);

                if (filter.EndDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "M.LotteryDate", DbType.DateTime, "@EndDateTo",
                         QueryConditionOperatorType.LessThan,
                         filter.EndDateTo.Value.AddDays(1));
                }


                object _rank;
                if (filter.RankType != null && EnumCodeMapper.TryGetCode(filter.RankType, out _rank))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "C.Rank",
                       DbType.Int32,
                       "@Rank",
                       QueryConditionOperatorType.Equal,
                       _rank);
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "M.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();

                cmd.ConvertEnumColumn(ds.Tables[0], new EnumColumnList { { "Rank", typeof(CustomerRank) } });
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }
        }
    }
}
