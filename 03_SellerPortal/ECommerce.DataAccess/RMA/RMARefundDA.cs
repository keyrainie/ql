using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.Invoice;
using ECommerce.Entity.RMA;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.RMA
{
    public class RMARefundDA
    {
        public static QueryResult<RMARefundQueryResultInfo> QueryList(RMARefundQueryFilter filter)
        {
            QueryResult<RMARefundQueryResultInfo> result = new QueryResult<RMARefundQueryResultInfo>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRMARefundList");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "SysNo DESC"))
            {
                //订单编号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "so.SysNo", DbType.Int32
    , "@SOSysNo", QueryConditionOperatorType.Equal, filter.SOSysNo);

                //订单日期
                DateTime orderDateFrom, orderDateTo;
                if (!string.IsNullOrWhiteSpace(filter.CreateDateFrom) && DateTime.TryParse(filter.CreateDateFrom, out orderDateFrom))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "R.CreateTime", DbType.DateTime
        , "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, orderDateFrom);

                }
                if (!string.IsNullOrWhiteSpace(filter.CreateDateTo) && DateTime.TryParse(filter.CreateDateTo, out orderDateTo))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "R.CreateTime", DbType.DateTime
        , "@CreateDateTo", QueryConditionOperatorType.LessThan, orderDateTo.Date.AddDays(1));
                }

                //顾客帐号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CustomerID", DbType.String
    , "@CustomerID", QueryConditionOperatorType.Like, filter.CustomerID);

                //退款单号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RefundID", DbType.String
    , "@RefundID", QueryConditionOperatorType.Equal, filter.RefundID);

                //退款单状态
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "r.Status", DbType.Int32
    , "@Status", QueryConditionOperatorType.Equal, filter.Status);

                //商家编号
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sc.MerchantSysNo", DbType.Int32
    , "@MerchantSysNo", QueryConditionOperatorType.Equal, filter.SellerSysNo);

                command.CommandText = builder.BuildQuerySql();

                StringBuilder sb = new StringBuilder();
                List<RMARefundQueryResultInfo> resultList = command.ExecuteEntityList<RMARefundQueryResultInfo>((s, t) =>
                {
                    t.Status = EnumHelper.GetDescription((RMARefundStatus)Enum.Parse(typeof(RMARefundStatus), s["Status"].ToString()));
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


        public static int CreateNewRefundSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateNewRMARefundSysNo");
            object result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public static RMARefundInfo Create(RMARefundInfo refundInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRMARefund");
            command.SetParameterValue("@SysNo", refundInfo.SysNo);
            command.SetParameterValue("@RefundID", refundInfo.RefundID);
            command.SetParameterValue("@SOSysNo", refundInfo.SOSysNo);
            command.SetParameterValue("@CustomerSysNo", refundInfo.CustomerSysNo);
            command.SetParameterValue("@CreateTime", refundInfo.InDate);
            command.SetParameterValue("@CreateUserSysNo", refundInfo.InUserSysNo);
            command.SetParameterValue("@CreateUserName", refundInfo.InUserName);
            command.SetParameterValue("@OrgCashAmt", refundInfo.OrgCashAmt);
            command.SetParameterValue("@CashAmt", refundInfo.CashAmt);
            command.SetParameterValue("@OrgPointAmt", refundInfo.OrgPointAmt);
            command.SetParameterValue("@PointAmt", refundInfo.PointPay);
            command.SetParameterValue("@OrgGiftCardAmt", refundInfo.OrgGiftCardAmt);
            command.SetParameterValue("@GiftCardAmt", refundInfo.GiftCardAmt);
            command.SetParameterValue("@RefundPayType", refundInfo.RefundPayType);
            command.SetParameterValue("@Status", refundInfo.Status);
            command.SetParameterValue("@CashFlag", refundInfo.CashAmt > 0 ? 0 : 1);
            command.SetParameterValue("@CompanyCode", refundInfo.CompanyCode);
            command.SetParameterValue("@LanguageCode", refundInfo.LanguageCode);
            command.SetParameterValue("@StoreCompanyCode", refundInfo.CompanyCode);
            command.ExecuteNonQuery();

            return refundInfo;
        }


        public static RMARefundItemInfo CreateItem(RMARefundItemInfo item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRMARefundItem");
            command.SetParameterValue("@RefundSysNo", item.RefundSysNo);
            command.SetParameterValue("@RegisterSysNo", item.RegisterSysNo);
            command.SetParameterValue("@OrgPrice", item.OrgPrice);
            command.SetParameterValue("@UnitDiscount", item.UnitDiscount);
            command.SetParameterValue("@ProductValue", item.ProductValue);
            command.SetParameterValue("@OrgPoint", item.OrgPoint);
            command.SetParameterValue("@RefundPrice", item.RefundPrice);
            command.SetParameterValue("@PointType", item.PointType);
            command.SetParameterValue("@RefundCash", item.RefundCash);
            command.SetParameterValue("@RefundPoint", item.RefundPoint);
            command.SetParameterValue("@RefundPriceType", item.RefundPriceType);
            command.SetParameterValue("@RefundCost", item.RefundCost);
            command.SetParameterValue("@RefundCostWithoutTax", item.RefundCostWithoutTax);
            command.SetParameterValue("@RefundCostPoint", item.RefundCostPoint);
            command.SetParameterValue("@OrgGiftCardAmt", item.OrgGiftCardAmt);
            command.SetParameterValue("@CompanyCode", item.CompanyCode);
            command.SetParameterValue("@LanguageCode", item.LanguageCode);
            command.SetParameterValue("@StoreCompanyCode", item.CompanyCode);
            command.ExecuteNonQuery();

            return item;
        }


        public static void CreateRefundBankInfo(SOIncomeRefundInfo refundInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRMARefundBankInfo");
            command.SetParameterValue("@OrderType", refundInfo.OrderType);
            command.SetParameterValue("@OrderSysNo", refundInfo.OrderSysNo);
            command.SetParameterValue("@SOSysNo", refundInfo.SOSysNo);
            command.SetParameterValue("@BankName", refundInfo.BankName);
            command.SetParameterValue("@CardNumber", refundInfo.CardNumber);
            command.SetParameterValue("@CardOwnerName", refundInfo.CardOwnerName);
            command.SetParameterValue("@RefundPayType", refundInfo.RefundPayType);
            command.SetParameterValue("@CreateUserSysNo", refundInfo.CreateUserSysNo);
            command.SetParameterValue("@CreateUserName", refundInfo.CreateUserName);
            command.SetParameterValue("@Status", refundInfo.Status);
            command.SetParameterValue("@HaveAutoRMA", refundInfo.HaveAutoRMA == true ? 1 : 0);
            command.SetParameterValue("@RefundCashAmt", refundInfo.RefundCashAmt);
            command.SetParameterValue("@RefundPoint", refundInfo.RefundPoint);
            command.SetParameterValue("@RefundGiftCard", refundInfo.RefundGiftCard);
            command.SetParameterValue("@CompanyCode", refundInfo.CompanyCode);
            command.SetParameterValue("@LanguageCode", refundInfo.LanguageCode);
            command.SetParameterValue("@StoreCompanyCode", refundInfo.CompanyCode);
            command.ExecuteNonQuery();
        }

        public static void CreateRefundSOIncome(SOIncomeInfo soIncomeInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRefundSOIncome");
            command.SetParameterValue("@OrderType", soIncomeInfo.OrderType);
            command.SetParameterValue("@OrderSysNo", soIncomeInfo.OrderSysNo);
            command.SetParameterValue("@OrderAmt", soIncomeInfo.OrderAmt);
            command.SetParameterValue("@IncomeStyle", soIncomeInfo.OrderType);
            command.SetParameterValue("@IncomeAmt", soIncomeInfo.IncomeAmt);
            command.SetParameterValue("@PayAmount", soIncomeInfo.PayAmount);
            command.SetParameterValue("@IncomeUserSysNo", soIncomeInfo.InUserSysNo);
            command.SetParameterValue("@IncomeUserName", soIncomeInfo.InUserName);
            command.SetParameterValue("@Status", soIncomeInfo.Status);
            command.SetParameterValue("@PointPayAmt", soIncomeInfo.PointPay);
            command.SetParameterValue("@GiftCardPayAmt", soIncomeInfo.GiftCardPayAmt);
            command.SetParameterValue("@CompanyCode", soIncomeInfo.CompanyCode);
            command.SetParameterValue("@LanguageCode", soIncomeInfo.LanguageCode);
            command.SetParameterValue("@StoreCompanyCode", soIncomeInfo.CompanyCode);
            command.ExecuteNonQuery();
        }

        public static void AuditSOIncomeRefund(int rmaRefundSysNo, int orderType, int toStatus, int userSysNo, string userName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AuditSOIncomeRefund");
            command.SetParameterValue("@OrderSysNo", rmaRefundSysNo);
            command.SetParameterValue("@OrderType", orderType);
            command.SetParameterValue("@Status", toStatus);
            command.SetParameterValue("@AuditUserSysNo", userSysNo);
            command.SetParameterValue("@AuditUserName", userName);
            command.ExecuteNonQuery();
        }

        public static RMARefundInfo LoadWithRefundSysNo(int rmaRefundSysNo, int sellerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadRMARefundWithRefundSysNo");
            command.SetParameterValue("@SysNo", rmaRefundSysNo);
            command.SetParameterValue("@SellerSysNo", sellerSysNo);
            RMARefundInfo refundInfo = command.ExecuteEntity<RMARefundInfo>();
            if (refundInfo != null)
            {
                refundInfo.RefundItems = LoadItemWithRefundSysNo(rmaRefundSysNo);
            }

            return refundInfo;
        }

        public static List<RMARefundItemInfo> LoadItemWithRefundSysNo(int rmaRefundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadRMARefundItemWithRefundSysNo");
            command.SetParameterValue("@RefundSysNo", rmaRefundSysNo);

            List<RMARefundItemInfo> list = command.ExecuteEntityList<RMARefundItemInfo>();
            return list;
        }


        public static void Update(RMARefundInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRMARefund");
            command.SetParameterValue("@SysNo", info.SysNo.Value);
            command.SetParameterValue("@Status", info.Status);
            command.SetParameterValue("@AuditUserSysNo", info.AuditUserSysNo);
            command.SetParameterValue("@AuditUserName", info.AuditUserName);
            command.SetParameterValue("@AuditTime", info.AuditDate);
            command.SetParameterValue("@RefundUserSysNo", info.RefundUserSysNo);
            command.SetParameterValue("@RefundUserName", info.RefundUserName);
            command.SetParameterValue("@RefundTime", info.RefundDate);
            command.SetParameterValue("@FinanceStatus", info.SOIncomeStatus);
            command.ExecuteNonQuery();
        }

        public static void BatchUpdateRegisterRefundStatus(int refundSysNo, RMARefundStatus rmaRefundStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("BatchUpdateRMARegisterRefundStatus");
            command.SetParameterValue("@RefundSysNo", refundSysNo);
            command.SetParameterValue("@RefundStatus", rmaRefundStatus);
            command.ExecuteNonQuery();
        }

        public static void ConfirmRefundSOIncome(RMARefundInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ConfirmRefundSOIncome");
            command.SetParameterValue("@OrderSysNo", info.SysNo);
            command.SetParameterValue("@OrderType", info.OrderType);
            command.SetParameterValue("@Status", info.Status);
            command.SetParameterValue("@ConfirmUserSysNo", info.RefundUserSysNo);
            command.SetParameterValue("@ConfirmUserName", info.RefundUserName);
            command.SetParameterValue("@ConfirmTime", info.RefundDate);
            command.ExecuteNonQuery();
        }

        public static void ConfirmRefundSOIncomeNet(RMARefundInfo info, SOIncomeInfo soIncomeInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ConfirmRefundSOIncomeNet");
            command.SetParameterValue("@OrderSysNo", info.SysNo);
            command.SetParameterValue("@OrderType", info.OrderType);
            command.SetParameterValue("@Status", soIncomeInfo.Status);
            command.SetParameterValue("@ExternalKey", soIncomeInfo.ExternalKey);
            command.SetParameterValue("@ConfirmUserSysNo", info.RefundUserSysNo);
            command.SetParameterValue("@ConfirmUserName", info.RefundUserName);
            command.SetParameterValue("@ConfirmTime", info.RefundDate);
            command.ExecuteNonQuery();
        }

        public static void BatchUpdateRegisterRefundStatusAndStatus(int refundSysNo, RMARefundStatus rMARefundStatus, RMARequestStatus rMARequestStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("BatchUpdateRegisterRefundStatusAndStatus");
            command.SetParameterValue("@RefundSysNo", refundSysNo);
            command.SetParameterValue("@RefundStatus", rMARefundStatus);
            command.SetParameterValue("@Status", rMARequestStatus);
            command.ExecuteNonQuery();
        }

        public static List<int> QueryRMARequsetSysNoByRefundSysNo(int refundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("QueryRMARequsetSysNoByRefundSysNo");
            command.SetParameterValue("@RefundSysNo", refundSysNo);
            return command.ExecuteFirstColumn<int>();
        }

        public static List<RMARefundInfo> GetValidRefundListBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidRMARefundListBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<RMARefundInfo>();
        }
    }
}
