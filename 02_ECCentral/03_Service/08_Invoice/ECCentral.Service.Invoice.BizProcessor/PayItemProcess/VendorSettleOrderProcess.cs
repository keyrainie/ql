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
    /// 针对于代销结算单的处理
    /// </summary>
    [VersionExport(typeof(IProcess), new string[] { "VendorSettleOrderProcess" })]
    [Export(typeof(ProcessBase))]
    public class VendorSettleOrderProcess : ProcessBase
    {
        #region For Pay

        protected override void BeforeProcessForPay(PayItemInfo payItemInfo)
        {
        }

        #endregion For Pay

        #region For Create

        private ConsignSettlementInfo m_ConsignSettlement;

        protected override void PreCheckForCreate(PayItemInfo entity)
        {
            base.PreCheckForCreate(entity);

            m_ConsignSettlement = ExternalDomainBroker.GetConsignSettlementInfo(entity.OrderSysNo.Value);

            if (m_ConsignSettlement == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", entity.OrderSysNo);
            }
            //填写代销结算单上的CompanyCode为付款单CompanyCode
            entity.CompanyCode = m_ConsignSettlement.CompanyCode;
            entity.OrderStatus = (int?)m_ConsignSettlement.Status;

            if (m_ConsignSettlement.Status == SettleStatus.SettlePassed)
            {
                if (entity.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_Create_OnlyCanAddNormalPayForVendorSettleOrder");
                }
            }
            else
            {
                ThrowBizException("PayItem_Create_CannotAddPayForUnSettled");
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
            //如果该付款单对应的应付款已经Abandon，那么此时需要重新激活该应付款并将其更新
            if (ReferencePayableInfo != null)
            {
                if (ReferencePayableInfo.PayStatus == PayableStatus.Abandon)
                {
                    ReferencePayableInfo.OrderAmt = entity.PayAmt;
                    ReferencePayableInfo.PayStatus = PayableStatus.UnPay;

                    if (entity.OrderType == PayableOrderType.VendorSettleOrder)
                    {
                        ReferencePayableInfo.EIMSNo = ExternalDomainBroker.GetConsignSettlementReturnPointSysNo(entity.OrderSysNo.Value);
                    }
                    else
                    {
                        ReferencePayableInfo.EIMSNo = null;
                    }
                    PayableBizProcessor.UpdateStatusAndOrderAmt(ReferencePayableInfo);

                    entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                    entity.CurrencySysNo = m_ConsignSettlement.CurrencyCode;
                    entity.OrderType = ReferencePayableInfo.OrderType;
                    entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
                }

                if (ReferencePayableInfo.PayStatus == PayableStatus.UnPay)
                {
                    entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                    entity.CurrencySysNo = m_ConsignSettlement.CurrencyCode;
                    entity.OrderType = ReferencePayableInfo.OrderType;
                    entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
                }
            }
            else
            {
                ReferencePayableInfo = new PayableInfo();
                ReferencePayableInfo.OrderSysNo = entity.OrderSysNo.Value;
                ReferencePayableInfo.OrderType = entity.OrderType.Value;
                ReferencePayableInfo.AlreadyPayAmt = 0;
                ReferencePayableInfo.OrderAmt = entity.PayAmt;
                ReferencePayableInfo.CurrencySysNo = m_ConsignSettlement.CurrencyCode;
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.Note = "Auto created by system!";
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;

                if (entity.OrderType == PayableOrderType.VendorSettleOrder)
                {
                    ReferencePayableInfo.EIMSNo = ExternalDomainBroker.GetConsignSettlementReturnPointSysNo(entity.OrderSysNo.Value);
                }
                ReferencePayableInfo = PayableBizProcessor.Create(ReferencePayableInfo);

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = m_ConsignSettlement.CurrencyCode;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
        }

        #endregion For Create

        #region For CancelAbandon

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