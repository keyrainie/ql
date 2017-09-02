using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.RMA;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.RMA
{
    public class RMARequestDA
    {
        public static QueryResult<RMARequestQueryResultInfo> QueryList(RMARequestQueryFilter filter)
        {
            QueryResult<RMARequestQueryResultInfo> result = new QueryResult<RMARequestQueryResultInfo>();

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRMARequestList");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "SysNo DESC"))
            {
                //订单编号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOSysNo", DbType.Int32
    , "@SOSysNo", QueryConditionOperatorType.Equal, filter.SOSysNo);

                //订单日期
                DateTime orderDateFrom, orderDateTo;
                if (!string.IsNullOrWhiteSpace(filter.OrderDateFrom) && DateTime.TryParse(filter.OrderDateFrom, out orderDateFrom))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderDate", DbType.DateTime
        , "@OrderDateFrom", QueryConditionOperatorType.MoreThanOrEqual, orderDateFrom);

                }
                if (!string.IsNullOrWhiteSpace(filter.OrderDateTo) && DateTime.TryParse(filter.OrderDateTo, out orderDateTo))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderDate", DbType.DateTime
        , "@OrderDateTo", QueryConditionOperatorType.LessThan, orderDateTo.Date.AddDays(1));
                }

                //顾客帐号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CustomerID", DbType.String
    , "@CustomerID", QueryConditionOperatorType.Like, filter.CustomerID);

                //退货单编号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RequestID", DbType.String
    , "@RequestID", QueryConditionOperatorType.Equal, filter.RequestID);

                //申请日期
                DateTime requestDateFrom, requestDateTo;
                if (!string.IsNullOrWhiteSpace(filter.RequestDateFrom) && DateTime.TryParse(filter.RequestDateFrom, out requestDateFrom))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "request.CreateTime", DbType.DateTime
        , "@RequestDateFrom", QueryConditionOperatorType.MoreThanOrEqual, requestDateFrom);

                }
                if (!string.IsNullOrWhiteSpace(filter.RequestDateTo) && DateTime.TryParse(filter.RequestDateTo, out requestDateTo))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "request.CreateTime", DbType.DateTime
        , "@RequestDateTo", QueryConditionOperatorType.LessThan, requestDateTo.Date.AddDays(1));
                }

                //退换货状态
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "request.Status", DbType.Int32
    , "@Status", QueryConditionOperatorType.Equal, filter.Status);

                //商品编号
                if (!string.IsNullOrWhiteSpace(filter.ProductSysNo))
                {
                    builder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, String.Format(@"
EXISTS (SELECT TOP 1 1 FROM dbo.RMA_Request_Item WITH(NOLOCK) 
        INNER JOIN dbo.RMA_Register WITH(NOLOCK) ON RMA_Request_Item.RegisterSysno = RMA_Register.Sysno
        INNER JOIN dbo.Product WITH(NOLOCK) ON Product.Sysno = RMA_Register.ProductSysNo
    WHERE RMA_Request_Item.RequestSysno =request.Sysno AND RMA_Register.ProductSysNo= {0})", filter.ProductSysNo));
                }

                //电话号码
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "request.Phone", DbType.String
    , "@Phone", QueryConditionOperatorType.Equal, filter.ReceiverPhone);

                //商家编号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "request.MerchantSysno", DbType.Int32
    , "@MerchantSysNo", QueryConditionOperatorType.Equal, filter.SellerSysNo);

                command.CommandText = builder.BuildQuerySql();

                StringBuilder sb = new StringBuilder();
                List<RMARequestQueryResultInfo> resultList = command.ExecuteEntityList<RMARequestQueryResultInfo>((s, t) =>
                {
                    sb.Clear();
                    if (!String.IsNullOrWhiteSpace((string)s["RequestType"]))
                    {
                        string[] types = s["RequestType"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < types.Length; i++)
                        {
                            sb.AppendFormat("{0}/", EnumHelper.GetDescription((RMARequestType)Enum.Parse(typeof(RMARequestType), types[i])));
                        }
                        t.RequestType = sb.ToString().TrimEnd('/');
                    }
                    t.Status = EnumHelper.GetDescription((RMARequestStatus)Enum.Parse(typeof(RMARequestStatus),s["Status"].ToString()));
                });
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                result.PageInfo = new PageInfo()
                {
                    PageIndex = filter.PageIndex,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    SortBy = filter.SortFields
                };
                result.ResultList = resultList;
            }

            return result;
        }

        public static RMARequestInfo LoadWithRequestSysNo(int rmaRequestSysNo, int sellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadRMARequestWithRequestSysNo");
            command.SetParameterValue("@SysNo", rmaRequestSysNo);
            command.SetParameterValue("@SellerSysNo", sellerSysNo);

            RMARequestInfo info = command.ExecuteEntity<RMARequestInfo>();
            if (info != null)
            {
                info.Registers = LoadRegisterWithRequestSysNo(rmaRequestSysNo);
            }
            return info;
        }

        public static List<RMARegisterInfo> LoadRegisterWithRequestSysNo(int rmaRequestSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadRMARegisterWithRequestSysNo");
            command.SetParameterValue("@RequestSysNo", rmaRequestSysNo);

            List<RMARegisterInfo> list = command.ExecuteEntityList<RMARegisterInfo>();
            return list;
        }

        public static string CreateServiceCode()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRMAServiceCode");
            return command.ExecuteScalar<string>();
        }


        public static void UpdateRegisterStatus(RMARegisterInfo registerInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRMARegisterStatus");
            command.SetParameterValue("@SysNo", registerInfo.SysNo);
            command.SetParameterValue("@RefundStatus", registerInfo.RefundStatus);
            command.SetParameterValue("@RevertStatus", registerInfo.RevertStatus);
            command.SetParameterValue("@Status", registerInfo.Status);

            command.ExecuteNonQuery();
        }

        public static void Update(RMARequestInfo request)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRMARequest");
            command.SetParameterValue("@SysNo", request.SysNo);
            command.SetParameterValue("@RecvTime", request.RecvTime);
            command.SetParameterValue("@RecvUserSysNo", request.RecvUserSysNo);
            command.SetParameterValue("@RecvUserName", request.RecvUserName);
            command.SetParameterValue("@Status", request.Status);
            command.SetParameterValue("@ReceiveWarehouse", request.ReceiveWarehouse);
            command.SetParameterValue("@ServiceCode", request.ServiceCode);
            command.SetParameterValue("@AuditUserSysNo", request.AuditUserSysNo);
            command.SetParameterValue("@AuditUserName", request.AuditUserName);
            command.SetParameterValue("@AuditTime", request.AuditTime);

            command.ExecuteNonQuery();
        }
    }
}
