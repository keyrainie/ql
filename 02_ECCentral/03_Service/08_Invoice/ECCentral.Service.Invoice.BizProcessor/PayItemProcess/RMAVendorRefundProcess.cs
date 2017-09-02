using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    /// <summary>
    /// 针对于RMA供应商退款单的处理
    /// </summary>
    [VersionExport(typeof(IProcess), new string[] { "RMAVendorRefundProcess" })]
    [Export(typeof(ProcessBase))]
    public class RMAVendorRefundProcess : ProcessBase
    {
        #region For Pay

        protected override void BeforeProcessForPay(PayItemInfo payItemInfo)
        {
            //DO NOTHING
        }

        #endregion For Pay

        #region For Create

        protected override void ProcessReferencePayableInfoForCreate(PayItemInfo entity)
        {
            var now = System.DateTime.Now;
            List<PayableInfo> payList = PayableBizProcessor.GetListByCriteria(new PayableInfo
            {
                OrderSysNo = entity.OrderSysNo,
                OrderType = entity.OrderType
            });

            if (payList != null && payList.Count > 0)
            {
                ReferencePayableInfo = payList[0];
                if (ReferencePayableInfo.PayStatus == PayableStatus.Abandon)
                {
                    ReferencePayableInfo.OrderAmt = entity.PayAmt;
                    ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                    ReferencePayableInfo.EIMSNo = null;
                    ReferencePayableInfo.EstimatedTimeOfPay = now;
                    PayableBizProcessor.UpdateStatusAndOrderAmt(ReferencePayableInfo);
                }
                entity.CurrencySysNo = 1;
                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
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
                ReferencePayableInfo.CurrencySysNo = 1;// 人民币类型
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.Note = "Auto created by system!";
                ReferencePayableInfo.EIMSNo = null;
                ReferencePayableInfo.EstimatedTimeOfPay = now;
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;
                ReferencePayableInfo = PayableBizProcessor.Create(ReferencePayableInfo);

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = 1;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
        }

        #endregion For Create

        #region For CancelAbandon

        protected override PayItemInfo PreCheckForCancelAbandon(PayItemInfo entity, out PayableInfo payableInfo)
        {
            throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_CancelAbandon_InvalidOrderType"));
        }

        protected override void AfterProcessForCancelAbandon(PayItemInfo payItemInfo)
        {
            //DO NOTHING
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
