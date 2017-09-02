using System;
using System.Data;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data.Common;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using System.Text;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICustomerVisitQueryDA))]
    public class CustomerVisitQueryDA : ICustomerVisitQueryDA
    {
        private PagingInfoEntity ToPagingInfo(dynamic pagingInfo)
        {
            return new PagingInfoEntity()
            {
                SortField = (pagingInfo.SortBy == null ? "" : pagingInfo.SortBy),
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        #region 回访列表查询
        public virtual DataTable Query(CustomerVisitQueryFilter filter, out int totalCount)
        {
            VisitSeachType searchType = VisitSeachType.WaitingVisit;
            if (filter.SeachType.HasValue)
            {
                searchType = Enum.TryParse<VisitSeachType>(filter.SeachType.Value.ToString(), out searchType) ? searchType : VisitSeachType.WaitingVisit;
            }
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            CustomDataCommand command = null;
            QuerySorterHelper.SetSorterSQL(pagingEntity);
            if (searchType == VisitSeachType.Visited || searchType == VisitSeachType.FollowUpMaintenance)
            {
                //回访统计查询或者是需跟进维护查询
                command = DataCommandManager.CreateCustomDataCommandFromConfig("GetVisitCustomerByMaintenanceQuery");
                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingEntity, "VC.SysNo DESC"))
                {
                    AddTheBasicCustomerConditions(filter, command, sqlBuilder);
                    AddTheQueryVisitCustomerConditions(filter, command, sqlBuilder, searchType);
                    command.CommandText = sqlBuilder.BuildQuerySql();

                    ReplaceVisitOrderStrWhere(filter, command);
                }
            }
            else if (searchType == VisitSeachType.Maintenance)
            {
                command = DataCommandManager.CreateCustomDataCommandFromConfig("GetNeedMaintainCustomers");
                if (filter.PageInfo != null && !string.IsNullOrEmpty(pagingEntity.SortField))
                {
                    pagingEntity.SortField = "tempTable.Priority ASC," + pagingEntity.SortField;
                }

                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    command.CommandText,
                    command,
                    pagingEntity,
                    "VC.LastCallTime DESC"))
                {
                    AddTheBasicCustomerConditions(filter, command, sqlBuilder);
                    AddTheQueryVisitCustomerConditions(filter, command, sqlBuilder, searchType);

                    if (string.IsNullOrEmpty(filter.CustomerSysNos))
                    {
                        filter.CustomerSysNos = "null";
                    }

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "@CustomerSysNoList",
                        DbType.AnsiString, "@CustomerSysNoList",
                        QueryConditionOperatorType.Equal,
                        filter.CustomerSysNos);

                    command.CommandText = sqlBuilder.BuildQuerySql();
                    ReplaceVisitOrderStrWhere(filter, command);
                }
            }
            else if (searchType == VisitSeachType.WaitingVisit)
            {
                //查询首次待会访的客户
                command = DataCommandManager.CreateCustomDataCommandFromConfig
                    ("GetFirstVisitingCustomers");
                if (filter.PageInfo != null && !string.IsNullOrEmpty(pagingEntity.SortField))
                {
                    pagingEntity.SortField = "tempTable.Priority ASC," + pagingEntity.SortField;
                }

                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    command.CommandText,
                    command,
                    pagingEntity,
                    "tempTable.Priority ASC,cus.CustomerSysNo DESC"))
                {
                    int days = 0;
                    if (!int.TryParse(AppSettingManager.GetSetting("Customer", "VisitCustomerActiveNeedDays"), out days))
                    {
                        days = 180;
                    }

                    string sql = "(TVC.LastBuyDate > '1990-1-2' AND TVC.LastBuyDate < @LastCallTime AND TVC.LastCallTime < @LastCallTime)";

                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, sql);
                    command.AddInputParameter("@LastCallTime", DbType.DateTime, DateTime.Now.AddDays(-days));

                    if (string.IsNullOrEmpty(filter.CustomerSysNos))
                    {
                        filter.CustomerSysNos = "null";
                    }

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "@CustomerSysNoList",
                        DbType.AnsiString, "@CustomerSysNoList",
                        QueryConditionOperatorType.Equal,
                        filter.CustomerSysNos);

                    AddCustomerConditionsForToVisit(filter, command, sqlBuilder);

                    command.CommandText = sqlBuilder.BuildQuerySql();
                }
            }
            else
            {
                //查询已经回访过的，但是仍然需要跟进的客户
                command = DataCommandManager.CreateCustomDataCommandFromConfig
                    ("GetNeedVisitingCustomers");
                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    command.CommandText,
                    command,
                    pagingEntity,
                    "CS.RegisterTime DESC"))
                {
                    //需要跟进LastCallStatus = 0
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "VC.LastCallStatus",
                        DbType.Int32, "@LastCallStatus",
                        QueryConditionOperatorType.Equal,
                        0);

                    AddTheBasicCustomerConditions(filter, command, sqlBuilder);
                    if (filter.IsSpiderSearch)
                    {
                        AddTheQueryVisitCustomerConditions(filter,
                            command,
                            sqlBuilder,
                           searchType);
                    }
                    command.CommandText = sqlBuilder.BuildQuerySql();
                }
            }
            EnumColumnList ecList = new EnumColumnList();
            ecList.Add("Rank", typeof(CustomerRank));
            ecList.Add("LastCallStatus", typeof(VisitDealStatus));
            ecList.Add("LastMaintenanceStatus", typeof(VisitDealStatus));
            ecList.Add("NeedBuy", typeof(YNStatusThree));
            ecList.Add("IsActive", typeof(YNStatus));
            //ecList.Add("IsRMA", typeof(YNStatus));
            ecList.Add("LastCallReason", typeof(VisitCallResult));
            DataTable dt = command.ExecuteDataTable(ecList);
            totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return dt;
        }

        private static void ReplaceVisitOrderStrWhere(CustomerVisitQueryFilter filter, CustomDataCommand command)
        {
            string visitOrderStrWhere = "where 1=1";
            if (filter.SpiderOrderDateFrom.HasValue)
            {
                visitOrderStrWhere += " and OrderDate>='" + filter.SpiderOrderDateFrom + "'";
            }
            if (filter.SpiderOrderDateTo.HasValue)
            {
                visitOrderStrWhere += " and OrderDate<'" + filter.SpiderOrderDateTo + "'";
            }
            command.ReplaceParameterValue("#VisitOrderStrWhere#",
                visitOrderStrWhere);
        }

        #endregion

        private static SorterSQLHelper m_querySorterHelper = null;

        private static SorterSQLHelper QuerySorterHelper
        {
            get
            {
                if (m_querySorterHelper == null)
                {
                    InitializeSorterSQLHelper();
                }
                return m_querySorterHelper;
            }
        }

        private static void InitializeSorterSQLHelper()
        {
            m_querySorterHelper = new SorterSQLHelper("cus.SysNo DESC");

            m_querySorterHelper.Add("RegisterTime", "cus.RegisterTime");
            m_querySorterHelper.Add("CustomerID", "cus.CustomerID");
            m_querySorterHelper.Add("CustomerName", "cus.CustomerName");
            m_querySorterHelper.Add("Phone", "cus.Phone");
            m_querySorterHelper.Add("Email", "cus.Email");
            m_querySorterHelper.Add("Rank", "cus.Rank");
            m_querySorterHelper.Add("LastCallTime", " VC.LastCallTime");
            m_querySorterHelper.Add("SpiderAmt", "ISNULL(VO.spideramt,0)");
            m_querySorterHelper.Add("SpiderCount", "ISNULL(VO.spidercount,0)");
            m_querySorterHelper.Add("LastBuyTime", "E.LastBuyDate");
            m_querySorterHelper.Add("LastContactTime", "vc.Status,vc.LastCallTime");

            m_querySorterHelper.Add("LastCallStatus", "vc.LastCallStatus");
            m_querySorterHelper.Add("LastCallReason", "vc.LastCallStatus,vc.LastMaintenanceStatus,vc.LastCallReason");
            m_querySorterHelper.Add("LastMaintenanceStatus", "vc.LastMaintenanceStatus");
            m_querySorterHelper.Add("ConfirmedTotalAmt", "cus.ConfirmedTotalAmt");
            m_querySorterHelper.Add("ContactCount", "VC.ContactCount");
            m_querySorterHelper.Add("ActiveStatus", "VC.IsActive");
            m_querySorterHelper.Add("NeedBuyDisplay", "VC.NeedBuy");
            m_querySorterHelper.Add("LastEditUserName", "UIF.DisplayName");
        }

        private static void AddTheQueryVisitCustomerConditions(CustomerVisitQueryFilter filter, CustomDataCommand command,
            DynamicQuerySqlBuilder sqlBuilder, VisitSeachType queryType)
        {
            if (queryType == VisitSeachType.WaitingVisit)//首次维护
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "VC.IsActive", DbType.Int32, "@IsActive",
                   QueryConditionOperatorType.Equal,
                   1);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "VC.IsRMA", DbType.Boolean, "@IsRMA",
                   QueryConditionOperatorType.Equal,
                   true);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "ISNULL(VC.LastMaintenanceStatus,-2)", DbType.Int32, "@LastMaintenanceStatus",
                   QueryConditionOperatorType.Equal,
                   -2);
            }
            if (queryType == VisitSeachType.FollowUpMaintenance)//需跟进维护
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "VC.IsActive", DbType.Int32, "@IsActive",
                   QueryConditionOperatorType.Equal,
                   1);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "VC.IsRMA", DbType.Boolean, "@IsRMA",
                   QueryConditionOperatorType.Equal,
                   true);
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "ISNULL(VC.LastMaintenanceStatus,-2)", DbType.Int32, "@LastMaintenanceStatus",
                   QueryConditionOperatorType.Equal,
                   0);
            }
            if (filter.SeachType == (int)VisitSeachType.Visited)//回访统计
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                if (filter.IsMaintain.HasValue)
                {
                    if (filter.IsMaintain.Value)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.OR,
                            "ISNULL(VC.LastMaintenanceStatus,-2)", DbType.Int32, "@LastCallStatus",
                            QueryConditionOperatorType.Equal,
                            filter.DealStatus);

                    }
                    else if (!filter.IsMaintain.Value)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                                 QueryConditionRelationType.AND,
                                 "VC.LastCallStatus", DbType.Int32, "@LastCallStatus",
                                 QueryConditionOperatorType.Equal,
                                 filter.DealStatus);
                    }
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                 QueryConditionRelationType.AND,
                                 "VC.LastCallStatus", DbType.Int32, "@LastCallStatus",
                                 QueryConditionOperatorType.Equal,
                                 filter.DealStatus);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.OR,
                        "ISNULL(VC.LastMaintenanceStatus,-2)", DbType.Int32, "@LastCallStatus",
                        QueryConditionOperatorType.Equal,
                        filter.DealStatus);
                }

                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }
            else
            {
                sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "VC.LastMaintenanceStatus", DbType.Int32, "@LastMaintenanceStatus",
               QueryConditionOperatorType.Equal,
               filter.DealStatus);
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "VC.CreateDate", DbType.DateTime, "@VisitDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.FromVisitDate);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "VC.CreateDate", DbType.DateTime, "@VisitDateTo",
                QueryConditionOperatorType.LessThan,
                filter.ToVisitDate);

            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "VC.IsActive", DbType.Int32, "@IsActive",
               QueryConditionOperatorType.Equal,
               filter.IsActivated);

            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "VC.NeedBuy", DbType.Int32, "@NeedBuy",
               QueryConditionOperatorType.Equal,
               filter.ConsumeDesire);

            if (filter.IsMaintain.HasValue)
            {
                if (filter.IsMaintain.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                           QueryConditionRelationType.AND,
                                           "VC.LastMaintenanceStatus", DbType.Int32, "@LastMaintenanceStatus",
                                           QueryConditionOperatorType.NotEqual,
                                           -2);
                }
                else if (!filter.IsMaintain.Value)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                           QueryConditionRelationType.AND,
                                           "VC.LastMaintenanceStatus", DbType.Int32, "@LastMaintenanceStatus",
                                           QueryConditionOperatorType.Equal,
                                           -2);
                }
            }

            sqlBuilder.ConditionConstructor.AddCondition(
               QueryConditionRelationType.AND,
               "VC.LastEditUserSysNo", DbType.Int32, "@LastEditUserSysNo",
               QueryConditionOperatorType.Equal,
               filter.LastEditorSysNo);

            sqlBuilder.ConditionConstructor.AddInCondition<int>(
                    QueryConditionRelationType.AND,
                    "VC.LastCallReason", DbType.Int32,
                    filter.CallResult);

        }

        private static void AddTheBasicCustomerConditions(CustomerVisitQueryFilter filter,
            CustomDataCommand command,
            DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.SysNo",
                DbType.Int32, "@CustomerSysNo",
                QueryConditionOperatorType.Equal,
                filter.CustomerSysNo);

            if (!string.IsNullOrEmpty(filter.CustomerID))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(
                    QueryConditionRelationType.AND,
                   "cus.CustomerID like @CustomerID");

                command.AddInputParameter("@CustomerID", DbType.String, string.Format("{0}%", filter.CustomerID));
            }


            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                    "cus.CustomerName like @CustomerName");

                command.AddInputParameter("@CustomerName", DbType.String, string.Format("{0}%", filter.CustomerName));
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                    "cus.Email like @Email");

                command.AddInputParameter("@Email", DbType.String, string.Format("{0}%", filter.Email));
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.Rank",
                DbType.Int32, "@Rank",
                QueryConditionOperatorType.Equal,
                filter.CustomerRank);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.CellPhone",
                DbType.String, "@CellPhone",
                QueryConditionOperatorType.Equal,
                filter.Phone);


            if (!string.IsNullOrEmpty(filter.Address))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                    "cus.DwellAddress like @DwellAddress");
                command.AddInputParameter("@DwellAddress", DbType.String, string.Format("{0}%", filter.Address));
            }

            if (filter.ShipType.HasValue)
            {
                var prefix = string.Empty;

                if (filter.ShipTypeCondition == 0) prefix = "=";
                else if (filter.ShipTypeCondition == 1) prefix = ">";
                else prefix = "<";

                if (filter.SeachType == (int)VisitSeachType.WaitingVisit)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        "DATEDIFF(day,TVC.LastBuyDate,GETDATE()) " + prefix + filter.ShipType);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        "DATEDIFF(day,e.LastBuyDate,GETDATE()) " + prefix + filter.ShipType);
                }
            }

            //是否VIP(Vip rank 为2或者4)
            if (filter.IsVip.HasValue && filter.IsVip.Value)
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
               "cus.VipRank IN (2,4)");
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.ConfirmedTotalAmt",
                DbType.Decimal, "@ConfirmedTotalAmt1",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.FromTotalAmount);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.ConfirmedTotalAmt",
                DbType.Decimal, "@ConfirmedTotalAmt2",
                QueryConditionOperatorType.LessThan,
                filter.ToTotalAmount);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "cus.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
        }

        private static void AddCustomerConditionsForToVisit(CustomerVisitQueryFilter filter,
          CustomDataCommand command,
          DynamicQuerySqlBuilder sqlBuilder)
        {
            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.SysNo",
                DbType.Int32, "@CustomerSysNo",
                QueryConditionOperatorType.Equal,
                filter.CustomerSysNo);

            if (!string.IsNullOrEmpty(filter.CustomerID))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(
                    QueryConditionRelationType.AND,
                   "cus.CustomerID like @CustomerID");

                command.AddInputParameter("@CustomerID", DbType.String, string.Format("{0}%", filter.CustomerID));
            }


            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                    "cus.CustomerName like @CustomerName");

                command.AddInputParameter("@CustomerName", DbType.String, string.Format("{0}%", filter.CustomerName));
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                    "cus.Email like @Email");

                command.AddInputParameter("@Email", DbType.String, string.Format("{0}%", filter.Email));
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.Rank",
                DbType.Int32, "@Rank",
                QueryConditionOperatorType.Equal,
                filter.CustomerRank);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.CellPhone",
                DbType.String, "@CellPhone",
                QueryConditionOperatorType.Equal,
                filter.Phone);


            if (!string.IsNullOrEmpty(filter.Address))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                    "cus.DwellAddress like @DwellAddress");
                command.AddInputParameter("@DwellAddress", DbType.String, string.Format("{0}%", filter.Address));
            }

            if (filter.ShipType.HasValue)
            {
                var prefix = string.Empty;

                if (filter.ShipTypeCondition == 0) prefix = "=";
                else if (filter.ShipTypeCondition == 1) prefix = ">";
                else prefix = "<";

                if (filter.SeachType == (int)VisitSeachType.WaitingVisit)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        "DATEDIFF(day,TVC.LastBuyDate,GETDATE()) " + prefix + filter.ShipType);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        "DATEDIFF(day,vc.LastBuyTime,GETDATE()) " + prefix + filter.ShipType);
                }
            }

            //是否VIP(Vip rank 为2或者4)
            if (filter.IsVip.HasValue && filter.IsVip.Value)
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
               "cus.VipRank IN (2,4)");
            }

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.ConfirmedTotalAmt",
                DbType.Decimal, "@ConfirmedTotalAmt1",
                QueryConditionOperatorType.MoreThanOrEqual,
                filter.FromTotalAmount);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "cus.ConfirmedTotalAmt",
                DbType.Decimal, "@ConfirmedTotalAmt2",
                QueryConditionOperatorType.LessThan,
                filter.ToTotalAmount);

            sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "TVC.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                filter.CompanyCode);
        }
    }
}

