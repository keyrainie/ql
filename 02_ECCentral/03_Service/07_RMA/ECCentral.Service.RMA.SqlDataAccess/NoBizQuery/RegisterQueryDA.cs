using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.Utility.DataAccess;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IRegisterQueryDA))]
    public class RegisterQueryDA : IRegisterQueryDA
    {
        #region IRegisterQueryDA Members

        public DataTable QueryRegister(RegisterQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            CustomDataCommand cmd = null;
            if (filter.IsRepeatRegister.HasValue && filter.IsRepeatRegister.Value)
            {
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRegistersForRepeatRegister");
            }
            else
            {
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRegisters");
            }
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " register.SysNo DESC "))
            {
                #region build conditions

                #region

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "request.CustomerSysNo",
                    DbType.Int32,
                    "@CustomerSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.CustomerSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.ProductSysNo",
                    DbType.Int32,
                    "@ProductSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.ProductSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "request.RequestID",
                    DbType.String,
                    "@RequestID",
                    QueryConditionOperatorType.Equal,
                    filter.RequestID
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.IsRecommendRefund",
                    DbType.Int32,
                    "@IsRecommendRefund",
                    QueryConditionOperatorType.Equal,
                    filter.IsRecommendRefund
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.RequestType",
                    DbType.Int32,
                    "@RequestTypeCode",
                    QueryConditionOperatorType.Equal,
                    filter.RequestType
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "request.SOSysNo",
                    DbType.Int32,
                    "@SOSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.SOSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.SysNo",
                    DbType.Int32,
                    "@SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.RegisterSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "customer.CustomerID",
                    DbType.String,
                    "@CustomerID",
                    QueryConditionOperatorType.Equal,
                    filter.CustomerID
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "request.CreateTime",
                    DbType.DateTime,
                    "@CreateTimeFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.CreateTimeFrom
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "request.CreateTime",
                    DbType.DateTime,
                    "@CreateTimeTo",
                    QueryConditionOperatorType.LessThanOrEqual,
                    filter.CreateTimeTo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.RevertStatus",
                    DbType.Int32,
                    "@RevertStatusCode",
                    QueryConditionOperatorType.Equal,
                    filter.RevertStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.NewProductStatus",
                    DbType.Int32,
                    "@NewProductStatusCode",
                    QueryConditionOperatorType.Equal,
                    filter.NewProductStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.OutBoundStatus",
                    DbType.Int32,
                    "@OutBoundStatusCode",
                    QueryConditionOperatorType.Equal,
                    filter.OutBoundStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.ReturnStatus",
                    DbType.Int32,
                    "@ReturnStatusCode",
                    QueryConditionOperatorType.Equal,
                    filter.ReturnStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.Status",
                    DbType.Int32,
                    "@StatusCode",
                    QueryConditionOperatorType.Equal,
                    filter.RequestStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.RefundStatus",
                    DbType.Int32,
                    "@RefundStatusCode",
                    QueryConditionOperatorType.Equal,
                    filter.RefundStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.NextHandler",
                    DbType.Int32,
                    "@NextHandlerCode",
                    QueryConditionOperatorType.Equal,
                    filter.NextHandler
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.IsWithin7Days",
                    DbType.Int32,
                    "@IsWithin7Days",
                    QueryConditionOperatorType.Equal,
                    filter.IsWithin7Days
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.RMAReason",
                    DbType.Int32,
                    "@RMAReason",
                    QueryConditionOperatorType.Equal,
                    filter.RMAReason
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "product.PMUserSysNo",
                    DbType.Int32,
                    "@PMUserSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.PMUserSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "category3.Category1SysNo",
                    DbType.Int32,
                    "@C1SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.Category1SysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "category3.Category2SysNo",
                    DbType.Int32,
                    "@C2SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.Category2SysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "category3.Category3SysNo",
                    DbType.Int32,
                    "@C3SysNo",
                    QueryConditionOperatorType.Equal,
                    filter.Category3SysNo
                );

                //销售方
                if (filter.SellersType != null)
                {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "request.InvoiceType",
                            DbType.AnsiStringFixedLength,
                             "@InvoiceType",
                            QueryConditionOperatorType.Equal,
                             filter.SellersType
                                                                  );
                    
                }
                //是否已打印标签
                if (filter.IsLabelPrinted.HasValue)
                {
                    string isLabelPrinted = string.Empty;
                    if (filter.IsLabelPrinted == false)
                    {
                        isLabelPrinted = "N";
                    }
                    else if (filter.IsLabelPrinted == true)
                    {
                        isLabelPrinted = "Y";
                    }

                    sqlBuilder.ConditionConstructor.AddCondition
                        (
                          QueryConditionRelationType.AND,
                        "request.IsLabelPrinted",
                        DbType.String,
                        "@IsLabelPrinted",
                        QueryConditionOperatorType.Equal,
                        isLabelPrinted
                        );
                }
                //商家处理结果
                sqlBuilder.ConditionConstructor.AddCondition
                   (
                     QueryConditionRelationType.AND,
                   "register.SellerOperationInfo",
                   DbType.String,
                   "@SellerOperationInfo",
                   QueryConditionOperatorType.Equal,
                   filter.SellerOperationInfo
                   );

                if (filter.IsRepeatRegister.HasValue)
                {
                    if (filter.IsRepeatRegister.Value)
                    {
                        QueryConditionOperatorType qcOperator = QueryConditionOperatorType.MoreThan;
                        if (filter.CompareSymbol == CompareSymbolType.Equal)
                        {
                            qcOperator = QueryConditionOperatorType.Equal;
                        }
                        else if (filter.CompareSymbol == CompareSymbolType.LessThan)
                        {
                            qcOperator = QueryConditionOperatorType.LessThan;
                        }
                        else if (filter.CompareSymbol == CompareSymbolType.MoreThan)
                        {
                            qcOperator = QueryConditionOperatorType.MoreThan;
                        }
                        sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "Repeat.RepeatProductCount",
                        DbType.Int32,
                        "@RepeatProductCount",
                        qcOperator,
                        filter.ProductCount);

                    }
                    else
                    {
                        string feedBackCondition1 = string.Empty;
                        string feedBackCondition2 = string.Empty;

                        feedBackCondition1 = @"SELECT 1
                                              FROM [OverseaServiceManagement].[dbo].[RepeatRegister] WITH(NOLOCK)
                                            WHERE SOSysNo = request.SOSysNo 
	                                            AND [ProductSysNo] = register.ProductSysNo 
	                                            AND [CurrentCount] = 1";

                        feedBackCondition2 = @"SELECT 1
                                              FROM [OverseaServiceManagement].[dbo].[RepeatRegister] WITH(NOLOCK)
                                            WHERE SOSysNo = request.SOSysNo 
	                                            AND [ProductSysNo] = register.ProductSysNo";

                        using (GroupCondition group = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                        {
                            sqlBuilder.ConditionConstructor.AddSubQueryCondition(
                                                                   QueryConditionRelationType.AND,
                                                                   null,
                                                                   QueryConditionOperatorType.Exist,
                                                                   feedBackCondition1
                                                                   );

                            sqlBuilder.ConditionConstructor.AddSubQueryCondition(
                                                QueryConditionRelationType.OR,
                                                null,
                                                QueryConditionOperatorType.NotExist,
                                                feedBackCondition2
                                                );
                        }
                    }
                }

                if (!string.IsNullOrEmpty(filter.NextHandlerList) && filter.NextHandler == RMANextHandler.Dun)
                {
                    string feedBackCondition = string.Empty;
                    feedBackCondition = "SELECT 1 FROM dbo.RMA_Register_Dunlog Dunlog WITH(NOLOCK) WHERE Dunlog.RegisterSysNo = register.SysNo"
                        + " AND Dunlog.FeedBack IN (" + filter.NextHandlerList + ")";
                    ConditionConstructor subCondition = sqlBuilder.ConditionConstructor.AddSubQueryCondition(
                                        QueryConditionRelationType.AND,
                                        null,
                                        QueryConditionOperatorType.Exist,
                                        feedBackCondition
                                        );

                }

                #endregion

                #region DateTime conditions

                if (filter.IsUnReceive.HasValue && filter.IsUnReceive.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "request.RecvTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.RecvTime",
                        DbType.DateTime,
                        "@RecvTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.RecvTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.RecvTime",
                        DbType.DateTime,
                        "@RecvTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.RecvTimeTo
                    );
                }

                if (filter.IsUnReturn.HasValue && filter.IsUnReturn.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "rmareturn.ReturnTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rmareturn.ReturnTime",
                        DbType.DateTime,
                        "@ReturnTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.ReturnTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rmareturn.ReturnTime",
                        DbType.DateTime,
                        "@ReturnTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.ReturnTimeTo
                    );
                }

                if (filter.IsUnRefund.HasValue && filter.IsUnRefund.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "refund.RefundTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "refund.RefundTime",
                        DbType.DateTime,
                        "@RefundTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.RefundTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "refund.RefundTime",
                        DbType.DateTime,
                        "@RefundTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.RefundTimeTo
                    );
                }

                if (filter.IsUnResponse.HasValue && filter.IsUnResponse.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "register.ResponseTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "register.ResponseTime",
                        DbType.DateTime,
                        "@ResponseTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.ResponseTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "register.ResponseTime",
                        DbType.DateTime,
                        "@ResponseTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.ResponseTimeTo
                    );
                }

                if (filter.IsUnOutbound.HasValue && filter.IsUnOutbound.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "outbound.OutTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "outbound.OutTime",
                        DbType.DateTime,
                        "@OutBoundTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.OutboundTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "outbound.OutTime",
                        DbType.DateTime,
                        "@OutBoundTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.OutboundTimeTo
                    );
                }

                if (filter.IsUnCheck.HasValue && filter.IsUnCheck.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "register.CheckTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "register.CheckTime",
                        DbType.DateTime,
                        "@CheckTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.CheckTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "register.CheckTime",
                        DbType.DateTime,
                        "@CheckTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.CheckTimeTo
                    );
                }

                if (filter.IsUnRevert.HasValue && filter.IsUnRevert.Value)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND, "rmarevert.OutTime", QueryConditionOperatorType.IsNull
                    );
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rmarevert.OutTime",
                        DbType.DateTime,
                        "@RevertTimeFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        filter.RevertTimeFrom
                    );
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "rmarevert.OutTime",
                        DbType.DateTime,
                        "@RevertTimeTo",
                        QueryConditionOperatorType.LessThanOrEqual,
                        filter.RevertTimeTo
                    );
                }

                #endregion

                #region VIP condition

                if (filter.IsVIP.HasValue)
                {
                    using (GroupCondition group = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        if (filter.IsVIP == true)
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.AND,
                                "customer.VIPRank",
                                DbType.Int32,
                                "@AutoVIP",
                                QueryConditionOperatorType.Equal,
                                2    // VIPRank.AutoVIP
                            );
                            sqlBuilder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.OR,
                                "customer.VIPRank",
                                DbType.Int32,
                                "@ManualVIP",
                                QueryConditionOperatorType.Equal,
                                4    // VIPRank.ManualVIP
                            );
                        }
                        else
                        {
                            sqlBuilder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.AND,
                                "customer.VIPRank",
                                DbType.Int32,
                                "@AutoNonVIP",
                                QueryConditionOperatorType.Equal,
                                1    // VIPRank.AutoNonVIP
                            );
                            sqlBuilder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.OR,
                                "customer.VIPRank",
                                DbType.Int32,
                                "@ManualNonVIP",
                                QueryConditionOperatorType.Equal,
                                3    // VIPRank.ManualNonVIP
                            );
                        }
                    }
                }

                #endregion
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.CompanyCode",
                    DbType.String,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    filter.CompanyCode
                    );
                #endregion
                cmd.CommandText = MakeSqlWithCondition(filter, sqlBuilder.BuildQuerySql());
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OutBoundStatus", typeof(RMAOutBoundStatus));
                enumList.Add("RevertStatus", typeof(RMARevertStatus));
                enumList.Add("RefundStatus", typeof(RMARefundStatus));
                enumList.Add("ReturnStatus", typeof(RMAReturnStatus));
                enumList.Add("NewProductStatus", typeof(RMANewProductStatus));
                enumList.Add("Status", typeof(RMARequestStatus));
                enumList.Add("InvoiceType", typeof(SellersType));

                CodeNamePairColumnList codeNameList = new CodeNamePairColumnList();
                codeNameList.Add("RMAReason", "RMA", "RMAReason");
                codeNameList.Add("SellerOperationInfo", "RMA", "SellerOperationInfo");
                var dt = cmd.ExecuteDataTable(enumList, codeNameList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private string MakeSqlWithCondition(RegisterQueryFilter filter, string originSql)
        {
            if (filter == null)
            {
                return originSql;
            }

            string sql = originSql;


            sql = Regex.Replace(
                sql,
                "#OutBoundTable#",
                GetOutBoundTableClause(
                    filter.OutboundTimeFrom.HasValue ||
                    filter.OutboundTimeTo.HasValue ||
                    (filter.IsUnOutbound.HasValue && filter.IsUnOutbound.Value)
                ),
                RegexOptions.IgnoreCase
            );
            sql = Regex.Replace(
                sql,
                "#RefundTable#",
                GetRefundTableClause(
                    filter.RefundTimeFrom.HasValue ||
                    filter.RefundTimeTo.HasValue ||
                    (filter.IsUnRefund.HasValue && filter.IsUnRefund.Value)
                ),
                RegexOptions.IgnoreCase
            );
            sql = Regex.Replace(
                sql,
                "#ReturnTable#",
                GetReturnTableClause(
                    filter.ReturnTimeFrom.HasValue ||
                    filter.ReturnTimeTo.HasValue ||
                    (filter.IsUnReturn.HasValue && filter.IsUnReturn.Value)
                ),
                RegexOptions.IgnoreCase
            );


            return sql;
        }

        private string GetOutBoundTableClause(bool nested)
        {
            return nested
                ? DataCommandManager.CreateCustomDataCommandFromConfig("OutBoundTableClause").CommandText
                : "";
        }

        private string GetRefundTableClause(bool nested)
        {
            return nested
                ? DataCommandManager.CreateCustomDataCommandFromConfig("RefundTableClause").CommandText
                : "";
        }

        private string GetReturnTableClause(bool nested)
        {
            return nested
                ? DataCommandManager.CreateCustomDataCommandFromConfig("ReturnTableClause").CommandText
                : "";
        }

        public List<RegisterDunLog> QueryRegisterDunLog(int registerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRegisterDunlog");
            cmd.SetParameterValue("@RegisterSysNo", registerSysNo);

            return cmd.ExecuteEntityList<RegisterDunLog>();
        }
        #endregion
    }
}
