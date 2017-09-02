using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICountdownQueryDA))]
    public class CountdownQueryDA : ICountdownQueryDA
    {
        public DataTable Query(CountdownQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("CountdownQueryList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "sc.[CreateTime] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[IsPromotionSchedule]", DbType.Int32, "@IsPromotionSchedule",
                    QueryConditionOperatorType.Equal, filter.IsPromotionSchedule);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "p.[MerchantSysNo]", DbType.Int32, "@MerchantSysNo",
                    QueryConditionOperatorType.Equal, filter.MerchantSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[CreateTime]", DbType.DateTime, "@CreateFromTime",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.CreateFromTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[CreateTime]", DbType.DateTime, "@CreateToTime",
                     QueryConditionOperatorType.LessThan,
                     filter.CreateToTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[StartTime]", DbType.DateTime, "@CountdownFromStartTime",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.CountdownFromStartTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[StartTime]", DbType.DateTime, "@CountdownToStartTime",
                     QueryConditionOperatorType.LessThan,
                     filter.CountdownToStartTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[EndTime]", DbType.DateTime, "@CountdownFromEndTime",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.CountdownFromEndTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[EndTime]", DbType.DateTime, "@CountdownToEndTime",
                     QueryConditionOperatorType.LessThan,
                     filter.CountdownToEndTime);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "p.[SysNo]", DbType.Int32, "@ProductSysNo",
                    QueryConditionOperatorType.Equal, filter.ProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sc.[CreateUserName]", DbType.String, "@CreateUserName",
                   QueryConditionOperatorType.Like, filter.CreateUserName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[Status]", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sc.[IsGroupOn]", DbType.StringFixedLength, "@IsGroupOn",
                    QueryConditionOperatorType.Equal, filter.IsGroupOn);
                if (filter.IsCountDownAreaShow>0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "sc.[IsCountDownAreaShow]", DbType.Int16, "@IsCountDownAreaShow",
                       QueryConditionOperatorType.Equal, filter.IsCountDownAreaShow);
                }

                if (filter.IsPromotionSchedule != null && filter.IsPromotionSchedule.Value == 1)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "sc.[PromotionTitle]", DbType.String, "@PromotionTitle",
                       QueryConditionOperatorType.Like, filter.PromotionTitle);

                    if (filter.IsSecondKill != null)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                           "sc.[PromotionType]", DbType.String, "@IsSecondKill",
                           filter.IsSecondKill.Value == 1 ? QueryConditionOperatorType.Equal : QueryConditionOperatorType.NotEqual
                           , "DC");
                    }
                }
                if (filter.IsHomePageShow > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "sc.[IsHomePageShow]", DbType.Int16, "@IsHomePageShow",
                       QueryConditionOperatorType.Equal, filter.IsHomePageShow);
                }
                if (filter.IsC1Show == "Y")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "sc.[IsC1Show]", DbType.StringFixedLength, "@IsC1Show",
                        QueryConditionOperatorType.Equal, filter.IsC1Show);
                }
                if (filter.IsC2Show == "Y")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "sc.[IsC2Show]", DbType.StringFixedLength, "@IsC2Show",
                        QueryConditionOperatorType.Equal, filter.IsC2Show);
                }
                

                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable("Status", typeof(CountdownStatus));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }


        public string GetPMByProductSysNo(string sysNo)
        {
            var dc = DataCommandManager.GetDataCommand("GetPMByProductSysNo");

            dc.SetParameterValue("@ProductSysNo", sysNo);

            return dc.ExecuteScalar<string>();
        }


        public void GetGiftAndCouponSysNo(int productSysNo, out int giftSysNo, out int couponSysNo)
        {
            var dc = DataCommandManager.GetDataCommand("GetGiftAndCouponSysNo");

            dc.SetParameterValue("@ProductSysNo", productSysNo);
            dc.ExecuteNonQuery();
            giftSysNo = (int)dc.GetParameterValue("GiftSysNo");
            couponSysNo = (int)dc.GetParameterValue("CouponSysNo");
        }
    }
}
