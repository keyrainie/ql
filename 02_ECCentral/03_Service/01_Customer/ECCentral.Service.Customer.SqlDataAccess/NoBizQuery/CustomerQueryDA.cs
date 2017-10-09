using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Customer;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.Customer;
using ECCentral.BizEntity.Customer.Society;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICustomerQueryDA))]
    public class CustomerQueryDA : ICustomerQueryDA
    {
        public virtual DataTable SimpleQuery(CustomerSimpleQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_SimpleQuery");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "C.SysNo", DbType.Int32, "@SystemNumber",
                   QueryConditionOperatorType.Equal,
                   queryCriteria.CustomerSysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.CustomerID", DbType.String, "@CustomerID", QueryConditionOperatorType.LeftLike, queryCriteria.CustomerID);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.CustomerName", DbType.String, "@CustomerName", QueryConditionOperatorType.LeftLike, queryCriteria.CustomerName);
                // sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.MemberShipCard", DbType.String, "@MemberShipCard", QueryConditionOperatorType.LeftLike, queryCriteria.CustomerName);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.Email", DbType.AnsiString, "@Email", QueryConditionOperatorType.LeftLike, queryCriteria.CustomerEmail);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "C.Status", DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, queryCriteria.Status);
                if (!string.IsNullOrEmpty(queryCriteria.CustomerPhone))
                {
                    //添加CustomerPhone子查询，并添加一个参数
                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "", QueryConditionOperatorType.Exist,
                        @"SELECT 1  FROM OverseaCustomerManagement.[dbo].[CustomerPhone] WITH(NOLOCK)
								WHERE OverseaCustomerManagement.[dbo].[CustomerPhone].Phone like '%'+@Phone+'%'
									AND OverseaCustomerManagement.[dbo].CustomerPhone.CustomerSysNo = C.SysNo");
                    cmd.AddInputParameter("@Phone", DbType.String, queryCriteria.CustomerPhone);
                }

                sb.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "C.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                queryCriteria.CompanyCode);

                cmd.CommandText = sb.BuildQuerySql();
                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add(2, typeof(CustomerStatus));
                enumColumnList.Add(30, typeof(CustomerType));
                enumColumnList.Add(17, typeof(CustomerRank));
                enumColumnList.Add(19, typeof(VIPRank));
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #region 顾客查询
        /// <summary>
        /// 查询顾客
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable Query(CustomerQueryFilter queryCriteria, out int totalCount)
        {
            totalCount = 0;
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_QueryExtend");

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            pagingInfo.SortField = queryCriteria.PagingInfo.SortBy;
            pagingInfo.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            pagingInfo.MaximumRows = queryCriteria.PagingInfo.PageSize;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand,
                pagingInfo,
                "C.RegisterTime DESC"))
            {
                //设置查询条件参数
                AddParameterCustomerList(queryCriteria, customCommand, sqlBuilder);
                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add(2, typeof(CustomerStatus));
                enumColumnList.Add(4, typeof(Gender));
                enumColumnList.Add(14, typeof(YNStatus));
                enumColumnList.Add(17, typeof(CustomerRank));
                enumColumnList.Add(19, typeof(VIPRank));
                enumColumnList.Add(30, typeof(CustomerType));
                enumColumnList.Add(31, typeof(YNStatus));
                enumColumnList.Add(34, typeof(YNStatus));
                enumColumnList.Add(36, typeof(AvtarShowStatus));
                DataTable dt = customCommand.ExecuteDataTable(enumColumnList);
                totalCount = int.Parse(customCommand.GetParameterValue("@TotalCount").ToString());

                List<CustomerInfo> aList = customCommand.ExecuteEntityList<CustomerInfo>();
                List<int?> customerSysNoList = aList.Select(p => p.SysNo).ToList();
                List<ThirdPartyUser> thirdPartUserList = GetThirdPartyUserInfo(customerSysNoList);
                if (thirdPartUserList.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < thirdPartUserList.Count; j++)
                        {
                            if (int.Parse(dt.Rows[i]["SysNo"].ToString()) == thirdPartUserList[j].CustomerSysNo)
                            {
                                ThirdPartyUser thirdPartyUser = thirdPartUserList.FirstOrDefault(p => p.CustomerSysNo == int.Parse(dt.Rows[i]["SysNo"].ToString()));
                                if (thirdPartyUser != null && thirdPartyUser.SubSource != null)
                                {
                                    if (thirdPartyUser.UserSource == "支付宝" && (thirdPartyUser.SubSource.ToUpper() == "VIP" || thirdPartyUser.SubSource.ToUpper() == "IMPERIAL_VIP"))
                                    {
                                        dt.Rows[i]["CustomerID"] = string.Format("{0}\r\n【{1}】", dt.Rows[i]["CustomerID"].ToString(), "金帐户");
                                    }
                                }
                            }
                        }
                    }
                }
                return dt;
            }
            #region 中蛋逻辑(“金帐户”显示)
            /****
                     * (第三方用户（表ThirdPartyUser）来源（字段UserSource）为“支付宝”、“VIP”、“IMPERIAL_VIP”，需要在CustomerID后面用红色显示“金帐户”
                     * 原有程序如下,启用时需修改****/


            //aList = CustomerDA.GetCustomerList(queryEntity);
            //List<ThirdPartyUser> thirdPartUserList = GetThirdPartyUserInfo(customerSysNoList);
            //if (thirdPartUserList.Count > 0)
            //{
            //    aList.ForEach( current=>
            //    {
            //        ThirdPartyUser thirdPartyUser = thirdPartUserList.FirstOrDefault(p => p.CustomerSysNo == current.SystemNumber);
            //        if (thirdPartyUser != null && thirdPartyUser.SubSource != null)
            //        {
            //            current.CustomerIDWithAlipayVipIndentity
            //            if (thirdPartyUser.UserSource == "支付宝" && (thirdPartyUser.SubSource.ToUpper() == "VIP" || thirdPartyUser.SubSource.ToUpper() == "IMPERIAL_VIP"))
            //            {
            //                current.CustomerIDWithAlipayVipIndentity = string.Format("{0}{1}",current.CustomerID,"金帐户");
            //                current.CustomerID = string.Format("{0}<br><font color='red'>【{1}】</font>", current.CustomerID, "金帐户");
            //            }
            //        }
            //    });
            //}
            #endregion
        }

        /// <summary>
        /// 获取第三方账户信息
        /// </summary>
        /// <param name="customerSysNoList"></param>
        /// <returns></returns>
        public virtual List<ThirdPartyUser> GetThirdPartyUserInfo(List<int?> customerSysNoList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" (-1,");
            foreach (int item in customerSysNoList)
            {
                sb.Append(item.ToString());
                sb.Append(",");
            }
            if (sb.ToString().EndsWith(","))
            {
                sb = sb.Remove(sb.ToString().Length - 1, 1);
            }
            sb.Append(" )");
            DataCommand dc = DataCommandManager.GetDataCommand("Customer_Get_ThirdPartyUserInfo");
            dc.ReplaceParameterValue("#StrWhere#", sb.ToString());
            return dc.ExecuteEntityList<ThirdPartyUser>();

        }

        /// <summary>
        /// 查询条件设置
        /// </summary>
        /// <param name="queryEntity"></param>
        /// <param name="cmd"></param>
        /// <param name="sqlBuilder"></param>
        private void AddParameterCustomerList(ECCentral.QueryFilter.Customer.CustomerQueryFilter queryEntity, CustomDataCommand cmd, DynamicQuerySqlBuilder sqlBuilder)
        {
            if (queryEntity.CustomerSysNo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "C.SysNo", DbType.Int32, "@SystemNumber",
                        QueryConditionOperatorType.Equal,
                        queryEntity.CustomerSysNo);
            }

            if (!string.IsNullOrEmpty(queryEntity.CustomerID))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "C.CustomerID", DbType.String, "@CustomerID",
                   QueryConditionOperatorType.LeftLike,
                   queryEntity.CustomerID);

                #region 中蛋AZCustomerID
                //sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.OR, "C.CustomerID LIKE @AZCustomerID");
                //cmd.AddInputParameter("@AZCustomerID", DbType.String, string.Format("{0}%", "AstraZeneca-" + queryEntity.CustomerID));
                #endregion

                //sqlBuilder.ConditionConstructor.EndGroupCondition();
            }

            if (!string.IsNullOrEmpty(queryEntity.Email))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.Email", DbType.String, "@EmailQueryParameter",
                    QueryConditionOperatorType.LeftLike,
                    queryEntity.Email);
            }
            if (!string.IsNullOrEmpty(queryEntity.IdentityCard))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.IdentityCard", DbType.String, "@IdentityCard",
                    QueryConditionOperatorType.Like,
                    queryEntity.IdentityCard);
            }

            if (!string.IsNullOrEmpty(queryEntity.Phone))
            {
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                    "C.Phone", DbType.String, "@Phone",
                    QueryConditionOperatorType.LeftLike,
                    queryEntity.Phone);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                    "C.CellPhone", DbType.String, "@CellPhone",
                    QueryConditionOperatorType.LeftLike,
                    queryEntity.Phone);

                sqlBuilder.ConditionConstructor.EndGroupCondition();
            }


            if (!string.IsNullOrEmpty(queryEntity.CustomerName))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.CustomerName", DbType.String, "@CustomerName",
                    QueryConditionOperatorType.LeftLike,
                    queryEntity.CustomerName);
            }

            if (queryEntity.Status.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal,
                    queryEntity.Status);
            }

            if (queryEntity.IsVip.HasValue)
            {
                if (queryEntity.IsVip.Value == CustomerVipOnly.VIP)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "C.VIPRank", DbType.Int32, new List<int>() { 2, 4 });
                }
                else if (queryEntity.IsVip.Value == CustomerVipOnly.Ordinary)
                {
                    sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "C.VIPRank", DbType.Int32, new List<int>() { 1, 3 });
                }
            }

            if (!string.IsNullOrEmpty(queryEntity.Address))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                 "C.DwellAddress", DbType.String, "@Address",
                 QueryConditionOperatorType.LeftLike,
                 queryEntity.Address);
            }
            if (queryEntity.IsPhoneConfirmed.HasValue)
            {
                if (queryEntity.IsPhoneConfirmed.Value == 0)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND,
                        "confirm.SysNo", QueryConditionOperatorType.IsNull);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND,
                        "confirm.SysNo", QueryConditionOperatorType.IsNotNull);
                }

            }

            if (queryEntity.IsEmailConfirmed.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.IsEmailConfirmed", DbType.Int32, "@IsEmailConfirmed",
                    QueryConditionOperatorType.Equal,
                    queryEntity.IsEmailConfirmed);
            }


            if (!string.IsNullOrEmpty(queryEntity.FromLinkSource))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "C.FromLinkSource", DbType.String, "@FromLinkSource",
                   QueryConditionOperatorType.LeftLike,
                   queryEntity.FromLinkSource);
            }

            if (!string.IsNullOrEmpty(queryEntity.RecommendedByCustomerID))
            {

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "C.RecommendedByCustomerID", DbType.String, "@RecommendedByCustomerID",
                   QueryConditionOperatorType.LeftLike,
                   queryEntity.RecommendedByCustomerID);
            }



            if (queryEntity.AvtarImageStatus.HasValue)
            {
                if (queryEntity.AvtarImageStatus == AvtarShowStatus.NotSet)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "E.AvtarImageStatus", DbType.AnsiStringFixedLength, "@AvtarImageStatus",
                        QueryConditionOperatorType.IsNull,
                        "NULL");
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "E.AvtarImageStatus", DbType.AnsiStringFixedLength, "@AvtarImageStatus",
                        QueryConditionOperatorType.Equal,
                        queryEntity.AvtarImageStatus == AvtarShowStatus.NotShow ? "D" : "A");
                }
            }

            if (queryEntity.CustomersType.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                              "C.CustomersType", DbType.Int32, "@CustomersType",
                              QueryConditionOperatorType.Equal,
                              queryEntity.CustomersType);
            }

            //数据库不支持ChannelSysNo
            if (queryEntity.ChannelSysNo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                              "E.ChannelMasterSysNo", DbType.Int32, "@ChannelMasterSysNo",
                              QueryConditionOperatorType.Equal,
                              queryEntity.ChannelSysNo);
            }
            #region 注册时间
            if (queryEntity.RegisterTimeFrom.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.RegisterTime", DbType.DateTime, "@RegisterTimeFrom",
                         QueryConditionOperatorType.MoreThanOrEqual,
                         queryEntity.RegisterTimeFrom);
            }

            if (queryEntity.RegisterTimeTo.HasValue)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.RegisterTime", DbType.DateTime, "@RegisterTimeTo",
                    QueryConditionOperatorType.LessThan,
                    queryEntity.RegisterTimeTo.Value);
            }
            #endregion

            #region 购买数量
            if (queryEntity.IsBuyCountCheck)
            {
                if (queryEntity.IsBuyCountRadio == 0)
                {
                    QueryConditionOperatorType operatorType = QueryConditionOperatorType.Equal;
                    if (queryEntity.OperationSign.HasValue)
                    {
                        switch (queryEntity.OperationSign.Value)
                        {
                            case OperationSignType.Equal:
                                operatorType = QueryConditionOperatorType.Equal;
                                break;
                            case OperationSignType.MoreThanOrEqual:
                                operatorType = QueryConditionOperatorType.MoreThanOrEqual;
                                break;
                            case OperationSignType.LessThanOrEqual:
                                operatorType = QueryConditionOperatorType.LessThanOrEqual;
                                break;
                            case OperationSignType.MoreThan:
                                operatorType = QueryConditionOperatorType.MoreThan;
                                break;
                            case OperationSignType.LessThan:
                                operatorType = QueryConditionOperatorType.LessThan;
                                break;
                            case OperationSignType.NotEqual:
                                operatorType = QueryConditionOperatorType.NotEqual;
                                break;
                        }
                    }
                    if (queryEntity.BuyCountValue.HasValue)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                               "ISNULL(E.BuyCount,0)", DbType.Int32, "@BuyCountValue",
                                operatorType,
                                queryEntity.BuyCountValue);
                    }
                }
                else
                {
                    if (queryEntity.BuyCountBeginPoint.HasValue)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                              "ISNULL(E.BuyCount,0)", DbType.Int32, "@BuyCountBeginPoint",
                              QueryConditionOperatorType.MoreThanOrEqual,
                              queryEntity.BuyCountBeginPoint);
                    }

                    if (queryEntity.BuyCountEndPoint.HasValue)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                              "ISNULL(E.BuyCount,0)", DbType.Int32, "@BuyCountEndPoint",
                              QueryConditionOperatorType.LessThanOrEqual,
                              queryEntity.BuyCountEndPoint);
                    }
                }
            }
            #endregion
            if (!string.IsNullOrEmpty(queryEntity.VipCardNo))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.MemberShipCard", DbType.String, "@MemberShipCard",
                    QueryConditionOperatorType.Equal,
                    queryEntity.VipCardNo);
            }
            #region 社团ID
            if (queryEntity.SocietyID.HasValue && queryEntity.SocietyID.Value > 0)
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "C.SocietyID", DbType.Int32, "@SocietyID",
                QueryConditionOperatorType.Equal,
                queryEntity.SocietyID);

            }
            #endregion
            cmd.CommandText = sqlBuilder.BuildQuerySql();
        }
        #endregion

        #region 经验值历史
        /// <summary>
        /// 经验值历史
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryCustomerExperience(ECCentral.QueryFilter.Customer.CustomerExperienceLogQueryFilter queryCriteria, out int totalCount)
        {
            totalCount = 0;
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCustomerExperienceLog");

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            pagingInfo.SortField = queryCriteria.PagingInfo.SortBy;
            pagingInfo.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            pagingInfo.MaximumRows = queryCriteria.PagingInfo.PageSize;

            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand,
                pagingInfo,
                "a.CreateTime DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "a.CustomerSysNo",
                 DbType.Int32,
                 "@CustomerSysNo",
                 QueryConditionOperatorType.Equal,
                 queryCriteria.CustomerSysNo);

                if (queryCriteria.CreateTimeFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CreateTime", DbType.DateTime, "@CreateTimeFrom",
                         QueryConditionOperatorType.MoreThanOrEqual,
                         queryCriteria.CreateTimeFrom);
                }

                if (queryCriteria.CreateTimeTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "a.CreateTime", DbType.DateTime, "@CreateTimeTo",
                       QueryConditionOperatorType.LessThan,
                       queryCriteria.CreateTimeTo.Value);
                }

                customCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataSet ds = customCommand.ExecuteDataSet();
                if (ds != null && ds.Tables.Count > 0)
                {
                    totalCount = int.Parse(customCommand.GetParameterValue("TotalCount").ToString());
                    return ds.Tables[0];
                }
            }
            return null;
        }
        #endregion

        #region 积分历史查询

        /// <summary>
        /// 积分历史查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryCustomerPointLog(ECCentral.QueryFilter.Customer.CustomerPointLogQueryFilter queryCriteria, out int totalCount)
        {
            totalCount = 0;
            DataTable dt = null;
            if (queryCriteria.IsCashPoint.HasValue && queryCriteria.IsCashPoint.Value == YNStatus.Y)
            {
                //现金转积分（只有获得历史）
                dt = QueryCustomerPointLogByForOther(queryCriteria, out totalCount);
            }
            else
            {
                //queryCriteria.ResultType=1 获得历史，queryCriteria.ResultType=-1 消费历史
                dt = QueryCustomerPointLogForAuto(queryCriteria, out totalCount);
            }
            return dt;
        }

        /// <summary>
        /// 积分历史记录
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private DataTable QueryCustomerPointLogForAuto(ECCentral.QueryFilter.Customer.CustomerPointLogQueryFilter queryCriteria, out int totalCount)
        {

            totalCount = 0;
            CustomDataCommand customCommand = null;
            string orderField = string.Empty;

            if (queryCriteria.ResultType == 1)
            {
                //获取历史
                customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPointObtainLog");
                orderField = "a.SysNo asc";
            }
            else
            {
                //消费历史
                customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryPointConsumeLog");
                orderField = "a.SysNo desc";
            }

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            pagingInfo.SortField = queryCriteria.PagingInfo.SortBy;
            pagingInfo.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            pagingInfo.MaximumRows = queryCriteria.PagingInfo.PageSize;



            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand,
                pagingInfo,
                orderField))
            {
                if (queryCriteria.CustomerSysNo.HasValue && queryCriteria.CustomerSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                   QueryConditionRelationType.AND,
                                   "a.CustomerSysNo",
                                   DbType.Int32,
                                   "@CustomerSysNo",
                                   QueryConditionOperatorType.Equal,
                                   queryCriteria.CustomerSysNo);
                }

                if (queryCriteria.OrderSysNo.HasValue && queryCriteria.OrderSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "a.SoSysNo",
                                  DbType.Int32,
                                  "@SoSysNo",
                                  QueryConditionOperatorType.Equal,
                                  queryCriteria.OrderSysNo);

                }

                if (queryCriteria.PointType.HasValue)
                {
                    string fieldName = queryCriteria.ResultType == 1 ? "a.ObtainType" : "a.ConsumeType";
                    string parameterName = queryCriteria.ResultType == 1 ? "@ObtainType" : "@ConsumeType";
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  fieldName,
                                  DbType.Int32,
                                  parameterName,
                                  QueryConditionOperatorType.Equal,
                                  queryCriteria.PointType);
                }
                if (queryCriteria.IsUseCreateDate)
                {
                    if (queryCriteria.CreateTimeFrom.HasValue)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.Indate", DbType.DateTime, "@CreateTimeFrom",
                             QueryConditionOperatorType.MoreThanOrEqual,
                             queryCriteria.CreateTimeFrom);
                    }

                    if (queryCriteria.CreateTimeTo.HasValue)
                    {
                        //选择的DateTime只有日期部分，查询包含当日的信息所以日期加1
                        //2012-1-1 --- 2012-1-2  则为 >= 2012-1-1 0:0:0  and  < 2012-1-3 0:0:0
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.Indate", DbType.DateTime, "@CreateTimeTo",
                            QueryConditionOperatorType.LessThan,
                            queryCriteria.CreateTimeTo.Value);
                    }
                }

                //if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(
                //        QueryConditionRelationType.AND,
                //        "a.CompanyCode",
                //        DbType.AnsiStringFixedLength,
                //        "@CompanyCode",
                //        QueryConditionOperatorType.Equal,
                //        queryCriteria.CompanyCode);
                //}

                customCommand.CommandText = sqlBuilder.BuildQuerySql();

                DataTable dt = customCommand.ExecuteDataTable();
                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["PointLogTypeName"] = CodeNamePairManager.GetName("Customer", "AdjustPointType", dr["pointlogtype"].ToString());
                    }
                }

                totalCount = int.Parse(customCommand.GetParameterValue("TotalCount").ToString());
                return dt;
            }
        }

        /// <summary>
        /// 现金转积分 的 积分历史记录
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private DataTable QueryCustomerPointLogByForOther(ECCentral.QueryFilter.Customer.CustomerPointLogQueryFilter queryCriteria, out int totalCount)
        {
            totalCount = 0;
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryOtherPointLog");

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            pagingInfo.SortField = queryCriteria.PagingInfo.SortBy;
            pagingInfo.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            pagingInfo.MaximumRows = queryCriteria.PagingInfo.PageSize;

            using (var sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand,
                pagingInfo,
                "a.SysNo asc"))
            {
                if (queryCriteria.CustomerSysNo.HasValue && queryCriteria.CustomerSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "a.CustomerSysNo",
                                  DbType.Int32,
                                  "@CustomerSysNo",
                                  QueryConditionOperatorType.Equal,
                                  queryCriteria.CustomerSysNo);

                }

                if (queryCriteria.PointType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "ObtainType",
                                  DbType.Int32,
                                  "@ObtainType",
                                  QueryConditionOperatorType.Equal,
                                  queryCriteria.PointType);
                }

                if (queryCriteria.IsUseCreateDate)
                {
                    if (queryCriteria.CreateTimeFrom.HasValue)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.Indate", DbType.DateTime, "@CreateTimeFrom",
                             QueryConditionOperatorType.MoreThanOrEqual,
                             queryCriteria.CreateTimeFrom);
                    }

                    if (queryCriteria.CreateTimeTo.HasValue)
                    {
                        //选择的DateTime只有日期部分，查询包含当日的信息所以日期加1
                        //2012-1-1 --- 2012-1-2  则为 >= 2012-1-1 0:0:0  and  < 2012-1-3 0:0:0
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.Indate", DbType.DateTime, "@CreateTimeTo",
                            QueryConditionOperatorType.LessThan,
                            queryCriteria.CreateTimeTo.Value);
                    }
                }

                //现金转积分
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, @"
                    (([ObtainType] = 19 
			        AND (memo ='客户多汇款转积分' 
					        OR memo = '客户多付款－产品调价' 
					        OR memo = '客户多付款－拆单/运费转积分' 
					        OR memo = '客户多付款－客户作废/更改订单需要转积分')) 
			        OR [ObtainType] IN (39,37,44)) ");

                //if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(
                //        QueryConditionRelationType.AND,
                //        "a.CompanyCode",
                //        DbType.AnsiStringFixedLength,
                //        "@CompanyCode",
                //        QueryConditionOperatorType.Equal,
                //        queryCriteria.CompanyCode);
                //}

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = customCommand.ExecuteDataTable();
                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["PointLogTypeName"] = CodeNamePairManager.GetName("Customer", "AdjustPointType", dr["pointlogtype"].ToString());
                    }
                }
                totalCount = int.Parse(customCommand.GetParameterValue("TotalCount").ToString());
                return dt;
            }
        }

        #endregion

        #region 恶意用户操作历史

        /// <summary>
        /// 恶意用户操作历史
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual DataTable QueryMaliceCustomerLog(int customerSysNo)
        {
            var customCommand = DataCommandManager.GetDataCommand("GetCustomerInfoOperateLog");

            //PagingInfoEntity pagingInfo = new PagingInfoEntity();
            //pagingInfo.SortField = filter.PagingInfo.SortBy;
            //pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            //pagingInfo.MaximumRows = filter.PagingInfo.PageSize;
            customCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            var ds = customCommand.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }
        #endregion

        /// <summary>
        /// 获取用户的权限
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual DataTable GetCustomerRight(CustomerQueryFilter filter)
        {
            var customCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetALLCustomerRight");

            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            pagingInfo.SortField = filter.PagingInfo.SortBy;
            pagingInfo.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            pagingInfo.MaximumRows = filter.PagingInfo.PageSize;

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(customCommand.CommandText, customCommand, pagingInfo, null))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CustomerID", DbType.String, "@CustomerID",
                    QueryConditionOperatorType.Equal, filter.CustomerID);

                customCommand.CommandText = sqlBuilder.BuildQuerySql();
                var ds = customCommand.ExecuteDataSet();
                if (ds != null && ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 获取客户安全密保问题
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public DataTable GetSecurityQuestion(int customerSysNo)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_Get_SecurityQuestion");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteDataTable();
        }
        public List<SocietyInfo> GetSocieties()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Customer_Get_SocietyInfo");
            return dc.ExecuteEntityList<SocietyInfo>();
        }
    }
}
