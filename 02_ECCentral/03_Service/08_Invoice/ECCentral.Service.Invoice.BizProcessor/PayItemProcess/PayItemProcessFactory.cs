using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    public static class PayItemProcessFactory
    {
        public static IProcess Get(PayableOrderType orderType)
        {
            switch (orderType)
            {
                case PayableOrderType.PO:
                case PayableOrderType.POAdjust:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "POProcess" });

                case PayableOrderType.VendorSettleOrder:
                case PayableOrderType.LeaseSettle:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "VendorSettleOrderProcess" });

                //case PayableOrderType.ReturnPointCashAdjust:
                //case PayableOrderType.SubAccount:
                //case PayableOrderType.SubInvoice:
                //    return ObjectFactory<IProcess>.NewInstance(new string[] { "EIMSReturnPointCashProcess" });

                case PayableOrderType.RMAPOR:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "RMAVendorRefundProcess" });

                case PayableOrderType.CollectionSettlement:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "CollectionSettlementProcess" });

                case PayableOrderType.Commission:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "CommissionProcess" });
                case PayableOrderType.CollectionPayment:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "CollectionPaymentProcess" });
                case PayableOrderType.GroupSettle:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "GroupSettleProcess" });
                //下面为不需要复杂校验的应付类型处理
                case PayableOrderType.CostChange:
                case PayableOrderType.ConsignAdjust:
                    return ObjectFactory<IProcess>.NewInstance(new string[] { "OtherOrderTypeProcess" }); 

                default:
                    throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_InvalidOrderType"));
            }
        }
    }
}