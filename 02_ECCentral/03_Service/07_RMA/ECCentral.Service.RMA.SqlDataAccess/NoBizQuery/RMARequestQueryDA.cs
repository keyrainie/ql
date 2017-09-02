using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRMARequestQueryDA))]
    public class RMARequestQueryDA : IRMARequestQueryDA
    {
        public DataTable QueryRMARequest(RMARequestQueryFilter filter, out int totalCount)
        {
            if (filter == null)
            {
                totalCount = -1;
                return null;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRequests");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "request.SysNo DESC"))
            {
                #region build query conditions

                if (filter.CreateDateFrom != null)
                {
                    builder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "request.CreateTime", DbType.DateTime, "@CreateTimeFrom",
                              QueryConditionOperatorType.MoreThanOrEqual,
                              filter.CreateDateFrom);
                }
                if (filter.CreateDateTo != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.CreateTime", DbType.DateTime, "@CreateTimeTo",
                               QueryConditionOperatorType.LessThan,
                               filter.CreateDateTo);
                }

                if (filter.ReceivedDateFrom != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.RecvTime", DbType.DateTime, "@RecvTimeFrom",
                               QueryConditionOperatorType.MoreThanOrEqual,
                               filter.ReceivedDateFrom);
                }

                if (filter.ReceivedDateTo != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.RecvTime", DbType.DateTime, "@RecvTimeTo",
                               QueryConditionOperatorType.LessThan,
                               filter.ReceivedDateTo);
                }

                if (filter.ETakeDateFrom != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.ETakeDate", DbType.DateTime, "@ETakeDateFrom",
                               QueryConditionOperatorType.MoreThanOrEqual,
                               filter.ETakeDateFrom);
                }

                if (filter.ETakeDateTo != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.ETakeDate", DbType.DateTime, "@ETakeDateTo",
                               QueryConditionOperatorType.LessThan,
                               filter.ETakeDateTo);
                }
                if (!string.IsNullOrEmpty(filter.SOID))
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "so.SOID", DbType.String, "@SOID",
                               QueryConditionOperatorType.Equal,
                               filter.SOID);

                }

                if (!string.IsNullOrEmpty(filter.RequestID))
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.RequestID", DbType.String, "@RequestID",
                               QueryConditionOperatorType.Equal,
                               filter.RequestID);
                }


                if (filter.IsVIP.HasValue)
                {
                    using (GroupCondition group = new GroupCondition(builder, QueryConditionRelationType.AND))
                    {
                        if (filter.IsVIP.Value)
                        {
                            builder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.AND,
                                "customer.VIPRank",
                                DbType.Int32,
                                "@AutoVIP",
                                QueryConditionOperatorType.Equal,
                                2    // VIPRank.AutoVIP
                            );
                            builder.ConditionConstructor.AddCondition(
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
                            builder.ConditionConstructor.AddCondition(
                                QueryConditionRelationType.AND,
                                "customer.VIPRank",
                                DbType.Int32,
                                "@AutoNonVIP",
                                QueryConditionOperatorType.Equal,
                                1    // VIPRank.AutoNonVIP
                            );
                            builder.ConditionConstructor.AddCondition(
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

                if (filter.CustomerSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "customer.SysNo", DbType.Int32, "@SysNo",
                               QueryConditionOperatorType.Equal,
                               filter.CustomerSysNo);
                }

                if (!string.IsNullOrEmpty(filter.CustomerID))
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "customer.CustomerID", DbType.String, "@CustomerID",
                               QueryConditionOperatorType.Equal,
                               filter.CustomerID);
                }

                if (filter.ReceiveUserSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.RecvUserSysNo", DbType.Int32, "@RecvUserSysNo",
                               QueryConditionOperatorType.Equal,
                               filter.ReceiveUserSysNo);
                }

                if (filter.IsSubmit.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.IsSubmit",
                        DbType.Int32,
                        "@IsSubmit",
                        QueryConditionOperatorType.Equal,
                        filter.IsSubmit);
                }

                if (filter.Status.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.Status",
                        DbType.Int32,
                        "@Status",
                        QueryConditionOperatorType.Equal,
                        filter.Status
                    );
                }

                if (filter.SysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.SysNo",
                        DbType.Int32,
                        "@SysNo",
                        QueryConditionOperatorType.Equal,
                        filter.SysNo
                    );
                }

                if (filter.SOSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.SOSysNo",
                        DbType.Int32,
                        "@SOSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.SOSysNo
                    );
                }
                if (!string.IsNullOrEmpty(filter.CompanyCode))
                {
                    builder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.CompanyCode",
                        DbType.AnsiStringFixedLength,
                        "@CompanyCode",
                        QueryConditionOperatorType.Equal,
                        filter.CompanyCode
                    );
                }

                if (filter.IsLabelPrinted != null)
                {
                    string strIsLabelPrinted = "N";
                    if (filter.IsLabelPrinted.Value)
                    {
                        strIsLabelPrinted = "Y";
                    }

                    builder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "request.IsLabelPrinted",
                   DbType.AnsiStringFixedLength,
                    "@IsLabelPrinted",
                   QueryConditionOperatorType.Equal,
                    strIsLabelPrinted
                     );
                }

                if (filter.SellersType != null)
                {
                    if (filter.SellersType == SellersType.Self)
                    {
                        builder.ConditionConstructor.AddCondition(
                                              QueryConditionRelationType.AND,
                                              "request.InvoiceType",
                                              DbType.AnsiStringFixedLength,
                                               "@InvoiceType",
                                              QueryConditionOperatorType.Equal,
                                               0
                                            );
                    }
                    else
                    {
                        builder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "request.InvoiceType",
                            DbType.AnsiStringFixedLength,
                             "@InvoiceType",
                            QueryConditionOperatorType.Equal,
                             1
                                                                  );
                    }


                }

                //服务编码 add by norton 2012.11.21
                if (!string.IsNullOrEmpty(filter.ServiceCode))
                {
                    builder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "request.ServiceCode",
                        DbType.String,
                        "@ServiceCode",
                        QueryConditionOperatorType.Equal,
                       filter.ServiceCode);
                }
                if (filter.AuditUserSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "request.AuditUserSysNo",
                        DbType.Int32,
                        "@AuditUserSysNo",
                        QueryConditionOperatorType.Equal,
                        filter.AuditUserSysNo);
                }

                if (filter.AuditDateFrom != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.AuditTime", DbType.DateTime, "@AuditDateFrom",
                               QueryConditionOperatorType.MoreThanOrEqual,
                               filter.AuditDateFrom);
                }
                if (filter.AuditDateTo != null)
                {
                    builder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "request.AuditTime", DbType.DateTime, "@AuditDateTo",
                               QueryConditionOperatorType.LessThanOrEqual,
                               filter.AuditDateTo);
                }

                #endregion

                dataCommand.CommandText = builder.BuildQuerySql();

                #region 扩展字段读取
                string columnExtendColumnRMAItemSysNos = string.Empty;
                //需要读取额外的单件连接数据
                if (filter.IsReadRMAItemSysNos)
                {
                    columnExtendColumnRMAItemSysNos = string.Format(",{0}", "(SELECT [OverseaOrderManagement].[dbo].[UF_GetRMAItemSysNos] (request.SysNo)) as RMAItemSysNos");
                }
                dataCommand.CommandText = dataCommand.CommandText.Replace("#ExtendColRMAItemSysNos#", columnExtendColumnRMAItemSysNos);

                #endregion

                dataCommand.SetParameterValue("@StartNumber", filter.PagingInfo.PageSize * filter.PagingInfo.PageIndex);
                dataCommand.SetParameterValue("@EndNumber", filter.PagingInfo.PageSize * filter.PagingInfo.PageIndex + filter.PagingInfo.PageSize);

                DataTable dt = dataCommand.ExecuteDataTable("Status", typeof(RMARequestStatus));
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public List<UserInfo> GetAllReceiveUsers()
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("GetAllReceiveUsers");

            return selectCommand.ExecuteEntityList<UserInfo>();
        }

        public List<UserInfo> GetAllConfirmUsers()
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("GetAllConfirmUsers");

            return selectCommand.ExecuteEntityList<UserInfo>();
        }
    }
}
