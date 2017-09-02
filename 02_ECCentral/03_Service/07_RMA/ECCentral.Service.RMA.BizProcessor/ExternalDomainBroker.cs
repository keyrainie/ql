using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.RMA.BizProcessor
{
    /// <summary>
    /// 所有依赖于外部Domain的接口调用
    /// </summary>
    internal static class ExternalDomainBroker
    {
        #region Invoice

        internal static void GetShipFee(int soSysNo, string stockSysNo, out decimal totalAmt, out decimal premiumAmt, out decimal shippingCharge, out decimal payPrice)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.GetShipFee(soSysNo, stockSysNo, out totalAmt, out premiumAmt, out shippingCharge, out payPrice);
        }

        internal static SOIncomeInfo GetValidSOIncomeInfo(int orderSysNo, SOIncomeOrderType orderType)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetValidSOIncome(orderSysNo, orderType);
        }

        internal static SOIncomeRefundInfo GetSOIncomeRefundInfo(int orderSysNo, RefundOrderType orderType)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetSOIncomeRefund(orderSysNo, orderType);
        }

        internal static List<InvoiceMasterInfo> GetSOInvoiceMaster(int soSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetSOInvoiceMaster(soSysNo);
        }

        internal static SOIncomeRefundInfo CreateSOIncomeRefundInfo(SOIncomeRefundInfo entity)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateSOIncomeRefund(entity);
        }

        internal static void UpdateSOIncomeRefundInfo(SOIncomeRefundInfo entity)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.UpdateSOIncomeRefund(entity);
        }

        internal static void AbandonSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AbandonSOIncomeRefundForRO(sysNo);
        }

        internal static void AbandonSOIncomeRefundForROBalance(int sysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AbandonSOIncomeRefundForROBalance(sysNo);
        }

        internal static void AutoAuditSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AutoAuditSOIncomeRefundForRO(sysNo);
        }

        internal static void SubmitAuditSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.SubmitAuditSOIncomeRefundForRO(sysNo);
        }

        internal static void CancelSubmitAuditSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.CancelSubmitAuditSOIncomeRefundForRO(sysNo);
        }

        internal static void AutoConfirmIncomeInfo(int soIncomeSysNo, int soSysNo, int opUserSysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AutoConfirmIncome(soIncomeSysNo, soSysNo, opUserSysNo);
        }

        internal static void AbandonSOIncome(int soIncomeSysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AbandonSOIncome(soIncomeSysNo);
        }

        internal static SOIncomeInfo CreateSOIncome(SOIncomeInfo entity)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateSOIncome(entity);
        }

        internal static bool CheckPayTypeCanCashRefund(int payType)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CheckPayTypeCanCashRefund(payType);
        }

        #endregion Invoice

        #region SO

        internal static SOBaseInfo GetSOBaseInfo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOBaseInfo(soSysNo);
        }

        internal static SOInfo GetSOInfo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(soSysNo);
        }

        internal static List<SOItemInfo> GetSOItemList(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOItemList(soSysNo);
        }

        internal static List<SOItemInfo> GetGiftBySOProductSysNo(int soSysNo, int productSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetGiftBySOProductSysNo(soSysNo, productSysNo);
        }

        internal static List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOPriceBySOSysNo(soSysNo);
        }

        internal static int? GetSOSysNoByCouponSysNo(int couponSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOSysNoByCouponSysNo(couponSysNo);
        }

        internal static DeliveryInfo GetDeliveryInfo(BizEntity.SO.DeliveryType type, int orderSysNo, DeliveryStatus status)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetDeliveryInfo(type, orderSysNo, status);
        }

        internal static void UpdateSOStatusToReject(int soSysNo)
        {
            ObjectFactory<ISOBizInteract>.Instance.UpdateSOStatusToReject(soSysNo);
        }

        internal static int NewSOSysNo()
        {
            return ObjectFactory<ISOBizInteract>.Instance.NewSOSysNo();
        }

        #endregion SO

        #region Customer

        internal static CustomerRank GetCustomerRank(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerRank(customerSysNo);
        }

        internal static int GetCustomerPointAddRequestStatus(int requestSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerPointAddRequestStatus(requestSysNo);
        }

        internal static int GetPriceprotectPoint(int soSysNo, List<int> productSysNoList)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetPriceprotectPoint(soSysNo, productSysNoList);
        }

        internal static CustomerInfo GetCustomerInfo(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerInfo(customerSysNo);
        }

        internal static CustomerBasicInfo GetCustomerBasicInfo(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerBasicInfo(customerSysNo);
        }

        internal static void AdjustPoint(AdjustPointRequest info)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPoint(info);
        }

        internal static void AdjustPrePay(CustomerPrepayLog adjustInfo)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPrePay(adjustInfo);
        }

        internal static void AbandonAdjustPointRequest(int requestSysNo)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AbandonAdjustPointRequest(requestSysNo);
        }

        internal static void CloseCustomerCalling(int rmaTrackingSysNo, string note)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.CloseCallsEvents(CallingReferenceType.RMA, rmaTrackingSysNo, note);
        }

        internal static void AdjustCustomerExperience(int customerSysNo, decimal ajustAmount, ExperienceLogType eperienceAdjustType, string memo)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustCustomerExperience(customerSysNo, ajustAmount, eperienceAdjustType, memo);
        }

        internal static decimal GetPointToMoneyRatio()
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetPointToMoneyRatio();
        }

        internal static string SendSMS(string cellphone, string message)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.SendSMS(cellphone, message);


        }

        #endregion Customer

        #region Common

        internal static int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        internal static int GetUserSysNo(string loginName, string sourceDirectoryKey, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserSysNo(loginName, sourceDirectoryKey, companyCode);
        }

        internal static string GetUserInfoBySysNo(int sysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(sysNo.ToString(), true);
        }

        internal static UserInfo GetUserInfo(int sysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(sysNo);
        }

        internal static string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }

        internal static PayType GetPayType(int payTypeSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetPayType(payTypeSysNo);
        }

        #endregion Common

        #region IM

        internal static ProductInfo GetProductInfo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
        }

        internal static List<ProductInfo> GetSimpleProductList(string productID)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetSimpleProductList(productID);
        }
        //internal static List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        //{
        //    return ObjectFactory<IIMBizInteract>.Instance.GetProductInfoListByProductSysNoList(productSysNoList);
        //}

        internal static string CreateElectronicGiftCard(int soSysNo, int customerSysNo, int quantity, decimal cashAmt, string memo, string companyCode)
        {
            return ObjectFactory<IIMBizInteract>.Instance.CreateElectronicGiftCard(soSysNo, customerSysNo, quantity, cashAmt, GiftCardType.Refund,"IPP.Service", memo, companyCode);
        }

        internal static string MandatoryVoidGiftCard(List<string> cardList, string companyCode)
        {
            return ObjectFactory<IIMBizInteract>.Instance.MandatoryVoidGiftCard(cardList, companyCode);
        }

        internal static string GiftCardRMARefund(List<GiftCard> cardList, string companyCode)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GiftCardRMARefund(cardList, companyCode);
        }

        internal static GiftCardInfo GetGiftCardInfoByReferenceSOSysNo(int soSysNo, int customerSysNo, GiftCardType internalType, CardMaterialType type)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetGiftCardInfoByReferenceSOSysNo(soSysNo, customerSysNo, internalType, type);
        }

        internal static List<GiftCardInfo> GetGiftCardInfoBySOSysNo(int soSysNo, GiftCardType internalType)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetGiftCardInfoBySOSysNo(soSysNo, internalType);
        }

        internal static List<GiftCardRedeemLog> GetGiftCardRedeemLog(int actionSysNo, ActionType actionType)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetGiftCardRedeemLog(actionSysNo, actionType);
        }

        internal static ProductManagerInfo GetPMInfoByProductSysNo(int productSysNo)
        {
            var product = ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
            return product.ProductBasicInfo.ProductManager;
        }

        #endregion IM

        #region MKT

        internal static PromotionCode_Customer_Log GetPromotionCodeLog(int soSysNo)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetPromotionCodeLog(soSysNo);
        }

        #endregion MKT

        #region PO

        internal static List<ConsignToAcctLogInfo> BatchCreateConsignToAcctLogs(List<ConsignToAcctLogInfo> consignToAcctLogInfos)
        {
            return ObjectFactory<IPOBizInteract>.Instance.BatchCreateConsignToAcctLogs(consignToAcctLogInfos);
        }

        internal static VendorInfo GetVendorFinanceInfoByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorInfoSysNo(vendorSysNo);
        }

        #endregion PO

        #region Inventory

        internal static ProductInventoryInfo GetProductInventoryInfo(int productSysNo, int stockSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }

        #endregion Inventory
    }
}