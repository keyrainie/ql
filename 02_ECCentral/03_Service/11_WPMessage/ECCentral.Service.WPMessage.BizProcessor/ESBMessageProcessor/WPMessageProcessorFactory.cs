using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.Customer;
using ECCentral.Service.EventMessage.SO;
using ECCentral.Service.Utility;

namespace ECCentral.Service.WPMessage.BizProcessor
{

    public class WPMessageProcessorFactory
    {
        public static List<IESBMessageProcessor> CreateProcessorList(ESBMessage msg)
        {
            string subject = msg.Subject;
            if (String.IsNullOrWhiteSpace(subject))
            {
                return new List<IESBMessageProcessor>(0);
            }

            // subject 转为大写进行比较
            subject = subject.Trim().ToUpper();
            List<IESBMessageProcessor> processorList = new List<IESBMessageProcessor>(2);
            //Subject 转为大写后进行比较
            switch (subject)
            {
                #region SO
                //case "ECC_SO_FPCHECKED":
                //    processorList.Add(new SOFPChecked_CreateConfirmTask());
                //    break;
                //case "ECC_SO_CONFIRMED":
                //    processorList.Add(new SOConfirmed_CompleteConfirmTask());
                //    break;
                //case "ECC_SO_SPLIT":
                //    processorList.Add(new SOSplit_CreateAuditTask());
                //    break;
                case "ECC_SO_AUDITED": // 订单审核Processor;
                    processorList.Add(new SOAuditedMessageProcessor());
                    break;
                case "ECC_SO_COMPLETED":
                    break;
                case "ECC_SO_OUTSTOCKED":
                    processorList.Add(new SOOutStock_CompleteOutStockTask());
                    break;
                //case "ECC_SOCUSTOMERABANDONREQUEST_CREATED":
                //    processorList.Add(new SOCreatedCustomerAbandonRequest_CreateAbandonRequestTask());
                //    break;
                //case "ECC_SOCUSTOMERABANDONREQUEST_CANCELED":
                //    processorList.Add(new SOCanceledCustomerAbandonRequest_CompleteAbandonRequestTask());
                //    break;
                //case "ECC_SOCUSTOMERABANDONREQUEST_COMPLETED":
                //    processorList.Add(new SOCompletedCustomerAbandonRequest_CompleteAbandonRequestTask());
                //    break;
                case "ECC_SO_VOIDED":
                    processorList.Add(new SOAbandonedMessageProcessor());
                    break;


                #endregion

                #region MKT
                case "ECC_COUPON_SUBMITED":
                    processorList.Add(new CouponSubmitMessageCreator());
                    break;
                case "ECC_COUPON_AUDITED":
                    processorList.Add(new CouponAuditMessageCompleter());
                    break;
                case "ECC_COUPON_AUDITCANCELED":
                    processorList.Add(new CouponAuditCancelMessageCompleter());
                    break;
                case "ECC_COUPON_AUDITREJECTED":
                    processorList.Add(new CouponAuditRejectMessageCompleter());
                    break;
                case "ECC_COUPON_VOIDED":
                    processorList.Add(new CouponVoidMessageCompleter());
                    break;

                case "ECC_SALEGIFT_SUBMITED":
                    processorList.Add(new SaleGiftSubmitMessageCreator());
                    break;
                case "ECC_SALEGIFT_AUDITED":
                    processorList.Add(new SaleGiftAuditMessageCompleter());
                    break;
                case "ECC_SALEGIFT_AUDITCANCELED":
                    processorList.Add(new SaleGiftAuditCancelMessageCompleter());
                    break;
                case "ECC_SALEGIFT_AUDITREJECTED":
                    processorList.Add(new SaleGiftAuditRejectMessageCompleter());
                    break;
                case "ECC_SALEGIFT_VOIDED":
                    processorList.Add(new SaleGiftVoidMessageCompleter());
                    break;

                case "ECC_GROUPBUY_CREATED":
                    processorList.Add(new GroupBuySaveMessageCreator());
                    break;
                case "ECC_GROUPBUY_UPDATED":
                    processorList.Add(new GroupBuyUpdateMessageCreator());
                    break;
                case "ECC_GROUPBUY_AUDITED":
                    processorList.Add(new GroupBuyAuditMessageCompleter());
                    break;
                case "ECC_GROUPBUY_VOIDED":
                    processorList.Add(new GroupBuyVoidMessageCompleter());
                    break;

                case "ECC_COMBOSALE_SUBMITED":
                    processorList.Add(new ComboSaleSubmitMessageCreator());
                    break;
                case "ECC_COMBOSALE_AUDITED":
                    processorList.Add(new ComboSaleAuditMessageCompleter());
                    break;
                case "ECC_COMBOSALE_AUDITREJECTED":
                    processorList.Add(new ComboSaleAuditRefuseMessageCompleter());
                    break;
                case "ECC_COMBOSALE_ACTIVED":
                    processorList.Add(new ComboSaleActiveMessageCompleter());
                    break;

                case "ECC_COUNTDOWN_SUBMITED":
                    processorList.Add(new CountDownSubmitMessageCreator());
                    break;
                case "ECC_COUNTDOWN_AUDITED":
                    processorList.Add(new CountDownAuditMessageCompleter());
                    break;
                case "ECC_COUNTDOWN_AUDITREJECTED":
                    processorList.Add(new CountDownAuditRejectMessageCompleter());
                    break;

                case "ECC_PRODUCTREVIEW_CREATED":
                    processorList.Add(new ProductReviewCreatedMessageCreator());
                    break;

                case "ECC_PRODUCTREVIEW_AUDITED":
                    processorList.Add(new ProductReviewAuditMessageCompleter());
                    break;
                case "ECC_PRODUCTREVIEW_VOIDED":
                    processorList.Add(new ProductReviewVoidMessageCompleter());
                    break;

                case "ECC_PRODUCTREVIEWREPLY_CREATED":
                    processorList.Add(new ProductReviewReplyCreatedMessageCreator());
                    break;
                case "ECC_PRODUCTREVIEWREPLY_AUDITED":
                    processorList.Add(new ProductReviewReplyAuditMessageCompleter());
                    break;
                case "ECC_PRODUCTREVIEWREPLY_VOIDED":
                    processorList.Add(new ProductReviewReplyVoidMessageCompleter());
                    break;






                case "ECC_PRODUCTCONSULT_CREATED":
                    processorList.Add(new ConsultCreateMessageCompleter());
                    break;

                case "ECC_PRODUCTCONSULTREPLY_CREATED":
                    processorList.Add(new ConsultReplyCreateMessageCompleter());
                    break;



                case "ECC_CONSULT_AUDITED":
                    processorList.Add(new ConsultAuditMessageCompleter());
                    break;
                case "ECC_CONSULT_VOIDED":
                    processorList.Add(new ConsultVoidMessageCompleter());
                    break;
                case "ECC_CONSULTREPLY_AUDITREJECTED":
                    processorList.Add(new ConsultReplyAuditRefuseMessageCompleter());
                    break;
                case "ECC_CONSULTREPLY_AUDITED":
                    processorList.Add(new ConsultReplyAuditMessageCompleter());
                    break;
                case "ECC_CONSULTREPLY_VOIDED":
                    processorList.Add(new ConsultReplyVoidMessageCompleter());
                    break;

                #endregion

                #region Invoice
                case "ECC_NETPAY_CREATED":
                    processorList.Add(new CreateNetpay_CreateAuditTask());
                    break;
                case "ECC_NETPAY_AUDITED":
                    processorList.Add(new NetpayAudited_CompleteAuditTask());
                    processorList.Add(new NetpayAudited_CreateSOAuditTask());
                    processorList.Add(new NetpayAudited_CreateAssignCustomerConsultTask());
                    break;
                case "ECC_NETPAY_VOIDED":
                    processorList.Add(new NetpayAbandoned_CompleteAuditTask());
                    break;
                #region 退款审核
                case "ECC_REFUNDAUDIT_ACCOUNTANTAUDITCREATED":
                    processorList.Add(new CreateRefundAccountantAudit_CreateTask());
                    break;
                case "ECC_REFUNDAUDIT_CASHIERREFUNDCREATED":
                    processorList.Add(new CreateRefundCashierRefund_CreateTask());
                    break;
                case "ECC_REFUND_ACCOUNTANTAUDITED":
                    processorList.Add(new RefundAuditAccountantPassed_CompleteAuditTask());
                    processorList.Add(new RefundAuditAccountantPassed_CreateAuditTask());
                    break;
                case "ECC_REFUND_ACCOUNTANTAUDITREJECTED":
                    processorList.Add(new RefundAuditAccountantRejected_CompleteAuditTask());
                    break;
                case "ECC_REFUND_CASHIERREFUNDED":
                    processorList.Add(new RefundAuditCashierRefunded_CompleteAuditTask());
                    processorList.Add(new RefundAuditCashierRefunded_CompleteROAuditTask());
                    break;
                #endregion
                case "ECC_SOINCOME_CREATED":
                    processorList.Add(new CreateSOIncome_CreateConfirmTask());
                    break;
                case "ECC_SOINCOME_CONFIRMED":
                    processorList.Add(new SOIncomeConfirmed_CompleteConfirmTask());
                    break;
                case "ECC_SOINCOMECONFIRM_CANCELED":
                    processorList.Add(new SOIncomeConfirmCanceled_CreateConfirmTask());
                    break;
                case "ECC_SOINCOME_VOIDED":
                    processorList.Add(new SOIncomeAbandoned_CompleteConfirmTask());
                    break;
                case "ECC_SOINCOME_SPLIT":
                    processorList.Add(new SOIncomeSplited_NewAndCompleteConfirmTask());
                    break;
                case "ECC_BALANCEREFUND_CREATED":
                    processorList.Add(new CreateBalanceRefund_CreateCSAuditTask());
                    break;
                case "ECC_BALANCEREFUNDCS_CONFIRMED":
                    processorList.Add(new BalanceRefundCSConfirmed_CreateFinAuditTask());
                    processorList.Add(new BalanceRefundCSConfirmed_CompleteCSAuditTask());
                    break;
                case "ECC_BALANCEREFUNDFIN_CONFIRMED":
                    processorList.Add(new BalanceRefundFinConfirmed_CompleteFinAuditTask());
                    break;
                case "ECC_BALANCEREFUND_VOIDED":
                    processorList.Add(new BalanceRefundAbandoned_CompleteAuditTask());
                    break;
                case "ECC_BALANCERECHARGEREQUEST_CREATED":
                    processorList.Add(new CreateBalanceRechargeRequest_CreateAuditTask());
                    break;
                case "ECC_BALANCERECHARGEREQUEST_AUDITED":
                    processorList.Add(new BalanceRechargeRequestAudited_CompleteAuditTask());
                    break;
                case "ECC_BALANCERECHARGEREQUEST_VOIDED":
                    processorList.Add(new BalanceRechargeRequestAbandoned_CompleteAuditTask());
                    break;
                #endregion

                #region RMA
                //case "PackageCardRMAWaitForRefund":
                //    processorList.Add(new RMARequestCreator());
                //    break;
                //case "RMDRefundSubmit":
                //    processorList.Add(new PackageCardRMAWaitForRefundCompletor());
                //    break;
                //case "RMARequestForServiceProduct":
                //    processorList.Add(new RMARequestForServiceProductCompletor());
                //    break;

                //****** Victor Added:
                case "ECC_CREATERMAREQUEST_AUDITED":
                    //创建RMA申请单待审核待办事项:
                    processorList.Add(new RMARequestWaitingForAuditCreator());
                    break;
                case "ECC_COMPLETERMAREQUEST_AUDITED":
                    //完成RMA申请单待审核待办事项:
                    processorList.Add(new RMARequestWaitingForAuditCompleter());

                    break;
                case "ECC_CREATERMAREFUND_SUBMITED":
                    //创建RMA退款单待审核待办事项:
                    processorList.Add(new RMARefundWaitingForSubmitCreator());
                    break;
                case "ECC_COMPLETERMAREFUND_SUBMITED":
                    //完成RMA退款单待审核待办事项:
                    processorList.Add(new RMARefundWaitingForSubmitCompleter());
                    break;

                case "ECC_CREATEREFUNDBALANCE_AUDITED":
                    //创建RMA退款调整单待审核待办事项:
                    processorList.Add(new RMARefundBalanceWaitingForAuditCreator());
                    break;
                case "ECC_COMPLETEREFUNDBALANCE_AUDITED":
                    //完成RMA退款调整单待审核待办事项:
                    processorList.Add(new RMARefundBalanceWaitingForAuditCompleter());
                    break;

                case "ECC_RMARO_SUBMITED":
                    //RO单提交审核待办事项:
                    processorList.Add(new RMAROSubmitCreator());
                    break;
                case "ECC_RO_CANCELSUBMITED":
                    //RO单取消提交审核待办事项:
                    processorList.Add(new RMAROCancelSubmitCreator());
                    break;
                //RO退款调整单取消审核:
                case "ECC_ROBALANCE_CANCELAUDITED":
                    processorList.Add(new RMAROBalanceCancelAuditCompletor());
                    break;

                #endregion

                #region IM
                case "ECC_PRODUCTPRICE_SUBMITED":
                    processorList.Add(new ProductPriceAuditSubmitTask());
                    break;
                case "ECC_PRODUCTPRICE_AUDITED":
                    processorList.Add(new ProductPriceAuditTask());
                    break;
                case "ECC_PRODUCTPRICE_REJECTED":
                    processorList.Add(new ProductPriceRejectTask());
                    break;
                case "ECC_MANUFACTURER_SUBMITED":
                    processorList.Add(new ManufacturerAuditSubmitTask());
                    break;
                case "ECC_MANUFACTURER_AUDITED":
                    processorList.Add(new ManufacturerAuditTask());
                    break;
                case "ECC_MANUFACTURER_REJECTED":
                    processorList.Add(new ManufacturerRejectTask());
                    break;
                case "ECC_MANUFACTURER_CANCELED":
                    processorList.Add(new ManufacturerCancelTask());
                    break;
                case "ECC_BRAND_SUBMITED":
                    processorList.Add(new BrandAuditSubmitTask());
                    break;
                case "ECC_BRAND_AUDITED":
                    processorList.Add(new BrandAuditTask());
                    break;
                case "ECC_BRAND_REJECTED":
                    processorList.Add(new BrandRejectTask());
                    break;
                case "ECC_CATEGORY_SUBMITED":
                    processorList.Add(new CategoryAuditSubmitTask());
                    break;
                case "ECC_CATEGORY_AUDITED":
                    processorList.Add(new CategoryAuditTask());
                    break;
                case "ECC_CATEGORY_REJECTED":
                    processorList.Add(new CategoryRejectTask());
                    break;
                case "ECC_CATEGORY_CANCELED":
                    processorList.Add(new CategoryCancelTask());
                    break;
                case "ECC_UPDATEPRODUCTPRICEREQUEST_CANCELED":
                    processorList.Add(new CanceledUpdateProductPriceRequest_CompleteUpdateProductPriceRequestTask());
                    break;
                #endregion

                #region Customer
                case "ECC_RMAREFUND_AUDITED":
                    processorList.Add(new AuditRMARefundMessageTask());
                    break;
                case "ECC_RMAREFUND_REJECTED":
                    processorList.Add(new RejectRMARefundMessageTask());
                    break;
                #endregion

                #region PO
                case "ECC_GATHERSETTLEMENT_CREATED":
                    processorList.Add(new CreateGatherSettlement_CreateAuditTask());
                    break;
                case "ECC_GATHERSETTLEMENTAUDIT_CANCELED":
                    processorList.Add(new AuditCanceledGatherSettlement_CreateAuditTask());
                    processorList.Add(new AuditCanceledGatherSettlement_CompleteSettleTask());
                    break;
                case "ECC_GATHERSETTLEMENT_AUDITED":
                    processorList.Add(new AuditedGatherSettlement_CreateSettleTask());
                    processorList.Add(new AuditedGatherSettlement_CompleteAuditTask());
                    break;
                case "ECC_GATHERSETTLEMENT_VOIDED":
                    processorList.Add(new AbandonedGatherSettlement_CompleteAuditTask());
                    break;
                case "ECC_GATHERSETTLEMENT_SETTLED":
                    processorList.Add(new SettledGatherSettlement_CompleteSettleTask());
                    break;
                case "ECC_GATHERSETTLEMENTSETTLE_CANCELED":
                    processorList.Add(new SettleCanceledGatherSettlement_CreateSettleTask());
                    break;

                case "ECC_VENDORREFUNDINFO_SUBMITED":
                    processorList.Add(new CreateVendorRefundInfo_CreateTask());
                    break;
                case "ECC_VENDORREFUNDINFO_AUDITED":
                    processorList.Add(new AuditedVendorRefundInfo_CompleteTask());
                    break;
                case "ECC_VENDORREFUNDINFO_REJECTED":
                    processorList.Add(new RejectedVendorRefundInfo_CompleteTask());
                    break;

                case "ECC_VENDORRANKREQUEST_AUDITED":
                    processorList.Add(new AuditedVendorRankRequest_CompleteTask());
                    break;
                case "ECC_VENDORRANKREQUEST_CANCELED":
                    processorList.Add(new CanceledVendorRefundInfo_CompleteTask());
                    break;
                case "ECC_VENDORRANKREQUEST_SUBMITED":
                    processorList.Add(new SubmitVendorRankRequest_CreateTask());
                    break;

                case "ECC_VENDORFINANCEINFOREQUEST_SUBMITED":
                    processorList.Add(new SubmitVendorFinanceInfoRequest_CreateTask());
                    break;
                case "ECC_VENDORFINANCEINFOREQUEST_AUDITED":
                    processorList.Add(new AuditedVendorFinanceInfoRequest_CompleteTask());
                    break;
                case "ECC_VENDORFINANCEINFOREQUEST_REJECTED":
                    processorList.Add(new RejectedVendorFinanceInfoRequest_CompleteTask());
                    break;
                case "ECC_VENDORFINANCEINFOREQUEST_CANCELED":
                    processorList.Add(new CanceledVendorFinanceInfoRequest_CompleteTask());
                    break;

                case "ECC_PURCHASEORDER_SUBMITED":
                    processorList.Add(new SubmitPurchaseOrderAudit_CreateTask());
                    break;
                case "ECC_PURCHASEORDER_CONFIRMED":
                    processorList.Add(new PurchaseOrderAuditConfirm_CompleteTask());
                    break;
                case "ECC_PURCHASEORDER_REJECTED":
                    processorList.Add(new PurchaseOrderAuditReject_CompleteTask());
                    break;

                case "ECC_PURCHASEORDERETATIMEINFO_SUBMITED":
                    processorList.Add(new SubmitPurchaseOrderETATimeInfo_CreateTask());
                    break;
                case "ECC_PURCHASEORDERETATIMEINFO_AUDITED":
                    processorList.Add(new PurchaseOrderETATimeInfoAudit_CompleteTask());
                    break;
                case "ECC_PURCHASEORDERETATIMEINFO_CANCELED":
                    processorList.Add(new PurchaseOrderETATimeInfoCancel_CompleteTask());
                    break;

                case "ECC_SETTLEMENTRULE_CREATED":
                    processorList.Add(new SettlementRuleCreate_CreateTask());
                    break;
                case "ECC_SETTLEMENTRULE_AUDITED":
                    processorList.Add(new SettlementRuleAudit_CompleteTask());
                    break;
                case "ECC_SETTLEMENTRULE_ABANDONED":
                    processorList.Add(new SettlementRuleAbandon_CompleteTask());
                    break;

                case "ECC_CONSIGNSETTLEMENT_CREATED":
                    processorList.Add(new ConsignSettlementCreate_CreateTask());
                    break;
                case "ECC_CONSIGNSETTLEMENT_CANCELED":
                    processorList.Add(new ConsignSettlementCancel_CompleteTask());
                    break;
                case "ECC_CONSIGNSETTLEMENT_ABANDONED":
                    processorList.Add(new ConsignSettlementAbandon_CompleteTask());
                    break;
                case "ECC_CONSIGNSETTLEMENT_AUDITED":
                    processorList.Add(new ConsignSettlementAudit_CompleteTask());
                    processorList.Add(new ConsignSettlementAuditCreate_CreateTask());
                    break;
                case "ECC_CONSIGNSETTLEMENT_SETTLEMENTED":
                    processorList.Add(new ConsignSettlementSettlement_CompleteTask());
                    break;
                case "ECC_CONSIGNSETTLEMENT_CANCELSETTLEMENTED":
                    processorList.Add(new ConsignSettlementCancelSettlement_CompleteTask());
                    break;

                case "ECC_COLLECTIONPAYMENT_CREATED":
                    processorList.Add(new CollectionPaymentCreate_CreateTask());
                    break;
                case "ECC_COLLECTIONPAYMENT_CANCELED":
                    processorList.Add(new CollectionPaymentCancel_CompleteTask());
                    break;
                case "ECC_COLLECTIONPAYMENT_ABANDONED":
                    processorList.Add(new CollectionPaymentAbandon_CompleteTask());
                    break;
                case "ECC_COLLECTIONPAYMENT_AUDITED":
                    processorList.Add(new CollectionPaymentAudit_CompleteTask());
                    processorList.Add(new CollectionPaymentAuditCreate_CreateTask());
                    break;
                case "ECC_COLLECTIONPAYMENT_SETTLEMENTED":
                    processorList.Add(new CollectionPaymentSettlement_CompleteTask());
                    break;
                case "ECC_COLLECTIONPAYMENT_CANCELSETTLEMENTED":
                    processorList.Add(new CollectionPaymentCancelSettlement_CompleteTask());
                    break;

                #endregion
            }

            return processorList;
        }
    }

}
