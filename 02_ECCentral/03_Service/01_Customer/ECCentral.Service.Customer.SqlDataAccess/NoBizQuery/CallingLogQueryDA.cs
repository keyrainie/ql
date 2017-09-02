using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICallingLogQueryDA))]
    public class CallingLogQueryDA : ICallingLogQueryDA
    {

        public virtual DataTable QuerySOList(QueryFilter.Customer.CustomerCallingQueryFilter queryEntity, out int totalCount)
        {
            PagingInfoEntity pagingEntity = SetPagingInfo(queryEntity);
            CustomDataCommand cmd = null;
            if (!string.IsNullOrEmpty(queryEntity.OrderSysNo))
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySOMasterForSO");
            else
                cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySOMaster");
            using (DynamicQuerySqlBuilder queryBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "sm.OrderDate DESC"))
            {
                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "sm.UpdateUserSysNo",
                         DbType.Int32,
                        "@UpdateUserSysNo",
                        QueryConditionOperatorType.Equal,
                        queryEntity.LastUpdateUserSysNo
                        );
                #region 特殊条件构造

                if (!string.IsNullOrEmpty(queryEntity.OrderSysNo) && queryEntity.OrderSysNo.Split('.').Length >= 2)
                {
                    string querySOList = queryEntity.OrderSysNo.Replace(".", ",");
                    queryBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "sm.sysno",
                        QueryConditionOperatorType.In,
                        querySOList);
                }
                else if (!string.IsNullOrEmpty(queryEntity.OrderSysNo))
                {
                    cmd.AddInputParameter("@SONumber", DbType.Int32, queryEntity.OrderSysNo);
                }
                #endregion
                #region 动态拼装条件
                //queryBuilder.AddCondition(QueryConditionRelationType.AND,
                //                        "sm.sysno",
                //                        DbType.Int32,
                //                        "@SysNo",
                //                        QueryConditionOperatorType.Equal,
                //                        queryEntity.SystemNumber
                //                        );

                if (!string.IsNullOrEmpty(queryEntity.CustomerName))
                {
                    queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "sm.receivename LIKE @ReceiveName");
                    cmd.AddInputParameter("@ReceiveName", DbType.String, string.Format("{0}%", queryEntity.CustomerName));
                }

                queryBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                if (!string.IsNullOrEmpty(queryEntity.PhoneORCellphone))
                {
                    queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "sm.receivephone LIKE @ReceivePhone");
                    cmd.AddInputParameter("@ReceivePhone", DbType.String, string.Format("{0}%", queryEntity.PhoneORCellphone));

                    queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "sm.receivecellphone LIKE @ReceiveCellPhone");
                    cmd.AddInputParameter("@ReceiveCellPhone", DbType.String, string.Format("{0}%", queryEntity.PhoneORCellphone));

                }
                queryBuilder.ConditionConstructor.EndGroupCondition();

                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                        "sm.status",
                                        DbType.Int32,
                                        "@Status",
                                        QueryConditionOperatorType.Equal,
                                        queryEntity.SOStatus
                                        );
                if (!string.IsNullOrEmpty(queryEntity.CustomerID))
                {
                    queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "cm.CustomerID LIKE @CustomerID");
                    cmd.AddInputParameter("@CustomerID", DbType.String, string.Format("{0}%", queryEntity.CustomerID));
                }

                if (!string.IsNullOrEmpty(queryEntity.Address))
                {
                    queryBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "sm.receiveaddress LIKE @ReceiveAddress");
                    cmd.AddInputParameter("@ReceiveAddress", DbType.String, string.Format("{0}%", queryEntity.Address));
                }

                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                       "sm.OrderDate",
                                       DbType.DateTime,
                                       "@OrderDateFrom",
                                       QueryConditionOperatorType.MoreThanOrEqual,
                                       queryEntity.CreateDateFrom
                                       );

                queryBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                       "sm.OrderDate",
                                       DbType.DateTime,
                                       "@OrderDateTo",
                                       QueryConditionOperatorType.LessThan,
                                       queryEntity.CreateDateTo
                                       );

                queryBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "sm.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    queryEntity.CompanyCode);


                #endregion
                cmd.CommandText = queryBuilder.BuildQuerySql();
                EnumColumnList list = new EnumColumnList();
                list.Add(12, typeof(SOStatus));
                list.Add(18, typeof(FPCheckStatus));
                DataTable dt = cmd.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public virtual DataTable QueryRMAList(QueryFilter.Customer.CustomerCallingQueryFilter queryEntity, out int totalCount)
        {
            PagingInfoEntity pagingEntity = SetPagingInfo(queryEntity);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetRMAList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                cmd.CommandText,
                cmd,
                pagingEntity,
                "C.CreateTime DESC"))
            {
                //  sqlBuilder.ConditionConstructor.AddCondition(
                //QueryConditionRelationType.AND,
                //"A.SysNo",
                //DbType.Int32,
                //"@SysNo ",
                //QueryConditionOperatorType.Equal,
                //queryEntity.SystemNumber
                //);

                if (queryEntity.ReopenCount.HasValue)
                {
                    if (queryEntity.ReopenCount == -1)
                    {
                        sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "F.CurrentCount",
                       DbType.Int32,
                       "@CurrentCount",
                       QueryConditionOperatorType.MoreThan,
                       1);
                        sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "F.RepeatProductCount",
                       DbType.Int32,
                       "@RepeatProductCount",
                       QueryConditionOperatorType.Equal,
                       1);
                        sqlBuilder.ConditionConstructor.EndGroupCondition();

                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "F.CurrentCount",
                        DbType.Int32,
                        "@CurrentCount",
                        QueryConditionOperatorType.Equal,
                        queryEntity.ReopenCount + 1);
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(
                             QueryConditionRelationType.AND,
                             "A.Status",
                             DbType.Int32,
                             "@RMAStatus1",
                             QueryConditionOperatorType.NotEqual,
                             0
                        );
                    sqlBuilder.ConditionConstructor.AddCondition(
                         QueryConditionRelationType.AND,
                         "A.Status",
                         DbType.Int32,
                         "@RMAStatus2",
                         QueryConditionOperatorType.NotEqual,
                         -1
                    );
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                               QueryConditionRelationType.AND,
                               "G.CreateUserSysNo",
                               DbType.Int32,
                               "@LastUpdateUserSysNo",
                               QueryConditionOperatorType.Equal,
                               queryEntity.LastUpdateUserSysNo
                               );
                if (queryEntity.RMAStatus.HasValue)
                {
                    if (queryEntity.RMAStatus.Value == CallingRMAStatus.CheckProcessing)
                    {
                        if (!queryEntity.ReopenCount.HasValue)
                        {
                            sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                            sqlBuilder.ConditionConstructor.AddCondition(
                           QueryConditionRelationType.AND,
                           "F.CurrentCount",
                           DbType.Int32,
                           "@CurrentCount",
                           QueryConditionOperatorType.MoreThan,
                           1);
                            sqlBuilder.ConditionConstructor.AddCondition(
                           QueryConditionRelationType.AND,
                           "F.RepeatProductCount",
                           DbType.Int32,
                           "@RepeatProductCount",
                           QueryConditionOperatorType.Equal,
                           1);
                            sqlBuilder.ConditionConstructor.EndGroupCondition();

                            sqlBuilder.ConditionConstructor.AddCondition(
                             QueryConditionRelationType.AND,
                             "A.Status",
                             DbType.Int32,
                             "@RMAStatus1",
                             QueryConditionOperatorType.NotEqual,
                             0
                        );
                            sqlBuilder.ConditionConstructor.AddCondition(
                                 QueryConditionRelationType.AND,
                                 "A.Status",
                                 DbType.Int32,
                                 "@RMAStatus2",
                                 QueryConditionOperatorType.NotEqual,
                                 -1
                            );
                        }
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                                     QueryConditionRelationType.AND,
                                     "A.Status",
                                     DbType.Int32,
                                     "@RMAStatus",
                                     QueryConditionOperatorType.Equal,
                                     queryEntity.RMAStatus
                                );
                    }
                }

                if (!string.IsNullOrEmpty(queryEntity.OrderSysNo) && queryEntity.OrderSysNo.Split('.').Length >= 2)
                {

                    string querySOList = queryEntity.OrderSysNo.Replace(".", ",");
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "C.SOSysNo",
                        QueryConditionOperatorType.In,
                        querySOList);
                }
                else if (!string.IsNullOrEmpty(queryEntity.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "C.SOSysNo",
                         DbType.Int32,
                        "@SOID",
                        QueryConditionOperatorType.Equal,
                        int.Parse(queryEntity.OrderSysNo)
                        );
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "A.SysNo ",
                 DbType.Int32,
                 "@RegisterSysNo ",
                 QueryConditionOperatorType.Equal,
                 queryEntity.RegisterSysNo
               );


                if (!string.IsNullOrEmpty(queryEntity.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "D.CustomerID LIKE @CustomerID");
                    cmd.AddInputParameter("@CustomerID", DbType.String, string.Format("{0}%", queryEntity.CustomerID));
                }

                if (!string.IsNullOrEmpty(queryEntity.CustomerName))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "D.CustomerName LIKE @CustomerName");
                    cmd.AddInputParameter("@CustomerName", DbType.String, string.Format("{0}%", queryEntity.CustomerName));
                }

                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                if (!string.IsNullOrEmpty(queryEntity.PhoneORCellphone))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "D.CellPhone LIKE @ReceivePhone");
                    cmd.AddInputParameter("@ReceivePhone", DbType.String, string.Format("{0}%", queryEntity.PhoneORCellphone));

                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "D.CellPhone LIKE @ReceiveCellPhone");
                    cmd.AddInputParameter("@ReceiveCellPhone", DbType.String, string.Format("{0}%", queryEntity.PhoneORCellphone));
                }
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                if (!string.IsNullOrEmpty(queryEntity.Address))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "D.DwellAddress LIKE @ReceiveAddress");
                    cmd.AddInputParameter("@ReceiveAddress", DbType.String, string.Format("{0}%", queryEntity.Address));
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "C.CreateTime",
                              DbType.DateTime,
                              "@CreateDateFrom",
                              QueryConditionOperatorType.MoreThanOrEqual,
                              queryEntity.CreateDateFrom
                         );

                sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "C.CreateTime",
                              DbType.DateTime,
                              "@CreateDateTo",
                              QueryConditionOperatorType.LessThan,
                              queryEntity.CreateDateTo
                         );
                #region 关闭时间
                sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "A.CloseTime",
                              DbType.DateTime,
                              "@CloseDateFrom",
                              QueryConditionOperatorType.MoreThanOrEqual,
                              queryEntity.CloseDateFrom
                         );

                sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "A.CloseTime",
                              DbType.DateTime,
                              "@CloseDateTo",
                              QueryConditionOperatorType.LessThan,
                              queryEntity.CloseDateTo
                         );
                #endregion
                sqlBuilder.ConditionConstructor.AddCondition(
                       QueryConditionRelationType.AND,
                       "A.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                       QueryConditionOperatorType.Equal,
                       queryEntity.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(3, typeof(CallingRMAStatus));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public virtual DataTable QueryComplainList(QueryFilter.Customer.CustomerCallingQueryFilter queryEntity, out int totalCount)
        {
            PagingInfoEntity pagingEntity = SetPagingInfo(queryEntity);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetComplainList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                cmd.CommandText,
                cmd,
                pagingEntity,
                "Complain.SysNo DESC"))
            {
                if (!string.IsNullOrEmpty(queryEntity.OrderSysNo) && queryEntity.OrderSysNo.Split('.').Length >= 2)
                {

                    string querySOList = queryEntity.OrderSysNo.Replace(".", ",");
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                        "Complain.SOSysNo",
                        QueryConditionOperatorType.In,
                        querySOList);
                }
                else if (!string.IsNullOrEmpty(queryEntity.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "Complain.SOSysNo",
                         DbType.Int32,
                        "@SOID",
                        QueryConditionOperatorType.Equal,
                        int.Parse(queryEntity.OrderSysNo)
                        );
                }

                if (queryEntity.ReopenCount.HasValue && queryEntity.ReopenCount > -1
                    && !string.IsNullOrEmpty(queryEntity.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "Complain.SOSysNo",
                         DbType.Int32,
                        "@SOID",
                        QueryConditionOperatorType.Equal,
                        int.Parse(queryEntity.OrderSysNo)
                        );
                }

                //if (queryEntity.SystemNumber.HasValue)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(
                //     QueryConditionRelationType.AND,
                //     "Complain.SysNo",
                //     DbType.Int32,
                //     "@SystemNumber",
                //     QueryConditionOperatorType.Equal,
                //     queryEntity.SystemNumber
                //     );
                //}

                if (queryEntity.ReopenCount.HasValue)
                {
                    if (queryEntity.ReopenCount == -1)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "Complain.ReopenCount",
                              DbType.Int32,
                              "@ReopenCount",
                              QueryConditionOperatorType.MoreThan,
                              0
                              );
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                          QueryConditionRelationType.AND,
                          "Complain.ReopenCount",
                          DbType.Int32,
                          "@ReopenCount",
                          QueryConditionOperatorType.Equal,
                          queryEntity.ReopenCount
                          );
                    }

                }
                //PRE环境加此条件查询时会报错，现将查询语句更改如下，有待验证 Norton.C.Li 2012.12.06
                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "h.CreateUserSysNo",
                    DbType.Int32,
                    "@LastUpdateUserSysNo",
                    QueryConditionOperatorType.Equal,
                    queryEntity.LastUpdateUserSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.OR,
                    "Complain.OperatorSysNo",
                    DbType.Int32,
                    "@GetComplainList",
                    QueryConditionOperatorType.Equal,
                    queryEntity.LastUpdateUserSysNo);
                sqlBuilder.ConditionConstructor.EndGroupCondition();
                //sqlBuilder.ConditionConstructor.AddCondition(
                //              QueryConditionRelationType.AND,
                //              @"(SELECT TOP 1 UserSysNo FROM [OverseaCustomerManagement].[dbo].[UF_SOComplain_GetOperatorUserName] (Complain.SysNo,Complain.OperatorSysNo))",
                //              DbType.Int32,
                //              "@LastUpdateUserSysNo",
                //              QueryConditionOperatorType.Equal,
                //              queryEntity.LastUpdateUserSysNo
                //              );

                if (queryEntity.ComplainStatus.HasValue)
                {
                    if (!queryEntity.ComplainStatus.HasValue)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "Complain.Status",
                              DbType.Int32,
                              "@Status",
                              QueryConditionOperatorType.NotEqual,
                              -1
                              );
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "Complain.Status",
                            DbType.Int32,
                            "@Status",
                            QueryConditionOperatorType.Equal,
                            queryEntity.ComplainStatus
                            );
                    }
                }

                if (!string.IsNullOrEmpty(queryEntity.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "Customer.CustomerID LIKE @CustomerID");
                    cmd.AddInputParameter("@CustomerID", DbType.String, string.Format("{0}%", queryEntity.CustomerID));
                }

                if (!string.IsNullOrEmpty(queryEntity.CustomerName))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "Customer.CustomerName LIKE @CustomerName");
                    cmd.AddInputParameter("@CustomerName", DbType.String, string.Format("{0}%", queryEntity.CustomerName));
                }


                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                if (!string.IsNullOrEmpty(queryEntity.PhoneORCellphone))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "Customer.Phone LIKE @ReceivePhone");
                    cmd.AddInputParameter("@ReceivePhone", DbType.String, string.Format("{0}%", queryEntity.PhoneORCellphone));

                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "Customer.CellPhone LIKE @ReceiveCellPhone");
                    cmd.AddInputParameter("@ReceiveCellPhone", DbType.String, string.Format("{0}%", queryEntity.PhoneORCellphone));
                }
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                if (!string.IsNullOrEmpty(queryEntity.Address))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "Customer.DwellAddress LIKE @ReceiveAddress");
                    cmd.AddInputParameter("@ReceiveAddress", DbType.String, string.Format("{0}%", queryEntity.Address));
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                                        QueryConditionRelationType.AND,
                                        "Complain.CreateDate",
                                        DbType.DateTime,
                                        "@CreateDateFrom",
                                        QueryConditionOperatorType.MoreThanOrEqual,
                                        queryEntity.CreateDateFrom
                                   );

                sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "Complain.CreateDate",
                              DbType.DateTime,
                              "@CreateDateTo",
                              QueryConditionOperatorType.LessThan,
                              queryEntity.CreateDateTo
                         );

                sqlBuilder.ConditionConstructor.AddCondition(
                                        QueryConditionRelationType.AND,
                                        "h.CreateDate",
                                        DbType.DateTime,
                                        "@CloseDateFrom",
                                        QueryConditionOperatorType.MoreThanOrEqual,
                                        queryEntity.CloseDateFrom
                                   );
                sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              "h.CreateDate",
                              DbType.DateTime,
                              "@CloseDateTo",
                              QueryConditionOperatorType.LessThan,
                              queryEntity.CloseDateTo
                         );


                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "Complain.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    queryEntity.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(7, typeof(SOComplainStatus));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public virtual DataTable QueryCallingLog(QueryFilter.Customer.CustomerCallingQueryFilter queryEntity, out int totalCount)
        {
            PagingInfoEntity pagingEntity = SetPagingInfo(queryEntity);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCallingList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                cmd.CommandText,
                cmd,
                pagingEntity,
                "SysNo DESC"))
            {
                if (queryEntity.SystemNumber.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "SysNo",
                        DbType.Int32,
                        "@SystemNumber",
                        QueryConditionOperatorType.Equal,
                        queryEntity.SystemNumber.Value);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "CustomerID", DbType.String, "@CustomerID",
                        QueryConditionOperatorType.Equal,
                        queryEntity.CustomerID);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "CustomerName", DbType.String, "@CustomerName",
                      QueryConditionOperatorType.Like,
                      queryEntity.CustomerName);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "Email", DbType.String, "@Email",
                       QueryConditionOperatorType.Like,
                       queryEntity.Email);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Phone", DbType.String, "@Phone",
                    QueryConditionOperatorType.Equal,
                     queryEntity.PhoneORCellphone);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "Address", DbType.String, "@Address",
                    QueryConditionOperatorType.Like,
                    queryEntity.Address);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.Status", DbType.Int32, "@Status",
                        QueryConditionOperatorType.Equal,
                        queryEntity.CallingStatus);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "CallReason", DbType.Int32, "@CallReason",
                      QueryConditionOperatorType.Equal,
                      queryEntity.CallReason);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "CreateDate",
                        DbType.DateTime,
                        "@CreateDateFrom",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryEntity.CreateDateFrom);

                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "CreateDate",
                        DbType.DateTime,
                        "@CreateDateTo",
                        QueryConditionOperatorType.LessThan,
                        queryEntity.CreateDateTo);

                    sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ReopenCount",
                    DbType.Int32,
                    "@ReopenCount",
                    QueryConditionOperatorType.Equal,
                    queryEntity.ReopenCount);
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                          QueryConditionRelationType.AND,
                          @"[LastEditUserSysNo]",
                          DbType.Int32,
                          "@LastUpdateUserSysNo",
                          QueryConditionOperatorType.Equal,
                          queryEntity.LastUpdateUserSysNo
                          );
                if (queryEntity.OperaterCallingTimes.HasValue && queryEntity.CallingTimes.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                              QueryConditionRelationType.AND,
                              @"(SELECT COUNT(1)
                                     FROM OverseaCustomerManagement.[dbo].Customer_CallingLog 
                                     WHERE 
                                         Customer_CallingSysNo=a.[SysNo]
                                    )",
                              DbType.Int32,
                              "@UsedTimes",
                              TransOperationSignType(queryEntity.OperaterCallingTimes.Value),
                              queryEntity.CallingTimes
                              );
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "OrderSysNo",
                    DbType.Int32,
                   "@OrderSysNo",
                   QueryConditionOperatorType.Equal,
                  queryEntity.OrderSysNo
                   );


                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "LastEditDate",
                    DbType.DateTime,
                    "@EndDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    queryEntity.FinishDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "LastEditDate",
                    DbType.DateTime,
                    "@EndDateTo",
                    QueryConditionOperatorType.LessThan,
                    queryEntity.FinishDateTo);



                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CloseDate",
                    DbType.DateTime,
                    "@CloseDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    queryEntity.CloseDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CloseDate",
                    DbType.DateTime,
                    "@CloseDateTo",
                    QueryConditionOperatorType.LessThan,
                    queryEntity.CloseDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "LogTitle",
                    DbType.String,
                   "@LogTitle",
                   QueryConditionOperatorType.LeftLike,
                  queryEntity.LogTitle
                   );

                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                 QueryConditionOperatorType.Equal,
                 queryEntity.CompanyCode);


                if (queryEntity.OperaterCallingHours.HasValue && queryEntity.CallingHours.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "a.Status",
                        DbType.Int32,
                        "@Status",
                        QueryConditionOperatorType.Equal,
                        2);
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "UsedHours",
                        DbType.Int32,
                        "@UsedHours",
                          TransOperationSignType((OperationSignType)queryEntity.OperaterCallingHours.Value),
                        queryEntity.CallingHours.Value);
                }
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList list = new EnumColumnList();
                list.Add(17, typeof(CallsEventsStatus));
                list.Add(20, typeof(CallingReferenceType));
                DataTable dt = cmd.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        private PagingInfoEntity SetPagingInfo(CustomerCallingQueryFilter queryEntity)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryEntity.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryEntity.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryEntity.PagingInfo.PageIndex * queryEntity.PagingInfo.PageSize;
            return pagingEntity;
        }

        public virtual DataTable GetRMARegisterList(RMARegisterQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;


            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetRMARegisterList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "B.RequestSysNo",
                 DbType.Int32,
                 "@RequestSysNo ",
                 QueryConditionOperatorType.Equal,
                 filter.RequestSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(
                             QueryConditionRelationType.AND,
                             "A.ProductSysNo",
                             DbType.Int32,
                             "@ProductSysNo",
                             QueryConditionOperatorType.Equal,
                             filter.ProductSysNo
                        );
                sqlBuilder.ConditionConstructor.AddCondition(
              QueryConditionRelationType.AND,
              "A.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
              QueryConditionOperatorType.Equal,
              filter.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }




        public virtual DataTable QueryCallsEventLog(CustomerCallsEventLogFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;


            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCallsEventLog");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                 QueryConditionRelationType.AND,
                 "a.Customer_CallingSysNo",
                 DbType.Int32,
                 "@CallsEventsSysNo ",
                 QueryConditionOperatorType.Equal,
                 filter.CallsEventsSysNo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enunlist = new EnumColumnList();
                enunlist.Add(3, typeof(CallsEventsStatus));
                enunlist.Add(7, typeof(CustomerCallReason));
                CodeNamePairColumnList cpList = new CodeNamePairColumnList();
                cpList.Add(2, "Customer", "RecordOrigion");
                DataTable dt = cmd.ExecuteDataTable(enunlist, cpList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public virtual DataTable GetUpdateUser(string CompanyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetUpdateUser");
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            return cmd.ExecuteDataTable();
        }

        private QueryConditionOperatorType TransOperationSignType(OperationSignType type)
        {
            switch (type)
            {
                case OperationSignType.Equal:
                    return QueryConditionOperatorType.Equal;
                case OperationSignType.MoreThanOrEqual:
                    return QueryConditionOperatorType.MoreThanOrEqual;
                case OperationSignType.LessThanOrEqual:
                    return QueryConditionOperatorType.LessThanOrEqual;
                case OperationSignType.MoreThan:
                    return QueryConditionOperatorType.MoreThan;
                case OperationSignType.LessThan:
                    return QueryConditionOperatorType.LessThan;
                case OperationSignType.NotEqual:
                    return QueryConditionOperatorType.NotEqual;
                default:
                    return QueryConditionOperatorType.Equal;
            }
        }
    }
}