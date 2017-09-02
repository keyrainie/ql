using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.BizEntity.SO;


namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICouponsQueryDA))]
    public class CouponsQueryDA : ICouponsQueryDA
    {
        public DataSet QueryCouponCode(CouponCodeQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponCode");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo ASC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CouponSysNo", DbType.Int32,
                    "@CouponSysNo", QueryConditionOperatorType.Equal, filter.CouponSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CouponCode", DbType.String,
                    "@CouponCode", QueryConditionOperatorType.Equal, filter.CouponCode);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual,
                    filter.CodeSysNoFrom, filter.CodeSysNoTo);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime,
                     "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan,
                     filter.InDateFrom, filter.InDateTo != null ? filter.InDateTo.Value.AddDays(1) : filter.InDateTo);
                if (filter.EndDateTo.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BeginDate", DbType.DateTime,
                      "@BeginDate", QueryConditionOperatorType.LessThan, filter.EndDateTo.Value.AddDays(1));
                if (filter.BeginDateFrom.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "EndDate", DbType.DateTime,
                      "@EndDate", QueryConditionOperatorType.MoreThanOrEqual, filter.BeginDateFrom);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }

        }

        public DataTable QueryCoupons(CouponsQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCoupons");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "WebChannelID", DbType.AnsiStringFixedLength,
                //    "@WebChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.MerchantSysNo", DbType.Int32,
                   "@MerchantSysNo", QueryConditionOperatorType.Equal, filter.MerchantSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo", DbType.Int32, "@SysNoFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.CouponsSysNoFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo", DbType.Int32, "@SysNoTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.CouponsSysNoTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CouponName", DbType.String,
                    "@CouponName", QueryConditionOperatorType.Like, filter.CouponsName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status", DbType.AnsiStringFixedLength,
                   "@Status", QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.InUser", DbType.String,
                   "@InUser", QueryConditionOperatorType.Equal, filter.CreateUser);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.AuditUser", DbType.String,
                   "@AuditUser", QueryConditionOperatorType.Equal, filter.AuditUser);

                if (filter.BeginDate.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.AuditDate", DbType.DateTime,
                    "@BeginDate", QueryConditionOperatorType.MoreThanOrEqual, filter.BeginDate);
                if (filter.EndDate.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.AuditDate", DbType.DateTime,
                     "@EndDate", QueryConditionOperatorType.LessThan, filter.EndDate.Value.AddDays(1));


                if (filter.CreateDateFrom.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime,
                     "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                if (filter.CreateDateTo.HasValue) sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.InDate", DbType.DateTime,
                     "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo.Value.AddDays(1));

                string sql = "DECLARE @CouponCode NVARCHAR(20) ";
                if (!string.IsNullOrEmpty(filter.CouponCode))
                {
                    sql += string.Format("SET @CouponCode='{0}'", filter.CouponCode);
                }

                cmd.CommandText = sql + sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(CouponsStatus));
                enumList.Add("RulesType", typeof(CouponsProductRangeType));
                var dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        /// <summary>
        /// 优惠券使用记录查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryCouponCodeRedeemLog(CouponCodeRedeemLogFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponCodeRedeemLog");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "Coupon.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "Coupon.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual,
                    filter.CouponBeginNo, filter.CouponEndNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.CouponName", DbType.String,
                    "@CouponName", QueryConditionOperatorType.Equal, filter.CouponName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RedeemLog.CouponCode", DbType.String,
                    "@CouponCode", QueryConditionOperatorType.Equal, filter.CouponCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cs.CustomerID", DbType.String,
                    "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.SysNo", DbType.String,
                    "@SOID", QueryConditionOperatorType.Equal, filter.SOID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RedeemLog.Status", DbType.String,
                    "@CouponCodeStatus", QueryConditionOperatorType.Equal, filter.CouponCodeStatus);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "RedeemLog.InDate", DbType.DateTime,
                     "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan,
                     filter.BeginUseDate, filter.EndUseDate);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable(new EnumColumnList{
                    {"SOStatus",typeof(SOStatus)},{"RedeemLogStatus",typeof(CouponCodeUsedStatus)}});

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        /// <summary>
        /// 优惠券发放记录查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryCouponCodeCustomerLog(CouponCodeCustomerLogFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCouponCodeCustomerLog");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "Coupon.InDate desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Coupon.CouponName", DbType.String,
                    "@CouponName", QueryConditionOperatorType.Like, filter.CouponName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Customer.SysNo", DbType.String,
                   "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Customer.CustomerID", DbType.String,
                    "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "Coupon.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual,
                    filter.CouponBeginNo, filter.CouponEndNo);

                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "CustomerLog.GetCouponCodeDate", DbType.DateTime,
                     "@GetCouponCodeDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThan,
                     filter.BeginUseDate, filter.EndUseDate);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable(new EnumColumnList{
                    {"Status",typeof(CouponsStatus)}});

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

    }



}
