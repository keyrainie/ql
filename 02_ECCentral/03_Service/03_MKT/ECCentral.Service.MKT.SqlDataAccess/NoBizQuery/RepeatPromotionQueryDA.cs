using System;
using System.Collections.Generic;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using System.Linq;


namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IRepeatPromotionQueryDA))]
    public class RepeatPromotionQueryDA : IRepeatPromotionQueryDA
    {
        /// <summary>
        /// 查询销售规则
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetSaleRules(RepeatPromotionQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleRules");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                //  "C.ReferenceType",
                //  DbType.Int32, "@ReferenceType",
                //  QueryConditionOperatorType.Equal,
                //  1);
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                // "C.ReferenceType",
                // DbType.Int32, "@ReferenceType",
                // QueryConditionOperatorType.IsNull,
                // DBNull.Value);
                //sqlBuilder.ConditionConstructor.EndGroupCondition();
              

                var sb = new StringBuilder("Exists(select * From IPP3.dbo.SaleRule_Item  l WITH(NOLOCK) ");
                sb.Append(String.Format("Where l.SaleRuleSysNo=C.SysNo And l.ProductSysNo={0} AND  C.Status IN(0,1)) "
                    , queryCriteria.ProductSysNo));
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, sb.ToString());
            
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(ComboStatus) }, { "IsShow", typeof(IsDefaultShow) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询赠品
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetGifts(RepeatPromotionQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetGifts");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                var sysNoList = (new QueryGift()).GetGiftSysNo(queryCriteria);
                var sysNoStr = sysNoList.Count <= 0 ? "-1" : sysNoList.Join(",");

                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
               "C.SysNo",
               QueryConditionOperatorType.In,
               sysNoStr);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(SaleGiftStatus) }, { "Type", typeof(SaleGiftType) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 查询优惠券
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetCoupons(RepeatPromotionQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetCoupons");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                var sysNoList = (new QueryCoupon()).GetCouponSysNo(queryCriteria);
                var sysNoStr = sysNoList.Count <= 0 ? "-1" : sysNoList.Join(",");

                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
               "C.SysNo",
               QueryConditionOperatorType.In,
               sysNoStr);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(CouponsStatus) }, { "ChannelType", typeof(CouponsMKTType) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        ///  查询限时抢购
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetSaleCountDowns(RepeatPromotionQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleCountDowns");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "C.ProductSysNo",
                  DbType.Int32, "@ProductSysNo",
                  QueryConditionOperatorType.Equal,
                  queryCriteria.ProductSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "C.Status",
                   DbType.Int32, "@Status",
                   QueryConditionOperatorType.In,
                    new List<object> { CountdownStatus.WaitForVerify, CountdownStatus.Ready
                                      ,CountdownStatus.Running,CountdownStatus.VerifyFaild});
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                 "C.IsPromotionSchedule",
                 DbType.Int32, "@IsPromotionSchedule",
                 QueryConditionOperatorType.Equal,
                  0);
                if (queryCriteria.StartDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.EndTime",
                        DbType.String, "@StartDate",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryCriteria.StartDate.Value);
                }
                if (queryCriteria.EndDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.StartTime",
                        DbType.String, "@EndDate",
                        QueryConditionOperatorType.LessThan,
                        queryCriteria.EndDate.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(CountdownStatus) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        ///  查询促销计划
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GeSaleCountDownPlan(RepeatPromotionQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleCountDowns");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "C.ProductSysNo",
                  DbType.Int32, "@ProductSysNo",
                  QueryConditionOperatorType.Equal,
                  queryCriteria.ProductSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "C.Status",
                   DbType.Int32, "@Status",
                   QueryConditionOperatorType.In,
                    new List<object> { CountdownStatus.WaitForVerify, CountdownStatus.Ready
                                      ,CountdownStatus.Running,CountdownStatus.Init,CountdownStatus.VerifyFaild});
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                 "C.IsPromotionSchedule",
                 DbType.Int32, "@IsPromotionSchedule",
                 QueryConditionOperatorType.Equal,
                  1);
                if (queryCriteria.StartDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.EndTime",
                        DbType.String, "@StartDate",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryCriteria.StartDate.Value);
                }
                if (queryCriteria.EndDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.StartTime",
                        DbType.String, "@EndDate",
                        QueryConditionOperatorType.LessThan,
                        queryCriteria.EndDate.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(CountdownStatus) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        ///  查询团购
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetProductGroupBuying(RepeatPromotionQueryFilter queryCriteria, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PageInfo.SortBy,
                MaximumRows = queryCriteria.PageInfo.PageSize,
                StartRowIndex = queryCriteria.PageInfo.PageIndex * queryCriteria.PageInfo.PageSize
            };

            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductGroupBuying");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "C.ProductSysNo",
                  DbType.Int32, "@ProductSysNo",
                  QueryConditionOperatorType.Equal,
                  queryCriteria.ProductSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "C.Status",
                   DbType.String, "@Status",
                   QueryConditionOperatorType.In,
                    new List<object> {  GroupBuyingStatus.Init,  GroupBuyingStatus.Pending
                                      , GroupBuyingStatus.Active});
                if (queryCriteria.StartDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.EndDate",
                        DbType.String, "@StartDate",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryCriteria.StartDate.Value);
                }
                if (queryCriteria.EndDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.BeginDate",
                        DbType.String, "@EndDate",
                        QueryConditionOperatorType.LessThan,
                        queryCriteria.EndDate.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(GroupBuyingStatus) }, { "IsByGroup", typeof(IsDefaultShow) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 赠品活动
        /// </summary>
        private class QueryGift
        {
            /// <summary>
            /// 获取赠品活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            public List<int> GetGiftSysNo(RepeatPromotionQueryFilter filter)
            {
                var source = new List<int>();
                var globals = GetGiftSysNoByGlobal(filter);
                var otherglobals = GetOthertGiftSysNoByGlobal(filter);
                var giftSysno = GetGiftSysNoList(filter);
                var otherSysNo = GetOtherGiftSysNoList(filter);
                source.AddRange(globals);
                source.AddRange(otherglobals);
                source.AddRange(giftSysno);
                source.AddRange(otherSysNo);
                source = source.Distinct().ToList();
                return source;
            }

            /// <summary>
            /// 获取全网无排除的赠品活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetGiftSysNoByGlobal(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetGiftSysNoByGlobal");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }

            /// <summary>
            /// 获取全网有排除的赠品活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetOthertGiftSysNoByGlobal(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetOthertGiftSysNoByGlobal");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@ProductSysNo", filter.ProductSysNo);
                cmd.SetParameterValue("@C3SysNo", filter.C3SysNo);
                cmd.SetParameterValue("@BrandSysNo", filter.BrandSysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }

            /// <summary>
            /// 获取单品买赠，同时购买以及厂商赠品的活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetGiftSysNoList(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetGiftSysNoList");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@ProductSysNo", filter.ProductSysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }

            /// <summary>
            /// 获取买满即送赠品的活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetOtherGiftSysNoList(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetOtherGiftSysNoList");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@ProductSysNo", filter.ProductSysNo);
                cmd.SetParameterValue("@C3SysNo", filter.C3SysNo);
                cmd.SetParameterValue("@BrandSysNo", filter.BrandSysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                var otherSource = GetOtherGiftSysNoListByExclude(filter);
                if (otherSource == null || otherSource.Count == 0) return source;
                source = source.Except(otherSource).ToList();
                return source;
            }

            /// <summary>
            /// 获取买满即送赠品的排除
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetOtherGiftSysNoListByExclude(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetOtherGiftSysNoListByExclude");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@ProductSysNo", filter.ProductSysNo);
                cmd.SetParameterValue("@C3SysNo", filter.C3SysNo);
                cmd.SetParameterValue("@BrandSysNo", filter.BrandSysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }


        }

        /// <summary>
        /// 蛋卷活动
        /// </summary>
        private class QueryCoupon
        {
            /// <summary>
            /// 获取蛋卷活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            public List<int> GetCouponSysNo(RepeatPromotionQueryFilter filter)
            {
                var source = new List<int>();
                var globals = GetCouponSysNoByGlobal(filter);
                var otherglobals = GetCouponSysNoByProduct(filter);
                var giftSysno = GetCouponSysNoByC3SysNoOrBrandSysNo(filter);
                source.AddRange(globals);
                source.AddRange(otherglobals);
                source.AddRange(giftSysno);
                return source;
            }

            /// <summary>
            /// 获取全网活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetCouponSysNoByGlobal(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetCouponSysNoByGlobal");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }

            /// <summary>
            /// 获取限定商品活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetCouponSysNoByProduct(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetCouponSysNoByProduct");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@ProductSysNo", filter.ProductSysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }

            /// <summary>
            /// 获取限定三级类以及品牌的活动
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetCouponSysNoByC3SysNoOrBrandSysNo(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetCouponSysNoByC3SysNoOrBrandSysNo");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@C3SysNo", filter.C3SysNo);
                cmd.SetParameterValue("@BrandSysNo", filter.BrandSysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                var otherSource = GetCouponSysNoByExcludeC3SysNo(filter);
                if (otherSource == null || otherSource.Count == 0) return source;
                source = source.Except(otherSource).ToList();
                return source;
            }

            /// <summary>
            /// 获取排除三级类
            /// </summary>
            /// <param name="filter"></param>
            /// <returns></returns>
            private List<int> GetCouponSysNoByExcludeC3SysNo(RepeatPromotionQueryFilter filter)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("GetCouponSysNoByExcludeC3SysNo");
                cmd.SetParameterValue("@StartDate", filter.StartDate ?? DateTime.Now.AddYears(-200));
                cmd.SetParameterValue("@EndDate", filter.EndDate ?? DateTime.Now.AddYears(200));
                cmd.SetParameterValue("@C3SysNo", filter.C3SysNo);
                var source = new List<int>();
                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetInt32(0);
                        source.Add(value);
                    }
                }
                return source;
            }
        }
    }
}
