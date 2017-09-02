using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Invoice.BizProcessor.ETPCalculator;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    /// <summary>
    /// 针对于代收结算单的处理
    /// </summary>
    [VersionExport(typeof(IProcess), new string[] { "CollectionSettlementProcess" })]
    [Export(typeof(ProcessBase))]
    public class CollectionSettlementProcess : ProcessBase
    {
        #region For Pay

        protected override void BeforeProcessForPay(PayItemInfo payItemInfo)
        {
        }

        #endregion For Pay

        #region For Create

        protected override void PreCheckForCreate(PayItemInfo entity)
        {
            base.PreCheckForCreate(entity);

            GatherSettlementInfo collectionSettlement = ExternalDomainBroker.GetGatherSettlementInfo(entity.OrderSysNo.Value);
            if (collectionSettlement == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", entity.OrderSysNo);
            }
            //填写代收结算单上的CompanyCode为付款单CompanyCode
            entity.CompanyCode = collectionSettlement.CompanyCode;
            entity.OrderStatus = (int?)collectionSettlement.SettleStatus;

            if (collectionSettlement.SettleStatus == GatherSettleStatus.SET)
            {
                if (entity.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_Create_OnlyCanAddNormalPayForVendorSettleOrder");
                }
            }
            else
            {
                ThrowBizException("PayItem_Create_SettleStatusInvalid");
            }

            List<PayableInfo> payList = PayableBizProcessor.GetListByCriteria(new PayableInfo
            {
                OrderSysNo = entity.OrderSysNo,
                OrderType = entity.OrderType
            });

            //如果该单据已经有应付了，对该应付作检查
            if (payList != null && payList.Count > 0)
            {
                ReferencePayableInfo = payList[0];
                if (ReferencePayableInfo.PayStatus == PayableStatus.FullPay)
                {
                    ThrowBizException("PayItem_Create_FullPay");
                }
            }
        }

        protected override void ProcessReferencePayableInfoForCreate(PayItemInfo entity)
        {
            int currencySysNo = 1;
            if (ReferencePayableInfo != null)
            {
                //如果该付款单对应的应付款已经Abandon，那么此时需要重新激活该应付款并将其更新
                if (ReferencePayableInfo.PayStatus == PayableStatus.Abandon)
                {
                    ReferencePayableInfo.OrderAmt = entity.PayAmt;
                    ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                    ReferencePayableInfo.EIMSNo = null;

                    PayableBizProcessor.UpdateStatusAndOrderAmt(ReferencePayableInfo);

                    //计算ETP
                    ReferencePayableInfo.EstimatedTimeOfPay = ETPCalculatorHelper.GetETPByPayPeriod(new PayableInfo
                    {
                        OrderType = PayableOrderType.CollectionSettlement,
                        OrderSysNo = entity.OrderSysNo.Value
                    }, DateTime.Now);
                    PayableBizProcessor.UpdatePayableETP(ReferencePayableInfo);

                    entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                    entity.CurrencySysNo = currencySysNo;
                    entity.OrderType = ReferencePayableInfo.OrderType;
                    entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
                }

                if (ReferencePayableInfo.PayStatus == PayableStatus.UnPay)
                {
                    //计算ETP
                    ReferencePayableInfo.EstimatedTimeOfPay = ETPCalculatorHelper.GetETPByPayPeriod(new PayableInfo
                    {
                        OrderType = PayableOrderType.CollectionSettlement,
                        OrderSysNo = entity.OrderSysNo.Value
                    }, DateTime.Now);
                    PayableBizProcessor.UpdatePayableETP(ReferencePayableInfo);

                    entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                    entity.CurrencySysNo = currencySysNo;
                    entity.OrderType = ReferencePayableInfo.OrderType;
                    entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
                }
            }
            else
            {
                ReferencePayableInfo = new PayableInfo();
                ReferencePayableInfo.OrderSysNo = entity.OrderSysNo.Value;
                ReferencePayableInfo.OrderType = entity.OrderType.Value;
                ReferencePayableInfo.AlreadyPayAmt = 0M;
                ReferencePayableInfo.OrderAmt = entity.PayAmt;
                ReferencePayableInfo.CurrencySysNo = currencySysNo;
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.InStockAmt = 0M;
                ReferencePayableInfo.Note = "Auto created CollectionSettlementPayItem!";
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;

                //计算ETP
                ReferencePayableInfo.EstimatedTimeOfPay = ETPCalculatorHelper.GetETPByPayPeriod(new PayableInfo
                {
                    OrderType = PayableOrderType.CollectionSettlement,
                    OrderSysNo = entity.OrderSysNo.Value
                }, DateTime.Now);
                ReferencePayableInfo.EIMSNo = null;
                ReferencePayableInfo = PayableBizProcessor.Create(ReferencePayableInfo);

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = currencySysNo;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
        }

        #endregion For Create

        #region For CancelAbandon

        protected override PayItemInfo PreCheckForCancelAbandon(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = base.PreCheckForCancelAbandon(entity, out payableInfo);

            GatherSettlementInfo collectionSettlement = ExternalDomainBroker.GetGatherSettlementInfo(payItemInfo.OrderSysNo.Value);
            if (collectionSettlement.SettleStatus == GatherSettleStatus.SET)
            {
                if (payItemInfo.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_CancelAbandon_VendorSettleOrderStatusNotMatchPayStyleForGatherSettlement");
                }
            }
            else
            {
                ThrowBizException("PayItem_CancelAbandon_VendorSettleOrderStatusNotSettledForGatherSettlement");
            }
            return payItemInfo;
        }

        protected override void AfterProcessForCancelAbandon(PayItemInfo payItemInfo)
        {
        }

        #endregion For CancelAbandon

        #region For CancelPay

        protected override void BeforeProcessForCancelPay(PayItemInfo payItemInfo)
        {
        }

        #endregion For CancelPay
    }
}