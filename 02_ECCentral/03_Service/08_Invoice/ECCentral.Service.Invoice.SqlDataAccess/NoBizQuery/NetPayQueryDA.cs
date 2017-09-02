using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(INetPayQueryDA))]
    public class NetPayQueryDA : INetPayQueryDA
    {
        #region INetPayQueryDA Members

        public DataTable Query(NetPayQueryFilter query, out int totalCount)
        {
            DataTable result = null;
            PagingInfoEntity pagingInfo = new PagingInfoEntity();
            if (query.PagingInfo != null)
            {
                MapSortField(query.PagingInfo);

                pagingInfo.MaximumRows = query.PagingInfo.PageSize;
                pagingInfo.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;
                pagingInfo.SortField = query.PagingInfo.SortBy;
            }
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryNetPayList");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "Result.SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.InputTime", DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.CreateDateFrom);
                string createDateTo = string.Empty;
                if (query.CreateDateTo.HasValue)
                {
                    createDateTo = Convert.ToDateTime(query.CreateDateTo).AddDays(1).ToString();
                }
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.InputTime", DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, createDateTo);
                if (!string.IsNullOrEmpty(query.SOSysNo))
                {
                    int[] sysno = Array.ConvertAll<string, int>(query.SOSysNo.Split('.'),
                        new Converter<string, int>((source) =>
                        {
                            if (string.IsNullOrEmpty(source))
                            {
                                return 0;
                            }
                            else
                            {
                                int ResultInt = 0;
                                System.Int32.TryParse(source, out ResultInt);
                                return ResultInt;

                            }
                        }));

                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "NetPay.SOSysNo", QueryConditionOperatorType.In,
                        string.Format(@"SELECT SOSysNo
FROM [IPP3].[dbo].[Finance_NetPay] WHERE MasterSoSysNo IN ({0})
UNION ALL
SELECT MasterSoSysNo
FROM [IPP3].[dbo].[Finance_NetPay] WHERE SOSysNo IN ({0})
UNION ALL
SELECT SoSysNo
FROM [IPP3].[dbo].[Finance_NetPay] WHERE SOSysNo IN ({0})
UNION ALL
SELECT SoSysNo FROM [IPP3].[dbo].[Finance_NetPay] WHERE MasterSoSysNo in
 (SELECT MasterSoSysNo
FROM [IPP3].[dbo].[Finance_NetPay] WHERE SOSysNo IN ({0}))", string.Join(",", sysno)));
                }

                if (!string.IsNullOrEmpty(query.StockID))
                {
                    sb.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format(@"Exists (SELECT SOSysNo FROM OverseaOrderManagement.dbo.V_OM_SO_ItemNoHistory SOItem WHERE WarehouseNumber={0}
AND SOMaster.SysNo=SOItem.SOSysNo )", query.StockID));
                }

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOCheckShipping.SOType", DbType.Int32, "@SOType", QueryConditionOperatorType.Equal, query.SOType);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.PayTypeSysNo", DbType.Int32, "@PayType", QueryConditionOperatorType.Equal, query.PayTypeCode);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.Status", DbType.Int32, "@SOStatus", QueryConditionOperatorType.Equal, query.SOStatus);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.ShipTypeSysNo", DbType.Int32, "@ShipType", QueryConditionOperatorType.Equal, query.ShipTypeCode);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NetPay.Source", DbType.Int32, "@Source", QueryConditionOperatorType.Equal, query.Source);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.DeliveryDate", DbType.DateTime, "@DeliveryDate", QueryConditionOperatorType.Equal, query.DeliveryDate);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOMaster.DeliveryTimeRange", DbType.Int32, "@DeliveryTimeRange", QueryConditionOperatorType.Equal, query.DeliveryTimeRange);
                if (query.SettlementStatus.HasValue)
                {
                    if (GroupBuyingSettlementStatus.Null == query.SettlementStatus)
                    {
                        sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOCheckShipping.SettlementStatus", DbType.String, "@SettlementStatus", QueryConditionOperatorType.IsNull, DBNull.Value);
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOCheckShipping.SOType", DbType.String, "@SOType1", QueryConditionOperatorType.Equal,7);
                        sb.ConditionConstructor.EndGroupCondition();
                    }
                    else
                    {
                        string settlementStatus = string.Empty;
                        switch (query.SettlementStatus.Value)
                        {
                            case GroupBuyingSettlementStatus.PlanFail:
                                settlementStatus = "P";
                                break;
                            case GroupBuyingSettlementStatus.Success:
                                settlementStatus = "S";
                                break;
                            case GroupBuyingSettlementStatus.Fail:
                                settlementStatus = "F";
                                break;
                            default:
                                throw new InvalidOperationException(string.Format("the type is not support of {0}", query.SettlementStatus.Value.ToString()));
                        }
                        sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOCheckShipping.SettlementStatus", DbType.String, "@SettlementStatus", QueryConditionOperatorType.Equal, settlementStatus);
                    }
                }
                //CashPay(现金支付)+PayPrice(手续费)+ShipPrice(运费)+PremiumAmt(保价费)+DiscountAmt(折扣)=OrderAmt(订单总金额)
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "(SOMaster.CashPay+SOMaster.PayPrice+SOMaster.ShipPrice+SOMaster.PremiumAmt+SOMaster.DiscountAmt)", DbType.Decimal, "@AmtFrom", QueryConditionOperatorType.MoreThanOrEqual, query.AmtFrom);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "(SOMaster.CashPay+SOMaster.PayPrice+SOMaster.ShipPrice+SOMaster.PremiumAmt+SOMaster.DiscountAmt)", DbType.Decimal, "@AmtTo", QueryConditionOperatorType.LessThanOrEqual, query.AmtTo);

                cmd.CommandText = sb.BuildQuerySql();
                cmd.CommandTimeout = 120;

                EnumColumnList enumColumns = new EnumColumnList();
                enumColumns.Add("Status", typeof(NetPayStatus));
                enumColumns.Add("Source", typeof(NetPaySource));
                enumColumns.Add("SOStatus", typeof(ECCentral.BizEntity.SO.SOStatus));
                enumColumns.Add("SettlementStatus", typeof(GroupBuyingSettlementStatus));

                result = cmd.ExecuteDataTable(enumColumns);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return result;
            }
        }

        #endregion INetPayQueryDA Members

        private void MapSortField(PagingInfo pagingInfo)
        {
            if (pagingInfo != null && !string.IsNullOrEmpty(pagingInfo.SortBy))
            {
                string sortBy = pagingInfo.SortBy.Trim();
                int trimLen = sortBy.Contains("asc") ? 4 : 5;
                string fieldName = sortBy.Substring(0, sortBy.Length - trimLen);
                string mapped = string.Empty;
                switch (fieldName)
                {
                    /* 请参考NetPay.config
                     * NetPay           -- [IPP3].[dbo].[Finance_NetPay] NetPay
                     * SOMaster         -- OverseaOrderManagement.dbo.V_OM_SO_Master
                     * SOCheckShipping  -- OverseaOrderManagement.dbo.V_OM_SO_CheckShipping
                     * ShipType         -- OverseaControlPanel.dbo.V_CP_ShipType
                     */
                    case "SysNo":
                        mapped = "Result.SysNo";
                        break;
                    case "StatusCode":
                        mapped = "Result.Status";
                        break;
                    case "SOSysNo":
                        mapped = "Result.SysNo";
                        break;
                    case "SOStatusCode":
                        mapped = "Result.Status";
                        break;
                    case "SOCashAmount"://SOCashAmount是一个计算来的字段，因此要用计算表达式才能排序
                        mapped = "Result.SOCashAmount";
                        break;
                    case "PrepayAmount":
                        mapped = "Result.PrepayAmt";
                        break;
                    case "PayAmount":
                        mapped = "Result.PayAmount";
                        break;
                    case "TenpayCoupon":
                        mapped = "Result.TenpayCoupon";
                        break;
                    case "Source":
                        mapped = "Result.Source";
                        break;
                    case "PayTypeCode":
                        mapped = "Result.PayTypeSysNo";
                        break;
                    case "ShipTypeCode":
                        mapped = "Result.SysNo";
                        break;
                    case "CreateUser":
                        mapped = "Result.InputUserSysNo";
                        break;
                    case "CreateTime":
                        mapped = "Result.InputTime";
                        break;
                    case "ApproveUser":
                        mapped = "Result.ApproveUserSysNo";
                        break;
                    case "ApproveTime":
                        mapped = "Result.ApproveTime";
                        break;
                    case "ReviewedUser":
                        mapped = "Result.ReviewedUserSysNo";
                        break;
                    case "ReviewedTime":
                        mapped = "Result.ReviewedTime";
                        break;
                }

                pagingInfo.SortBy = sortBy.Replace(fieldName, mapped);
            }
        }
    }
}