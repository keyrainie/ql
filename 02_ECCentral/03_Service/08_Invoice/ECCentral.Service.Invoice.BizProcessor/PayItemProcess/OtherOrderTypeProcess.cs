using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.ComponentModel.Composition;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    [VersionExport(typeof(IProcess), new string[] { "OtherOrderTypeProcess" })]
    [Export(typeof(ProcessBase))]
    public class OtherOrderTypeProcess : ProcessBase
    {
        public OtherOrderTypeProcess()
        { }

        protected override void BeforeProcessForPay(BizEntity.Invoice.PayItemInfo payItemInfo)
        {
            
        }

        protected override void PreCheckForCreate(PayItemInfo entity)
        {
            base.PreCheckForCreate(entity);


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
                    entity.CurrencySysNo = 1;
                    entity.OrderType = ReferencePayableInfo.OrderType;
                    entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
                }

                if (ReferencePayableInfo.PayStatus == PayableStatus.UnPay)
                {
                    entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                    entity.CurrencySysNo = 1;
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
                ReferencePayableInfo.CurrencySysNo = 1;
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.Note = "Auto created by system!";
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;
              
                ReferencePayableInfo = PayableBizProcessor.Create(ReferencePayableInfo);

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = 1;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
        }

        protected override void AfterProcessForCancelAbandon(BizEntity.Invoice.PayItemInfo payItemInfo)
        {
            //throw new NotImplementedException();
        }

        protected override void BeforeProcessForCancelPay(BizEntity.Invoice.PayItemInfo payItemInfo)
        {
            //throw new NotImplementedException();
        }
    }
}
