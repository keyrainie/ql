using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISOIncomeQueryDA))]
    public class SOIncomeQueryDA : ISOIncomeQueryDA
    {
        #region 常量定义

        private const string TempTable = @"DECLARE @temporder TABLE(
 OrderSysNo int
);
INSERT @temporder
SELECT OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WITH(NOLOCK) WHERE MasterSoSysNo IN ({0})
UNION ALL
SELECT MasterSoSysNo  As OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WITH(NOLOCK) WHERE OrderSysNo IN ({0})
UNION ALL
SELECT OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WITH(NOLOCK) WHERE OrderSysNo IN ({0})
UNION ALL
SELECT OrderSysNo FROM [IPP3].[dbo].[Finance_SOIncome] WITH(NOLOCK) WHERE MasterSoSysNo in
 (SELECT MasterSoSysNo As OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WITH(NOLOCK) WHERE OrderSysNo IN ({0}));";

        private const string ExTempTable = @"DECLARE @temporder TABLE(
 OrderSysNo int
);
INSERT @temporder
SELECT OrderSysNo
FROM [IPP3].[dbo].[Finance_SOIncome] WITH(NOLOCK) WHERE OrderSysNo IN ({0});";

        private const string TempWhere = @"select OrderSysNo from  @temporder";
        private const int TimeOut = 180; //设置查询超时时间为180（s）

        #endregion 常量定义

        #region ISOIncomeQueryDA Members

        public DataSet Query(SOIncomeQueryFilter query, out int totalCount)
        {
            DataSet result = null;
            totalCount = 0;

            if (!string.IsNullOrEmpty(query.ByCustomer))
            {
                if (Convert.ToInt32(query.ByCustomer) == 1)
                {
                    if (!string.IsNullOrEmpty(query.OrderID))
                    {
                        //从第一条记录开始
                        query.PagingInfo.PageIndex = 0;
                    }
                }
                else if (Convert.ToInt32(query.ByCustomer) == 2)
                {
                    result = GetIncomeAllByRela(query, out totalCount);
                    return result;
                }
            }

            switch (query.OrderType)
            {
                case null:
                    result = GetIncomeAll(query, out totalCount);
                    break;
                case SOIncomeOrderType.SO:
                    if (!query.IsCombine)
                    {
                        result = GetIncomeSO(query, out totalCount);
                    }
                    else
                    {
                        result = GetIncomeMergeSO(query, out totalCount);
                    }
                    break;
                case SOIncomeOrderType.RO:
                    result = GetIncomeRO(query, out totalCount);
                    break;
                case SOIncomeOrderType.AO:
                    result = GetIncomeAO(query, out totalCount);
                    break;
                case SOIncomeOrderType.RO_Balance:
                    result = GetIncomeROBalance(query, out totalCount);
                    break;
                case SOIncomeOrderType.OverPayment:
                    result = GetIncomeMRO(query, out totalCount);
                    break;
                case SOIncomeOrderType.Group:
                    result = GetIncomeGroupSO(query, 1, out totalCount);
                    break;
                case SOIncomeOrderType.GroupRefund:
                    result = GetIncomeGroupSO(query, 0, out totalCount);
                    break;
            }
            if (!string.IsNullOrEmpty(query.OrderID)
                && (!string.IsNullOrEmpty(query.ByCustomer) && Convert.ToInt32(query.ByCustomer) == 1)
                && (result.Tables["DataResult"] != null && result.Tables["DataResult"].Rows.Count > 0))
            {
                query.CustomerSysNo = string.Join(",", result.Tables["DataResult"].AsEnumerable().Select(r => r["CustomerSysNo"]));
                query.OrderID = "";
                result = GetIncomeAll(query, out totalCount);
            }
            return result;
        }

        #endregion ISOIncomeQueryDA Members

        #region Private Methods

        #region 查询方法

        private DataSet GetIncomeAllByRela(SOIncomeQueryFilter query, out int totalCount)
        {
            string customerSysNoList = string.Empty;
            string tempTab1 = string.Empty;
            DataSet result = null;

            CustomDataCommand dataCommand;

            switch (query.OrderType)
            {
                case SOIncomeOrderType.SO:
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAllListBySO");
                    break;
                case SOIncomeOrderType.RO:
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAllListByRO");
                    break;
                case SOIncomeOrderType.AO:
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAllListByAO");
                    break;
                case SOIncomeOrderType.RO_Balance:
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAllListByROBalance");
                    break;
                case SOIncomeOrderType.OverPayment:
                    dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAllListByOverPayment");
                    break;
                default:
                    totalCount = 0;
                    DataSet defaultDS = new DataSet();
                    defaultDS.Tables.Add("DataResult");
                    defaultDS.Tables.Add("StatisticResult");
                    return defaultDS;
            }

            string[] orderIDArray = query.OrderID.Trim().Split('.');
            string tempPara1 = "'" + string.Join("', '", orderIDArray) + "'";

            int orderID;
            string tempPara2 = string.Join(",", orderIDArray.ToList().ConvertAll(s => int.TryParse(s, out orderID) ? orderID : 0));

            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeTime",
                   DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom);

                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeTime",
                       DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom);

                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                       DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ConfirmuserSysNo",
                      DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }
                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.Status",
                      DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
                      DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);

                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql()
                    .Replace("#tempTab1#", tempTab1).Replace("#tempPara1#", tempPara1).Replace("#tempPara2#", tempPara2);

                FixSearchAllSql(dataCommand);

                ExecuteDataResult(dataCommand, out result, out totalCount);
            }

            return result;
        }

        private DataSet GetIncomeAll(SOIncomeQueryFilter query, out int totalCount)
        {
            string customerSysNoList = string.Empty;
            string tempTab1 = "";
            DataSet result = null;

            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAllList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "SysNo DESC"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);

                    if (query.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTable, orderIDList.ToListString());
                    }
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.ordersysno in ({0})", TempWhere));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);

                    //                    //增加货到付款类型查询：（Jin已去掉）
                    //                   
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeTime",
                   DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom);

                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeTime",
                       DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom);

                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                       DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ConfirmuserSysNo",
                      DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }
                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.Status",
                      DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                             "fsb.RefundPayType",
                                                             DbType.Int32,
                                                             "@RMARefundPayType",
                                                             QueryConditionOperatorType.Equal,
                                                             query.RMARefundPayType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
                      DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);

                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("#tempTab1#", tempTab1);

                FixSearchAllSql(dataCommand);

                ExecuteDataResult(dataCommand, out result, out totalCount);
            }

            return result;
        }

        private DataSet GetIncomeSO(SOIncomeQueryFilter query, out int totalCount)
        {
            MapSortField(query);

            string tempTab1 = string.Empty;
            string customerSysNoList = string.Empty;
            DataSet result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeSOList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "fs.SysNo"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);

                    if (query.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTable, orderIDList.ToListString());
                    }
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.ordersysno in ({0})", TempWhere));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);

                    //增加货到付款类型查询(Jin 已删除掉)

                }

                if (query.IsCheckShip)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "sc.shippingfee * 1.2 < sc.shipcost");
                }

                if (query.IsDifference)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "fs.IncomeAmt<>fs.OrderAmt");
                }
                if (query.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }
                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }

                if (query.PayedDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PayedDate",
                    DbType.DateTime, "@PayedDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.PayedDateFrom.Value);
                }
                if (query.PayedDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PayedDate",
                   DbType.DateTime, "@PayedDateTo", QueryConditionOperatorType.LessThanOrEqual, query.PayedDateTo.Value);
                }

                if (query.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }
                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }

                if (query.SOOutDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OutTime",
                   DbType.DateTime, "@SOOutDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.SOOutDateFrom.Value);
                }

                if (query.SOOutDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OutTime",
                   DbType.DateTime, "@SOOutDateTo", QueryConditionOperatorType.LessThanOrEqual, query.SOOutDateTo.Value);
                }

                if (query.SOOutDateFrom != null || query.SOOutDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOStatus",
                  DbType.Int32, "@OutStock", QueryConditionOperatorType.Equal, 4);//已出库
                }

                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }

                if (!string.IsNullOrEmpty(query.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.payTypeSysNo",
                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.PayTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.ShipTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ShipTypeSysNo",
                    DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.ShipTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.FreightMen))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "dl.DeliveryManUserSysNo",
                    DbType.Int32, "@FreightMen", QueryConditionOperatorType.Equal, Int32.Parse(query.FreightMen));
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
                    DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.SOOutStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.SOStatus",
                    DbType.Int32, "@OutStock", QueryConditionOperatorType.Equal, (int)query.SOOutStatus.Value);//订单状态
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                //加入输出参数来获取总量
                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("#tempTab1#", tempTab1);

                ExecuteDataResult(dataCommand, out result, out totalCount, "@TotalCount");
            }

            return result;
        }

        private DataSet GetIncomeMergeSO(SOIncomeQueryFilter query, out int totalCount)
        {
            //去掉OYSD的合单逻辑
            return GetIncomeRO(query, out totalCount);
        }

        private DataSet GetIncomeRO(SOIncomeQueryFilter query, out int totalCount)
        {
            MapSortField(query);

            DataSet result = null;
            string customerSysNoList = string.Empty;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeROList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "fs.SysNo"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.fsOrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }

                if (query.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }
                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }
                if (query.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }
                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }

                if (query.RORefundDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.RORefundDateFrom.Value);
                }

                if (query.RORefundDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, query.RORefundDateTo);
                }

                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
               DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.IsCash)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CashFlag",
               DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                }

                if (query.IsAudit)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.Status",
                          DbType.Int32, "@IsAudit", QueryConditionOperatorType.Equal, 1);
                }

                if (!string.IsNullOrEmpty(query.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.OrderSysNo",
               DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.OrderSysNo));
                }

                if (!string.IsNullOrEmpty(query.BankName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.BankName",
               DbType.AnsiStringFixedLength, "@BankName", QueryConditionOperatorType.Like, query.BankName.Trim());
                }

                if (!string.IsNullOrEmpty(query.PostAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.PostAddress",
               DbType.AnsiStringFixedLength, "@PostAddress", QueryConditionOperatorType.Like, query.PostAddress.Trim());
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                //加入输出参数来获取总量
                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                ExecuteDataResult(dataCommand, out result, out totalCount, "@TotalCount");
            }

            return result;
        }

        private DataSet GetIncomeAO(SOIncomeQueryFilter query, out int totalCount)
        {
            MapSortField(query);

            string customerSysNoList = string.Empty;
            DataSet result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeAOList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "fs.SysNo"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.OrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }

                if (query.IsCheckShip)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "sc.shippingfee * 1.2 < sc.shipcost");
                }

                if (query.IsDifference)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "fs.IncomeAmt<>fs.OrderAmt");
                }
                if (query.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }
                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }
                if (query.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }
                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }

                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }

                if (!string.IsNullOrEmpty(query.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.payTypeSysNo",
                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.PayTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.ShipTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ShipTypeSysNo",
                 DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.ShipTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
               DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",
                      DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.IsAudit)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.Status",
                          DbType.Int32, "@IsAudit", QueryConditionOperatorType.Equal, 1);
                }

                if (!string.IsNullOrEmpty(query.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.OrderSysNo",
               DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.OrderSysNo));
                }

                if (!string.IsNullOrEmpty(query.BankName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.BankName",
               DbType.AnsiStringFixedLength, "@BankName", QueryConditionOperatorType.Like, query.BankName.Trim());
                }

                if (!string.IsNullOrEmpty(query.PostAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.PostAddress",
               DbType.AnsiStringFixedLength, "@PostAddress", QueryConditionOperatorType.Like, query.PostAddress.Trim());
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                //加入输出参数来获取总量
                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                ExecuteDataResult(dataCommand, out result, out totalCount, "@TotalCount");
            }

            return result;
        }

        private DataSet GetIncomeROBalance(SOIncomeQueryFilter query, out int totalCount)
        {
            MapSortField(query);

            string customerSysNoList = string.Empty;
            DataSet result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeROBalanceList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "fs.SysNo"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.fsOrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }

                if (query.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }
                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }
                if (query.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }
                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }

                if (query.RORefundDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.RORefundDateFrom.Value);
                }

                if (query.RORefundDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, query.RORefundDateTo);
                }

                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
               DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.IsCash)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CashFlag",
               DbType.Int32, "@CashFlag", QueryConditionOperatorType.Equal, 0);
                }

                if (query.IsAudit)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.Status",
                          DbType.Int32, "@IsAudit", QueryConditionOperatorType.Equal, 1);
                }

                if (!string.IsNullOrEmpty(query.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.OrderSysNo",
               DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.OrderSysNo));
                }

                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                //加入输出参数来获取总量
                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                ExecuteDataResult(dataCommand, out result, out totalCount, "@TotalCount");
            }

            return result;
        }

        private DataSet GetIncomeMRO(SOIncomeQueryFilter query, out int totalCount)
        {
            MapSortField(query);

            string customerSysNoList = string.Empty;
            DataSet result = null;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeMROList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "fs.SysNo"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.OrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }

                if (query.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }

                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }

                if (query.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }

                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }

                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }

                if (!string.IsNullOrEmpty(query.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.payTypeSysNo",

                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.PayTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.ShipTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ShipTypeSysNo",

                 DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.ShipTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.FreightMen))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "dl.DeliveryManUserSysNo",

                DbType.Int32, "@FreightMen", QueryConditionOperatorType.Equal, Int32.Parse(query.FreightMen));
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",

               DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",

               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.IsAudit)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.Status",

                          DbType.Int32, "@IsAudit", QueryConditionOperatorType.Equal, 1);
                }

                if (!string.IsNullOrEmpty(query.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.OrderSysNo",

               DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.OrderSysNo));
                }

                if (!string.IsNullOrEmpty(query.BankName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.BankName",
               DbType.AnsiStringFixedLength, "@BankName", QueryConditionOperatorType.Like, query.BankName.Trim());
                }

                if (!string.IsNullOrEmpty(query.PostAddress))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.PostAddress",
               DbType.AnsiStringFixedLength, "@PostAddress", QueryConditionOperatorType.Like, query.PostAddress.Trim());
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                //加入输出参数来获取总量
                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                ExecuteDataResult(dataCommand, out result, out totalCount, "@TotalCount");
            }

            return result;
        }

        private DataSet GetROAdjust(SOIncomeQueryFilter query, out int totalCount)
        {
            MapSortField(query);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeRO_AdjustList");
            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);
            DataSet result = null;
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "fs.SysNo"))
            {
                string customerSysNoList = string.Empty;
                #region 条件参数添加
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);
                    sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.OrderSysNo in ({0})", orderIDList.ToListString()));
                }

                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "fs.CompanyCode",
                     DbType.AnsiStringFixedLength,
                     "@CompanyCode",
                     QueryConditionOperatorType.Equal,
                     query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }

                if (query.CreateDateFrom != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }
                if (query.CreateDateTo != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }

                if (query.ConfirmDateFrom != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }
                if (query.ConfirmDateTo != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "fs.confirmusersysno",
                      DbType.Int32,
                      "@IncomeConfirmer",
                      QueryConditionOperatorType.Equal,
                      Int32.Parse(query.IncomeConfirmer));
                }

                if (query.IncomeStatus.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                    DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus.Value);
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
                    DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",
                      DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.IsAudit)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.Status",
                          DbType.Int32, "@IsAudit", QueryConditionOperatorType.Equal, 1);
                }

                if (!string.IsNullOrEmpty(query.OrderSysNo))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.OrderSysNo",
                    DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.OrderSysNo));
                }

                if (!string.IsNullOrEmpty(query.BankName))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.BankName",
                    DbType.AnsiStringFixedLength, "@BankName", QueryConditionOperatorType.Like, query.BankName.Trim());
                }

                if (!string.IsNullOrEmpty(query.PostAddress))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fsb.PostAddress",
                    DbType.AnsiStringFixedLength, "@PostAddress", QueryConditionOperatorType.Like, query.PostAddress.Trim());
                }

                if (query.RORefundDateFrom != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.RefundTime",
                   DbType.DateTime, "@RORefundDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.RORefundDateFrom.Value);
                }

                if (query.RORefundDateTo != null)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.RefundTime",
                   DbType.DateTime, "@RORefundDateTo", QueryConditionOperatorType.LessThan, query.RORefundDateTo);
                }

                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }

                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                #endregion
                //加入输出参数来获取总量
                AddStatisticOutParameter(cmd);

                cmd.CommandText = sb.BuildQuerySql();

                ExecuteDataResult(cmd, out result, out totalCount, "@TotalCount");

            }
            return result;
        }

        private DataSet GetIncomeGroupSO(SOIncomeQueryFilter query, int statue, out int totalCount)
        {
            string tempTab1 = string.Empty;
            string customerSysNoList = string.Empty;
            DataSet result = null;

            CustomDataCommand dataCommand = null;
            if (statue == 1)
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeGroupSOList");
            else
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleIncomeGroupRefundSOList");

            PagingInfoEntity pagingInfo = CreatePagingInfo(query.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "fs.SysNo"))
            {
                if (!string.IsNullOrEmpty(query.CustomerSysNo))
                {
                    customerSysNoList = query.CustomerSysNo.Replace(".", ",");
                    customerSysNoList = customerSysNoList.TrimEnd(new char[] { ',' });
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("gbt.customersysno in ({0})", customerSysNoList));
                }

                if (!string.IsNullOrEmpty(query.OrderID))
                {
                    List<int> orderIDList = GetOrderIDListInt(query.OrderID);

                    if (query.IsRelated)
                    {
                        tempTab1 = string.Format(TempTable, orderIDList.ToListString());
                    }
                    else
                    {
                        tempTab1 = string.Format(ExTempTable, orderIDList.ToListString());
                    }
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("fs.ordersysno in ({0})", TempWhere));
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (query.IncomeType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incomestyle",
                    DbType.Int32, "@IncomeType", QueryConditionOperatorType.Equal, query.IncomeType.Value);
                }

                if (query.IsCheckShip)
                {
                }

                if (query.IsDifference)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "fs.IncomeAmt<>fs.OrderAmt");
                }
                if (query.CreateDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                    DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom.Value);
                }
                if (query.CreateDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.incometime",
                   DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, query.CreateDateTo);
                }

                if (query.PayedDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PayedDate",
                    DbType.DateTime, "@PayedDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.PayedDateFrom.Value);
                }
                if (query.PayedDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PayedDate",
                   DbType.DateTime, "@PayedDateTo", QueryConditionOperatorType.LessThanOrEqual, query.PayedDateTo.Value);
                }

                if (query.ConfirmDateFrom != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ConfirmDateFrom.Value);
                }
                if (query.ConfirmDateTo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmtime",
                   DbType.DateTime, "@ConfirmDateTo", QueryConditionOperatorType.LessThan, query.ConfirmDateTo);
                }

                if (!string.IsNullOrEmpty(query.IncomeConfirmer))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.confirmusersysno",
                  DbType.Int32, "@IncomeConfirmer", QueryConditionOperatorType.Equal, Int32.Parse(query.IncomeConfirmer));
                }

                if (query.SOOutDateFrom != null)
                {
                }

                if (query.SOOutDateTo != null)
                {
                }

                if (query.SOOutDateFrom != null || query.SOOutDateTo != null)
                {
                }

                if (query.IncomeStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.status",
                 DbType.Int32, "@IncomeStatus", QueryConditionOperatorType.Equal, query.IncomeStatus);
                }

                if (!string.IsNullOrEmpty(query.PayTypeSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "gbt.PayType",
                 DbType.Int32, "@payTypeSysNo", QueryConditionOperatorType.Equal, Int32.Parse(query.PayTypeSysNo));
                }

                if (!string.IsNullOrEmpty(query.ShipTypeSysNo))
                {
                }

                if (!string.IsNullOrEmpty(query.FreightMen))
                {
                }

                if (!string.IsNullOrEmpty(query.ReferenceID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.ReferenceID",
                    DbType.AnsiString, "@ReferenceID", QueryConditionOperatorType.Like, query.ReferenceID);
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType);
                }

                if (query.SOOutStatus.HasValue)
                {
                }
                if (query.IncomeAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.IncomeAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.IncomeAmt",
                     DbType.AnsiString, "@IncomeAmt", oper, query.IncomeAmt);
                }
                if (query.PrepayAmt.HasValue)
                {
                    QueryConditionOperatorType oper = QueryConditionOperatorType.Equal;
                    switch (query.PrepayAmtOper)
                    {
                        case OperationSignType.LessThanOrEqual:
                            oper = QueryConditionOperatorType.LessThanOrEqual;
                            break;
                        case OperationSignType.Equal:
                            oper = QueryConditionOperatorType.Equal;
                            break;
                        case OperationSignType.MoreThanOrEqual:
                            oper = QueryConditionOperatorType.MoreThanOrEqual;
                            break;
                        default:
                            break;
                    }
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "fs.PrepayAmt",
                     DbType.AnsiString, "@PrepayAmt", oper, query.PrepayAmt);
                }

                //加入输出参数来获取总量
                AddStatisticOutParameter(dataCommand);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql().Replace("#tempTab1#", tempTab1);

                ExecuteDataResult(dataCommand, out result, out totalCount, "@TotalCount");
            }

            return result;
        }
        #endregion 查询方法

        #region 辅助方法

        private static void FixSearchAllSql(CustomDataCommand dataCommand)
        {
            int index = dataCommand.CommandText.IndexOf("fsb.ordertype = 4");
            if (index != -1)
            {
                string begin = dataCommand.CommandText.Substring(0, index);
                string end = dataCommand.CommandText.Substring(index);
                end = end.Replace("fs.OrderSysNo in", "fs.fsOrderSysNo in");
                dataCommand.CommandText = begin + end;
            }
        }

        private List<int> GetOrderIDListInt(string orderid)
        {
            if (!string.IsNullOrEmpty(orderid))
            {
                orderid = orderid.ToUpper().Replace(" ", "").Replace(".", ",");
                List<int> orderIDList = new List<int>();
                string[] temp = orderid.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    int index = temp[i].IndexOf("R3");
                    if (index != -1)
                    {
                        temp[i] = temp[i].Substring(2).TrimStart('0');
                    }
                }
                int[] result;
                result = Array.ConvertAll(temp, str =>
                {
                    int p = 0;
                    int.TryParse(str, out p);
                    return p;
                });
                orderIDList.AddRange(result);
                return orderIDList;
            }
            else
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// 构造PagingInfo对象
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private PagingInfoEntity CreatePagingInfo(ECCentral.QueryFilter.Common.PagingInfo pageInfo)
        {
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (pageInfo != null)
            {
                pagingInfo.MaximumRows = pageInfo.PageSize;
                pagingInfo.StartRowIndex = pageInfo.PageIndex * pageInfo.PageSize;
                pagingInfo.SortField = pageInfo.SortBy;
            }
            return pagingInfo;
        }

        /// <summary>
        /// 添加输入参数来获得统计信息
        /// <param name="dataCommand"></param>
        private void AddStatisticOutParameter(CustomDataCommand dataCommand)
        {
            dataCommand.AddOutParameter("@TotalOrderAmt", DbType.String, 50);
            dataCommand.AddOutParameter("@TotalIncomeAmt", DbType.String, 50);
            dataCommand.AddOutParameter("@TotalAlreadyIncomeAmt", DbType.String, 50);
            dataCommand.AddOutParameter("@TotalPrepayAmt", DbType.String, 50);
            dataCommand.AddOutParameter("@TotalShipPrice", DbType.String, 50);
            dataCommand.AddOutParameter("@PageTotalCount", DbType.String, 50);

            //下面的输出参数专用于收款单自动确认的收款单查询
            dataCommand.AddOutParameter("@TotalReturnCash", DbType.String, 50);
            dataCommand.AddOutParameter("@TotalReturnPoint", DbType.String, 50);
            dataCommand.AddOutParameter("@TotalToleranceAmt", DbType.String, 50);
        }

        /// <summary>
        /// 映射排序字段
        /// </summary>
        /// <param name="query"></param>
        private static void MapSortField(SOIncomeQueryFilter query)
        {
            if (query.PagingInfo != null && !string.IsNullOrEmpty(query.PagingInfo.SortBy))
            {
                var index = 0;
                index = query.PagingInfo.SortBy.Contains("asc") ? 4 : 5;
                var sort = query.PagingInfo.SortBy.Substring(0, query.PagingInfo.SortBy.Length - index);
                var sortField = query.PagingInfo.SortBy;
                switch (sort)
                {
                    case "SysNo":
                        query.PagingInfo.SortBy = sortField.Replace("SysNo", "fs.SysNo");
                        break;
                    case "OrderSysNo":
                        query.PagingInfo.SortBy = sortField.Replace("OrderSysNo", "fs.OrderSysNo");
                        break;
                    case "CustomerSysNo":
                        query.PagingInfo.SortBy = sortField.Replace("CustomerSysNo", "fs.CustomerSysNo");
                        break;
                    case "OrderType":
                        query.PagingInfo.SortBy = sortField.Replace("OrderType", "fs.OrderType");
                        break;
                    case "OrderID":
                        if (query.OrderType == SOIncomeOrderType.SO
                            || query.OrderType == SOIncomeOrderType.AO
                            || query.OrderType == SOIncomeOrderType.OverPayment)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("OrderID", "fs.orderid");
                        }
                        else if (query.OrderType == SOIncomeOrderType.RO
                            || query.OrderType == SOIncomeOrderType.RO_Balance)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("OrderID", "fs.orderid");
                        }

                        break;
                    case "IncomeStyle":
                        query.PagingInfo.SortBy = sortField.Replace("IncomeStyle", "fs.IncomeStyle");
                        break;
                    case "OrderAmt":
                        query.PagingInfo.SortBy = sortField.Replace("OrderAmt", "fs.OrderAmt");
                        break;
                    case "PrepayAmt":
                        query.PagingInfo.SortBy = sortField.Replace("PrepayAmt", "fs.PrepayAmt");
                        break;
                    case "GiftCardPayAmt":
                        query.PagingInfo.SortBy = sortField.Replace("GiftCardPayAmt", "fs.GiftCardPayAmt");
                        break;
                    case "RefundGiftCard":
                        query.PagingInfo.SortBy = sortField.Replace("RefundGiftCard", "fsb.RefundGiftCard");
                        break;
                    case "IncomeAmt":
                        query.PagingInfo.SortBy = sortField.Replace("IncomeAmt", "fs.IncomeAmt");
                        break;
                    case "ShipPrice":
                        if (query.OrderType == SOIncomeOrderType.SO)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("ShipPrice", "fs.ShipPrice");
                        }
                        else
                        {
                            query.PagingInfo.SortBy = string.Empty;
                        }
                        break;
                    case "ShippingFee":
                        if (query.OrderType == SOIncomeOrderType.SO)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("ShippingFee", "sc.ShippingFee");
                        }
                        else
                        {
                            query.PagingInfo.SortBy = string.Empty;
                        }
                        break;
                    case "PackageFee":
                        if (query.OrderType == SOIncomeOrderType.SO)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("PackageFee", "sc.ShippingFee");
                        }
                        else
                        {
                            query.PagingInfo.SortBy = string.Empty;
                        }
                        break;
                    case "RegisteredFee":
                        if (query.OrderType == SOIncomeOrderType.SO)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("RegisteredFee", "sc.RegisteredFee");
                        }
                        else
                        {
                            query.PagingInfo.SortBy = string.Empty;
                        }
                        break;
                    case "ShipCost":
                        if (query.OrderType == SOIncomeOrderType.SO)
                        {
                            query.PagingInfo.SortBy = sortField.Replace("ShipCost", "sc.ShipCost");
                        }
                        else
                        {
                            query.PagingInfo.SortBy = string.Empty;
                        }
                        break;
                    case "IncomeUser":
                        query.PagingInfo.SortBy = sortField.Replace("IncomeUser", "suiu.DisplayName");
                        break;
                    case "IncomeTime":
                        query.PagingInfo.SortBy = sortField.Replace("IncomeTime", "fs.IncomeTime");
                        break;
                    case "ConfirmUser":
                        query.PagingInfo.SortBy = sortField.Replace("ConfirmUser", "sucu.DisplayName");
                        break;
                    case "ConfirmTime":
                        query.PagingInfo.SortBy = sortField.Replace("ConfirmTime", "fs.ConfirmTime");
                        break;
                    case "IncomeStatus":
                        query.PagingInfo.SortBy = sortField.Replace("IncomeStatus", "fs.Status");
                        break;
                    case "ReferenceID":
                        query.PagingInfo.SortBy = sortField.Replace("ReferenceID", "fs.ReferenceID");
                        break;
                    case "RMARefundPayType":
                        if (query.OrderType == SOIncomeOrderType.RO_Balance)
                        {
                            query.PagingInfo.SortBy = string.Empty;
                        }
                        else
                        {
                            query.PagingInfo.SortBy = sortField.Replace("RMARefundPayType", "fsb.refundpaytype");
                        }
                        break;
                    case "RelatedSoSysNo":
                        query.PagingInfo.SortBy = sortField.Replace("RelatedSoSysNo", "netPay.RelatedSoSysNo");
                        break;
                }
            }
        }

        /// <summary>
        /// 执行查询，返回查询结果DataSet和记录总数
        /// </summary>
        /// <param name="dataCommand">DataCommand</param>
        /// <param name="resultDS">用于返回查询结果的DataSet对象</param>
        /// <param name="totalCount">查询记录总条数</param>
        /// <param name="totalCountPramName">获取记录总数SQL输出参数名称</param>
        private void ExecuteDataResult(CustomDataCommand dataCommand, out DataSet resultDS, out int totalCount, string totalCountPramName)
        {
            //设置超时时间
            dataCommand.CommandTimeout = TimeOut;

            EnumColumnList enumColumns = new EnumColumnList();
            enumColumns.Add("OrderType", typeof(SOIncomeOrderType));
            enumColumns.Add("IncomeStyle", typeof(SOIncomeOrderStyle));
            enumColumns.Add("IncomeStatus", typeof(SOIncomeStatus));
            enumColumns.Add("RMARefundPayType", typeof(RefundPayType));
            enumColumns.Add("SapImportedStatus", typeof(SapImportedStatus));
            DataTable resultTable = dataCommand.ExecuteDataTable(enumColumns);
            resultTable.TableName = "DataResult";

            DataTable statisticTable = CreateStatisticResultTable(dataCommand, resultTable);
            statisticTable.TableName = "StatisticResult";

            if (!totalCountPramName.StartsWith("@"))
            {
                totalCountPramName = "@" + totalCountPramName;
            }
            totalCount = Convert.ToInt32(dataCommand.GetParameterValue(totalCountPramName));

            resultDS = new DataSet();
            resultDS.Tables.Add(resultTable);
            resultDS.Tables.Add(statisticTable);
        }

        /// <summary>
        /// 执行查询，返回查询结果DataSet和记录总数，默认取得记录总数的SQL输出参数名称为@PageTotalCount
        /// </summary>
        /// <param name="dataCommand"></param>
        /// <param name="resultDS"></param>
        /// <param name="totalCount"></param>
        private void ExecuteDataResult(CustomDataCommand dataCommand, out DataSet resultDS, out int totalCount)
        {
            ExecuteDataResult(dataCommand, out resultDS, out totalCount, "@PageTotalCount");
        }

        /// <summary>
        /// 构造统计结果表格
        /// </summary>
        private DataTable CreateStatisticResultTable(DataCommand dataCommand, DataTable dataResult)
        {
            DataTable table = new DataTable("StatisticResultTable");

            table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("StatisticType",typeof(StatisticType)),
                new DataColumn("OrderAmt",typeof(decimal)),
                new DataColumn("IncomeAmt",typeof(decimal)),
                new DataColumn("AlreadyIncomeAmt",typeof(decimal)),
                new DataColumn("PrepayAmt",typeof(decimal)),
                new DataColumn("ShipPrice",typeof(decimal)),
                //下面的输出参数专用于收款单自动确认的收款单查询
                new DataColumn("ReturnCash",typeof(decimal)),
                new DataColumn("ReturnPoint",typeof(int)),
                new DataColumn("ToleranceAmt",typeof(decimal))
            });

            #region 总计

            StatisticResult total = new StatisticResult()
            {
                OrderAmt = Convert.ToDecimal(!(dataCommand.GetParameterValue("TotalOrderAmt") is DBNull) ? dataCommand.GetParameterValue("TotalOrderAmt") : 0M),
                IncomeAmt = Convert.ToDecimal(!(dataCommand.GetParameterValue("TotalIncomeAmt") is DBNull) ? dataCommand.GetParameterValue("TotalIncomeAmt") : 0M),
                AlreadyIncomeAmt = Convert.ToDecimal(!(dataCommand.GetParameterValue("TotalAlreadyIncomeAmt") is DBNull) ? dataCommand.GetParameterValue("TotalAlreadyIncomeAmt") : 0M),
                PrepayAmt = Convert.ToDecimal(!(dataCommand.GetParameterValue("TotalPrepayAmt") is DBNull) ? dataCommand.GetParameterValue("TotalPrepayAmt") : 0M),
                ShipPrice = Convert.ToDecimal(!(dataCommand.GetParameterValue("TotalShipPrice") is DBNull) ? dataCommand.GetParameterValue("TotalShipPrice") : 0M),
                //下面的输出参数专用于收款单自动确认的收款单查询
                ReturnCash = Convert.ToDecimal(!(dataCommand.GetParameterValue("ReturnCash") is DBNull) ? dataCommand.GetParameterValue("ReturnCash") : 0M),
                ToleranceAmt = Convert.ToDecimal(!(dataCommand.GetParameterValue("ToleranceAmt") is DBNull) ? dataCommand.GetParameterValue("ToleranceAmt") : 0M),
                ReturnPoint = Convert.ToInt32(!(dataCommand.GetParameterValue("ReturnPoint") is DBNull) ? dataCommand.GetParameterValue("ReturnPoint") : 0)
            };
            table.Rows.Add(CreateStatisticResultRow(table, total, StatisticType.Total));

            #endregion 总计

            #region 本页小计

            StatisticResult subtotal = new StatisticResult()
            {
                OrderAmt = dataResult.Select().Sum(row => Convert.ToDecimal(!row.IsNull("OrderAmt") ? row["OrderAmt"] : 0M)),
                IncomeAmt = dataResult.Select().Sum(row => Convert.ToDecimal(!row.IsNull("IncomeAmt") ? row["IncomeAmt"] : 0M)),
                AlreadyIncomeAmt = dataResult.Select("IncomeStatus=1").Sum(row => Convert.ToDecimal(!row.IsNull("IncomeAmt") ? row["IncomeAmt"] : 0M)),
                PrepayAmt = dataResult.Select().Sum(row => Convert.ToDecimal(!row.IsNull("PrepayAmt") ? row["PrepayAmt"] : 0M)),
                ShipPrice = dataResult.Select().Sum(row => Convert.ToDecimal(!row.IsNull("ShipPrice") ? row["ShipPrice"] : 0M))
            };
            table.Rows.Add(CreateStatisticResultRow(table, subtotal, StatisticType.Page));

            #endregion 本页小计

            return table;
        }

        /// <summary>
        /// 构造统计行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="data"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        private DataRow CreateStatisticResultRow(DataTable table, StatisticResult data, StatisticType tp)
        {
            DataRow row = table.NewRow();
            row["StatisticType"] = tp;
            row["OrderAmt"] = data.OrderAmt;
            row["IncomeAmt"] = data.IncomeAmt;
            row["AlreadyIncomeAmt"] = data.AlreadyIncomeAmt;
            row["PrepayAmt"] = data.PrepayAmt;
            row["ShipPrice"] = data.ShipPrice;

            return row;
        }

        /// <summary>
        /// 统计结果
        /// </summary>
        private struct StatisticResult
        {
            public decimal OrderAmt;
            public decimal IncomeAmt;
            public decimal AlreadyIncomeAmt;
            public decimal PrepayAmt;
            public decimal ShipPrice;
            public decimal? ReturnCash;
            public decimal? ToleranceAmt;
            public int? ReturnPoint;
        }

        #endregion 辅助方法

        #endregion Private Methods

        #region QueryROExport

        public DataSet QueryROExport(SOIncomeQueryFilter filter)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetROExport");
            dataCommand.SetParameterValue("@RefundTimeFrom", filter.RORefundDateFrom.HasValue ? filter.RORefundDateFrom.Value : DateTime.Parse("1/1/1753 12:00:00 AM"));
            dataCommand.SetParameterValue("@RefundTimeTo", filter.RORefundDateTo.HasValue ? filter.RORefundDateTo.Value : DateTime.MaxValue);
            dataCommand.SetParameterValue("@CompanyCode", filter.CompanyCode);

            return dataCommand.ExecuteDataSet();
        }

        #endregion QueryROExport

        #region QuerySO

        public DataSet QuerySO(SOQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingInfo = CreatePagingInfo(filter.PagingInfo);
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOMaster");
            DataSet result = null;
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "sm.SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.SOSysNo))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.SOSysNo);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("sm.SysNo in ({0})", orderIDList.ToListString()));
                }
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("SOStatus", typeof(SOStatus));
                enumColumns.Add("PayStatus", typeof(SOIncomeStatus));

                dataCommand.AddOutParameter("@TotalAmount", DbType.String, 50);
                DataTable resultDT = dataCommand.ExecuteDataTable(enumColumns);
                resultDT.TableName = "DataResult";

                DataTable statisticDT = new DataTable();
                statisticDT.TableName = "StatisticResult";
                statisticDT.Columns.Add("TotalAmount", typeof(decimal));
                statisticDT.Rows.Add(Convert.ToDecimal(!(dataCommand.GetParameterValue("@TotalAmount") is DBNull) ? dataCommand.GetParameterValue("@TotalAmount") : 0M));

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                result = new DataSet();
                result.Tables.AddRange(new DataTable[] { resultDT, statisticDT });
            }
            return result;
        }

        public DataSet ExportQuerySO(SOQueryFilter filter)
        {

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOMasterForExport");


            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "sm.SysNo"))
            {
                if (!string.IsNullOrEmpty(filter.SOSysNo))
                {
                    List<int> orderIDList = GetOrderIDListInt(filter.SOSysNo);
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("sm.SysNo in ({0})", orderIDList.ToListString()));
                }
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                return dataCommand.ExecuteDataSet();
            }

        }


        #endregion QuerySO

        public DataTable QuerySaleReceivables(SaleReceivablesQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySaleReceivables");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo ASC"))
            {
                cmd.SetParameterValue("@QueryDate", filter.QueryDate);
                cmd.SetParameterValue("@CurreySysNo", filter.Currency);
                cmd.SetParameterValue("@PaySysNo", filter.PayTypeSysNo);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable<SaleCurrency>("Currency");
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 运费报表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QuerySOFreightStatDetai(SOFreightStatDetailQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySOFreightStatDetai");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RealFreightConfirm", DbType.Int32, "@RealFreightConfirm", QueryConditionOperatorType.Equal, filter.RealFreightConfirm);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOFreightConfirm", DbType.Int32, "@SOFreightConfirm", QueryConditionOperatorType.Equal, filter.SOFreightConfirm);

                if (!string.IsNullOrEmpty(filter.SOSysNo))
                {
                    int sysNo;
                    var soSysNoList = filter.SOSysNo.Split('.').ToList();
                    soSysNoList.RemoveAll(s => !int.TryParse(s, out sysNo));
                    if (soSysNoList.Count > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("SoSysNo IN({0})",string.Join(",", soSysNoList)));
                    }
                }

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("RealFreightConfirm", typeof(RealFreightStatus));
                enumColumnList.Add("SOFreightConfirm", typeof(CheckStatus));

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }
    }
}