public class SorterSQLHelper : Dictionary<string, string>
{

    private string defaultSorterKey = "defaultsorterkey";

    public SorterSQLHelper(string defaultSorter)
        : this()
    {
        this.Add(defaultSorterKey, defaultSorter);
    }

    private SorterSQLHelper()
        : base(StringComparer.CurrentCultureIgnoreCase)
    {
    }

    public string GetSorterSQL(string sorterPara)
    {
        if (string.IsNullOrEmpty(sorterPara))
        {
            return this[defaultSorterKey];
        }

        StringBuilder result = new StringBuilder();

        string[] paras = sorterPara.TrimEnd().TrimStart().Split(" ".ToCharArray());

        if (paras.Length == 2 && this.ContainsKey(paras[0].Trim()))
        {
            result.Append(this[paras[0].Trim()]);
            result.Append(" " + paras[1].ToUpper());
        }
        else
        {
            result.Append(this[defaultSorterKey]);
        }

        return result.ToString();
    }

    public void SetSorterSQL(PagingInfoEntity pagingInfo)
    {
        if (pagingInfo != null)
        {
            if (string.IsNullOrEmpty(pagingInfo.SortField))
            {
                pagingInfo.SortField = this[defaultSorterKey];
            }
            else
            {
                pagingInfo.SortField = GetSorterSQL(pagingInfo.SortField);
            }
        }
    }

    public string DefaultSorter
    {
        get
        {
            return this[defaultSorterKey];
        }
    }

    public new void Add(string key, string value)
    {
        base.Add(key.Trim(), value.Trim());
    }

    public new void Clear()
    {
        if (!string.IsNullOrEmpty(this[defaultSorterKey]))
        {
            string defaultValue = this[defaultSorterKey];
            base.Clear();
            base.Add(defaultSorterKey, defaultValue);
        }
        else
        {
            base.Clear();
        }
    }

    public new bool Remove(string key)
    {
        if (key.Trim() == defaultSorterKey)
        {
            return false;
        }
        else
        {
            return base.Remove(key);
        }
    }
}