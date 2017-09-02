using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    /// <summary>
    /// 针对于采购单的处理
    /// </summary>
    [VersionExport(typeof(IProcess), new string[] { "POProcess" })]
    [Export(typeof(ProcessBase))]
    public class POProcess : ProcessBase
    {
        #region For Pay

        protected override PayItemInfo PreCheckForPay(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = base.PreCheckForPay(entity, out payableInfo);

            if (entity.OrderType == PayableOrderType.POAdjust)
            {
                //PO调整单不用作额外的检查
                return payItemInfo;
            }

            var poInfo = ExternalDomainBroker.GetPurchaseOrderInfo(payableInfo.OrderSysNo.Value, 0);
            if (poInfo == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", payItemInfo.OrderSysNo);
            }

            if (payItemInfo.PayStyle == PayItemStyle.Advanced)
            {
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Abandoned)
                {
                    ThrowBizException("PayItem_Pay_AbandonedPOCanNotAdvanced");
                }
            }
            else
            {
                if (payItemInfo.OrderType != PayableOrderType.POAdjust
                    && poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.InStocked
                    && poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.PartlyInStocked
                    && poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.SystemClosed
                    && poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.ManualClosed
                    && poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.VendorClosed)
                {
                    ThrowBizException("PayItem_Pay_POStatusInvalid");
                }
            }
            return payItemInfo;
        }

        protected override void BeforeProcessForPay(PayItemInfo payItemInfo)
        {
            //DO NOTHING
        }

        #endregion For Pay

        #region For Cancel Pay

        protected override PayItemInfo PreCheckForCancelPay(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = base.PreCheckForCancelPay(entity, out payableInfo);

            var poInfo = ExternalDomainBroker.GetPurchaseOrderInfo(payableInfo.OrderSysNo.Value, 0);
            if (poInfo == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", payItemInfo.OrderSysNo);
            }
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.WaitingInStock)
            {
                if (payItemInfo.PayStyle != PayItemStyle.Advanced)
                {
                    ThrowBizException("PayItem_CancelPay_OnlyCanAdvancePay");
                }
            }
            else if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.VendorClosed)
            {
                if (payItemInfo.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_CancelPay_OnlyCanNormalPay");
                }
            }
            else
            {
                ThrowBizException("PayItem_CancelPay_POStatusInvalid");
            }
            return payItemInfo;
        }

        protected override void BeforeProcessForCancelPay(PayItemInfo payItemInfo)
        {
        }

        #endregion For Cancel Pay

        #region For Create

        private PurchaseOrderInfo m_POInfo;

        protected override void PreCheckForCreate(PayItemInfo entity)
        {
            base.PreCheckForCreate(entity);

            m_POInfo = ExternalDomainBroker.GetPurchaseOrderInfo(entity.OrderSysNo.Value, 0);
            if (m_POInfo == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", entity.OrderSysNo);
            }
            if (m_POInfo.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.Consign)
            {
                ThrowBizException("PayItem_Create_InvalidPOConsignStatus");
            }
            //填写PO的CompanyCode为付款单的CompanyCode
            entity.CompanyCode = m_POInfo.CompanyCode;
            entity.OrderStatus = (int?)m_POInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus;

            if (entity.PayStyle == PayItemStyle.Advanced)
            {
                decimal usingReturnPoint = 0M;
                if (m_POInfo.EIMSInfo != null && m_POInfo.EIMSInfo.EIMSInfoList != null)
                {
                    usingReturnPoint = m_POInfo.EIMSInfo.EIMSInfoList.Sum(s => s.EIMSAmt ?? 0M);
                }
                decimal poTotalAmt = m_POInfo.PurchaseOrderBasicInfo.TotalAmt ?? 0M;
                if (entity.PayAmt != poTotalAmt - usingReturnPoint)
                {
                    ThrowBizException("PayItem_Create_AdvancedPayPayedAmtNeedEqualUnPayedAmtFormat", entity.PayAmt.Value.ToString("0.00"), poTotalAmt.ToString("0.00"), usingReturnPoint.ToString("0.00"));
                }
            }
            if (m_POInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.WaitingInStock)
            {
                if (entity.PayStyle != PayItemStyle.Advanced)
                {
                    ThrowBizException("PayItem_Create_CanOnlyAddAdvancePayItemForPO");
                }
            }
            else if (m_POInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked || m_POInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked)
            {
                if (entity.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_Create_CanOnlytAddNormalPayItemForPO");
                }
            }
            else
            {
                ThrowBizException("PayItem_Create_CannotAddPayItemForOtherPOStatus");
            }

            List<PayableInfo> payList = PayableBizProcessor.GetListByCriteria(new PayableInfo
            {
                OrderSysNo = entity.OrderSysNo,
                OrderType = entity.OrderType,
                BatchNumber = entity.BatchNumber
            });

            #region 如果该单据已经有应付了，对该应付作检查

            if (payList != null && payList.Count > 0)
            {
                //是否有已作废的应付款
                List<PayableInfo> adandonPayList = payList.Where(w => w.PayStatus == PayableStatus.Abandon).ToList();

                if (adandonPayList.Count > 0)
                {
                    ReferencePayableInfo = adandonPayList[0];
                }
                else
                {
                    ReferencePayableInfo = payList[0];

                    var payItems = PayItemBizProcessor.GetListByCriteria(new PayItemInfo
                    {
                        PaySysNo = ReferencePayableInfo.SysNo
                    });
                    payItems.Add(entity);

                    if (payItems.Where(x => x.Status != PayItemStatus.Abandon).Sum(x => x.PayAmt) > ReferencePayableInfo.OrderAmt)
                    {
                        ThrowBizException("PayItem_Create_TotalPayAmtCanNotMoreThanOrderAmt");
                    }

                    //该检查现阶段只针对PO，因为其他类型单据没有预付款
                    if (entity.PayStyle == PayItemStyle.Normal)
                    {
                        var list = PayItemBizProcessor.GetListByCriteria(new PayItemInfo
                        {
                            Status = PayItemStatus.Origin,
                            PayStyle = PayItemStyle.Advanced,
                            PaySysNo = ReferencePayableInfo.SysNo
                        });
                        if (list != null && list.Count > 0)
                        {
                            ThrowBizException("PayItem_Create_AdvancedPayItemExists");
                        }
                    }
                    if (ReferencePayableInfo.PayStatus == PayableStatus.FullPay)
                    {
                        ThrowBizException("PayItem_Create_FullPay");
                    }
                }
            }

            #endregion 如果该单据已经有应付了，对该应付作检查
        }

        protected override void ProcessReferencePayableInfoForCreate(PayItemInfo entity)
        {
            var now = DateTime.Now;

            //如果该付款单对应的应付款已经Abandon，那么此时需要重新激活该应付款并将其更新
            if (ReferencePayableInfo != null)
            {
                if (ReferencePayableInfo.PayStatus == PayableStatus.Abandon)
                {
                    ReferencePayableInfo.OrderAmt = entity.PayAmt;
                    ReferencePayableInfo.PayStatus = PayableStatus.UnPay;

                    ReferencePayableInfo.InStockAmt = entity.InStockAmt;
                    ReferencePayableInfo.EIMSAmt = entity.EIMSAmt;
                    ReferencePayableInfo.RawOrderAmt = entity.RawOrderAmt;

                    CaclETP(entity, now);

                    PayableBizProcessor.UpdateStatusAndOrderAmt(ReferencePayableInfo);
                }
                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = m_POInfo.PurchaseOrderBasicInfo.CurrencyCode;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
            else
            {
                ReferencePayableInfo = new PayableInfo();
                ReferencePayableInfo.OrderSysNo = entity.OrderSysNo.Value;
                ReferencePayableInfo.OrderType = entity.OrderType.Value;
                ReferencePayableInfo.CurrencySysNo = m_POInfo.PurchaseOrderBasicInfo.CurrencyCode;
                ReferencePayableInfo.OrderAmt = entity.PayAmt;
                ReferencePayableInfo.AlreadyPayAmt = 0;
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.Note = "Auto created by system!";
                ReferencePayableInfo.BatchNumber = entity.BatchNumber.HasValue ? entity.BatchNumber : 1;
                ReferencePayableInfo.EIMSAmt = entity.EIMSAmt;
                ReferencePayableInfo.InStockAmt = entity.InStockAmt;
                ReferencePayableInfo.RawOrderAmt = entity.RawOrderAmt;
                ReferencePayableInfo.OrderStatus = entity.OrderStatus;
                ReferencePayableInfo.EstimatedTimeOfPay = entity.EstimatedTimeOfPay;
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;

                CaclETP(entity, now);

                ReferencePayableInfo = PayableBizProcessor.Create(ReferencePayableInfo);

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = m_POInfo.PurchaseOrderBasicInfo.CurrencyCode;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
        }

        private void CaclETP(PayItemInfo payItem, DateTime now)
        {
            //负采购，调整单，预付款ETP是当前时间
            if (payItem.PayAmt <= 0
                || payItem.PayStyle == PayItemStyle.Advanced)
            {
                ReferencePayableInfo.EstimatedTimeOfPay = now;
            }
        }

        #endregion For Create

        #region For CancelAbandon

        protected override PayItemInfo PreCheckForCancelAbandon(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = base.PreCheckForCancelAbandon(entity, out payableInfo);

            var poInfo = ExternalDomainBroker.GetPurchaseOrderInfo(payItemInfo.OrderSysNo.Value, 0);
            if (poInfo == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", payItemInfo.OrderSysNo);
            }
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.WaitingInStock)
            {
                if (payItemInfo.PayStyle != PayItemStyle.Advanced)
                {
                    ThrowBizException("PayItem_CancelAbandon_AdvancePayNotMatchPOStatus");
                }
            }
            else if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.InStocked
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.PartlyInStocked
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.SystemClosed
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.ManualClosed
                || poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.VendorClosed)
            {
                if (payItemInfo.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_CancelAbandon_NormalPayNotMatchPOStatus");
                }
            }
            else
            {
                ThrowBizException("PayItem_CancelAbandon_StatusInvalid");
            }
            return payItemInfo;
        }

        protected override void AfterProcessForCancelAbandon(PayItemInfo entity)
        {
        }

        #endregion For CancelAbandon

        #region For Lock

        protected override PayItemInfo PreCheckForLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            if (entity.OrderType == PayableOrderType.POAdjust)
            {
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_Lock_InvalidOrderType"));
            }

            return base.PreCheckForLock(entity, out payableInfo);
        }

        #endregion For Lock

        #region For CancelLock

        protected override PayItemInfo PreCheckForCancelLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            if (entity.OrderType == PayableOrderType.POAdjust)
            {
                throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_UnLock_InvalidOrderType"));
            }

            return base.PreCheckForCancelLock(entity, out payableInfo);
        }

        #endregion For CancelLock
    }
}