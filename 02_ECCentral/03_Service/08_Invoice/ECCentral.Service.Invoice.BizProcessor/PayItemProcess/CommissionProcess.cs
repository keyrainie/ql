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
    /// 针对于佣金账扣单的处理
    /// </summary>
    [VersionExport(typeof(IProcess), new string[] { "CommissionProcess" })]
    [Export(typeof(ProcessBase))]
    public class CommissionProcess : ProcessBase
    {
        #region For Pay

        protected override void BeforeProcessForPay(PayItemInfo payItemInfo)
        {
            //DO NOTHING
        }

        #endregion For Pay

        #region For Create

        protected override void PreCheckForCreate(PayItemInfo entity)
        {
            base.PreCheckForCreate(entity);

            CommissionMaster commissionMaster = ExternalDomainBroker.GetCommissionMaster(entity.OrderSysNo.Value);
            if (commissionMaster == null)
            {
                ThrowBizException("PayItem_OrderNotExisitsFormat", entity.OrderSysNo);
            }
            //填写佣金账扣上的CompanyCode为付款单CompanyCode
            entity.CompanyCode = commissionMaster.CompanyCode;
            entity.OrderStatus = (int?)commissionMaster.Status;

            if (commissionMaster.Status == VendorCommissionMasterStatus.SET)
            {
                if (entity.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_Create_OnlyCanAddNormalPayForVendorSettleOrder");
                }
            }
            else
            {
                ThrowBizException("PayItem_Create_CommissionMasterStatusInvalid");
            }

            List<PayableInfo> payList = PayableBizProcessor.GetListByCriteria(new PayableInfo()
            {
                OrderSysNo = entity.OrderSysNo,
                OrderType = entity.OrderType
            });

            //如果该单据已经有支付，对该应付作检查
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
                if (ReferencePayableInfo.PayStatus == PayableStatus.Abandon)
                {
                    ReferencePayableInfo.OrderAmt = entity.PayAmt;
                    ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                    ReferencePayableInfo.EIMSNo = null;

                    PayableBizProcessor.UpdateStatusAndOrderAmt(ReferencePayableInfo);
                }

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = currencySysNo;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
            else
            {
                ReferencePayableInfo = new PayableInfo();
                ReferencePayableInfo.OrderSysNo = entity.OrderSysNo.Value;
                ReferencePayableInfo.OrderType = entity.OrderType.Value;
                ReferencePayableInfo.AlreadyPayAmt = 0;
                ReferencePayableInfo.OrderAmt = entity.PayAmt;
                ReferencePayableInfo.CurrencySysNo = currencySysNo;
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.Note = "Auto created CommissionPayItem!";
                ReferencePayableInfo.InStockAmt = 0.00m;
                ReferencePayableInfo.EstimatedTimeOfPay = DateTime.Now;
                ReferencePayableInfo.EIMSNo = null;
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;

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

            CommissionMaster commissionMaster = ExternalDomainBroker.GetCommissionMaster(payableInfo.OrderSysNo.Value);
            if (commissionMaster.Status == VendorCommissionMasterStatus.SET)
            {
                if (payItemInfo.PayStyle != PayItemStyle.Normal)
                {
                    ThrowBizException("PayItem_CancelAbandon_VendorSettleOrderStatusNotMatchPayStyleForCommissionMaster");
                }
            }
            else
            {
                ThrowBizException("PayItem_CancelAbandon_VendorSettleOrderStatusNotSettledForCommissionMaster");
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
            //DO NOTHING
        }

        #endregion For CancelPay

        #region For Lock

        protected override PayItemInfo PreCheckForLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_Lock_InvalidOrderType"));
        }

        #endregion For Lock

        #region For CancelLock

        protected override PayItemInfo PreCheckForCancelLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_UnLock_InvalidOrderType"));
        }

        #endregion For CancelLock
    }
}