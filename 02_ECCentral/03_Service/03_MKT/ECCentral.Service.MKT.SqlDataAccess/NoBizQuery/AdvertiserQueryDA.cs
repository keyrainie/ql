using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IAdvertiserQueryDA))]
    public class AdvertiserQueryDA : IAdvertiserQueryDA
    {
        /// <summary>
        /// 广告商查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryAdvertiser(AdvertiserQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Advertisers_GetAdvertisers");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "AdvertiserName", DbType.String, "@AdvertiserName",
                    QueryConditionOperatorType.Like, filter.AdvertiserName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "MonitorCode", DbType.String, "@MonitorCode",
                    QueryConditionOperatorType.Like, filter.MonitorCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Status", DbType.String, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CookiePeriod", DbType.Int32, "@CookiePeriod",
                    QueryConditionOperatorType.Equal, filter.CookiePeriod);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<ECCentral.BizEntity.MKT.ADStatus>("Status");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime dtime = System.Convert.ToDateTime(dr["InDate"].ToString());
                    string selectDT = dtime.ToString("yyyy-MM-dd");
                    //dr["EffectLink"] = "http://www.oysd.cn/mkt/queryadveffect.asox?unionid=" + dr["AdvertiserUserName"] + "&pwd=" + dr["AdvertiserPassword"] + "&starttime=" + selectDT + "&endtime=" + selectDT; 
                    dr["EffectLink"] = string.Format(AppSettingManager.GetSetting("MKT", "AdvertiserEffectLink") + "?unionid={0}&pwd={1}&starttime={2}&endtime={3}", dr["AdvertiserUserName"].ToString(), dr["AdvertiserPassword"].ToString(), selectDT, selectDT);
                }
                return dt;//.Tables[0];
            }
        }

        /// <summary>
        /// 广告效果查询 /广告总计的价格查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryAdvEffect(AdvEffectQueryFilter filter, out int totalCount)
        {

            DataCommand cmd = DataCommandManager.GetDataCommand("AdvEffectMonitor_GetAdvEffectMonitorList");

            string sortField = "SysNo";
            string sortType = "Desc";

            if (filter.InDateFrom.HasValue && filter.InDateTo.HasValue && filter.InDateFrom.Equals(filter.InDateTo)) filter.InDateTo = filter.InDateTo.Value.AddDays(1);

            if (filter.PageInfo != null && !string.IsNullOrEmpty(filter.PageInfo.SortBy))
            {
                sortField = filter.PageInfo.SortBy.Split(' ')[0].ToString();
                sortType = filter.PageInfo.SortBy.Split(' ')[1].ToString();
            }

            cmd.SetParameterValue("@SortField", sortField);
            cmd.SetParameterValue("@SortType", sortType);
            cmd.SetParameterValue("@PageSize", filter.PageInfo.PageSize);
            cmd.SetParameterValue("@PageCurrent", filter.PageInfo.PageIndex);

            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            cmd.SetParameterValue("@OperationType", string.Format("%{0}%", filter.OperationType));
            cmd.SetParameterValue("@CMP", string.Format("%{0}%", filter.CMP));
            cmd.SetParameterValue("@IsPhone", filter.IsPhone);
            cmd.SetParameterValue("@IsEmailConfirmed", filter.IsEmailConfirmed);
            cmd.SetParameterValue("@CreateDateFrom", filter.InDateFrom);
            cmd.SetParameterValue("@CreateDateTo", filter.InDateTo);
            cmd.SetParameterValue("@SOID", string.Format("%{0}%", filter.SOID));
            cmd.SetParameterValue("@CustomerID", string.Format("%{0}%", filter.CustomerID));
            cmd.SetParameterValue("@IsValidSO", filter.IsValidSO.HasValue ? (int)filter.IsValidSO : -1);
            cmd.SetParameterValue("@SOStatus", filter.SOStatus.HasValue ? (int)filter.SOStatus : -999);
            if (filter.SOStatus == ECCentral.BizEntity.SO.SOStatus.OutStock && filter.IsRefund.HasValue)
                cmd.SetParameterValue("@IsRefundSO", (int)filter.IsRefund);
            else
                cmd.SetParameterValue("@IsRefundSO", -1);
            cmd.SetParameterValue("@MinSOAmt", filter.MinSOAmt);
            cmd.SetParameterValue("@MaxSOAmt", filter.MaxSOAmt);
            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("IsPhone", typeof(ECCentral.BizEntity.MKT.NYNStatus));
            enumList.Add("IsEmailConfirmed", typeof(ECCentral.BizEntity.MKT.NYNStatus));
            DataTable dt = cmd.ExecuteDataTable(enumList);
            if (dt != null && dt.Rows.Count > 0)
            {
                totalCount = Convert.ToInt32(cmd.GetParameterValue("TotalCount"));
                DataColumn dc = new DataColumn("TotalSOAmount", typeof(decimal));
                dt.Columns.Add(dc);
                dt.Rows[0]["TotalSOAmount"] = Convert.ToDecimal(cmd.GetParameterValue("@TotalSOAmt"));
                return dt;
            }
            else
            {
                totalCount = 0;
                return dt;
            }


            //PagingInfoEntity pagingEntity = new PagingInfoEntity();
            //pagingEntity.SortField = filter.PageInfo.SortBy;
            //pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            //pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            //var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AdvEffectMonitor_GetAdvEffectMonitorList");

            //using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            //{
            //    //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

            //    sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.CreateDate", DbType.DateTime, "@CreateDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);

            //    //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SO.SysNo IS NULL OR (SO.SOType <> 4 AND SO.SOType <> 5))");

            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CMP", DbType.String, "@CMP", QueryConditionOperatorType.Like, filter.CMP);
            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.OperationType", DbType.String, "@OperationType", QueryConditionOperatorType.Like, filter.OperationType);
            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CI.CustomerID", DbType.String, "@CustomerID", QueryConditionOperatorType.Equal, filter.CustomerID);
            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ReferenceSysNo", DbType.Int32, "@ReferenceSysNo", QueryConditionOperatorType.Equal, filter.SOSysNo);
            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CI.IsEmailConfirmed", DbType.Int32, "@IsEmailConfirmed", QueryConditionOperatorType.Equal, filter.IsEmailConfirmed);
            //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CC.Status", DbType.Int32, "@CellPhoneConfirmed", QueryConditionOperatorType.Equal, filter.CellPhoneConfirmed);

            //    //订单有效性
            //    if (filter.IsValidSO == ECCentral.BizEntity.MKT.YNStatus.Yes)
            //        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SO.Status NOT IN (0,1,2,3,4,-6) OR (SO.Status = 4 AND SO.HaveAutoRMA=1) OR (SO.Status IS NULL AND A.OperationType NOT LIKE '%InstalmentOrder%' AND A.OperationType NOT LIKE '%Register%'))");
            //    else if (filter.IsValidSO == ECCentral.BizEntity.MKT.YNStatus.No)
            //        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SO.Status IN (0,1,2,3,-6) OR (SO.Status = 4 AND (SO.HaveAutoRMA <> 1 )))");//SO.HaveAutoRMA <> 1 OR SO.HaveAutoRMA IS NULL

            //    //订单状态
            //    if (filter.Status != -999)
            //    {
            //        if (filter.Status != -99)
            //            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SO.Status", DbType.Int32, "@SOStatus", QueryConditionOperatorType.Equal, filter.Status);
            //        else
            //            sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "SO.Status = 4 AND SO.HaveAutoRMA = 1");
            //    }
            //    //if (filter.Status == 4)
            //    //    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SO.HaveRefund IS NULL)");

            //    if (filter.MaxSOAmt > 0 && filter.MinSOAmt >= 0)
            //        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(" + filter.MinSOAmt + " < SO.SOAmt - SO.DiscountAmt AND " + filter.MaxSOAmt + " >= SO.SOAmt - SO.DiscountAmt)");
            //    else if (filter.MaxSOAmt == 0 && filter.MinSOAmt == 0)
            //        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "(SO.SOAmt - SO.DiscountAmt <= 0)");

            //    cmd.CommandText = sqlBuilder.BuildQuerySql();


            //    EnumColumnList enumList = new EnumColumnList();
            //    enumList.Add("SOStatus", typeof(ECCentral.BizEntity.SO.SOStatus));
            //    enumList.Add("CellPhoneConfirmed", typeof(ECCentral.BizEntity.MKT.NYNStatus));
            //    enumList.Add("IsEmailConfirmed", typeof(ECCentral.BizEntity.MKT.NYNStatus));
            //    enumList.Add("Rank", typeof(ECCentral.BizEntity.Customer.CustomerRank));

            //    //CodeNamePairColumnList pairList = new CodeNamePairColumnList();
            //    //pairList.Add("SOAmtLevel", "MKT", "SOAmtLevel");

            //    var dt = cmd.ExecuteDataTable(enumList);//, pairList);

            //    totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        decimal SOAmount = decimal.Parse(dr[1].ToString());
            //        if (SOAmount == 0)
            //            dr["ShowSOAmtLevel"] = "Z";
            //        else if (0 < SOAmount && SOAmount < 100)
            //            dr["ShowSOAmtLevel"] = "A";
            //        else if (100 < SOAmount && SOAmount < 300)
            //            dr["ShowSOAmtLevel"] = "B";
            //        else if (300 < SOAmount && SOAmount < 500)
            //            dr["ShowSOAmtLevel"] = "C";
            //        else if (500 < SOAmount && SOAmount < 800)
            //            dr["ShowSOAmtLevel"] = "D";
            //        else if (800 < SOAmount && SOAmount < 1000)
            //            dr["ShowSOAmtLevel"] = "E";
            //        else if (1000 < SOAmount && SOAmount < 1500)
            //            dr["ShowSOAmtLevel"] = "F";
            //        else if (1500 < SOAmount && SOAmount < 2000)
            //            dr["ShowSOAmtLevel"] = "G";
            //        else if (2000 < SOAmount && SOAmount < 3000)
            //            dr["ShowSOAmtLevel"] = "H";
            //        else if (3000 < SOAmount && SOAmount < 5000)
            //            dr["ShowSOAmtLevel"] = "I";
            //        else if (5000 < SOAmount && SOAmount < 8000)
            //            dr["ShowSOAmtLevel"] = "J";
            //        else if (8000 < SOAmount && SOAmount < 10000)
            //            dr["ShowSOAmtLevel"] = "K";
            //        else
            //            dr["ShowSOAmtLevel"] = "L";
            //    }
            //    return dt;
            //}
        }

        /// <summary>
        /// 广告效果BBS查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryAdvEffectBBS(AdvEffectBBSQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AdvEffectMonitor_GetAdvBBSList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "B.UserID DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);

                if (filter.InDateFrom.HasValue && filter.InDateTo.HasValue && filter.InDateFrom.Equals(filter.InDateTo)) filter.InDateTo = filter.InDateTo.Value.AddDays(1);

                //sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "A.ClickDate", DbType.DateTime, "@ClickDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ClickDate", DbType.DateTime, "@ClickDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.InDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.ClickDate", DbType.DateTime, "@ClickDateTo", QueryConditionOperatorType.LessThan, filter.InDateTo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 查询订阅维护
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QuerySubscription(SubscriptionQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo != null)
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            else
            {
                pagingEntity = null;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Subscription_GetSubscriptionList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "TransactionNumber DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "s.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThanOrEqual, QueryConditionOperatorType.LessThanOrEqual, filter.InDateFrom, filter.InDateTo);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.MoreThan, filter.InDateFrom);
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.LessThan, filter.InDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, "A");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.SubscriptionCategorySysNo", DbType.Int32, "@SubscriptionCategoryID", QueryConditionOperatorType.Equal, filter.SubscriptionCategoryID);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 查询订阅分类
        /// </summary>
        /// <returns></returns>
        public List<SubscriptionCategory> QuerySubscriptionCategory()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SubscriptionCategory_GetSubscriptionCategoryList");
            return cmd.ExecuteEntityList<SubscriptionCategory>();
        }
    }
}
