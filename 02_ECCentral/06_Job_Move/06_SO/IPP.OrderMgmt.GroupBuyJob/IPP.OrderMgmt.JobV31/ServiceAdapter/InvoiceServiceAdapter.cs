using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.ServiceContracts;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.DataContracts;
using Newegg.Oversea.Framework.Contract;

namespace IPP.OrderMgmt.JobV31.ServiceAdapter
{
    public class InvoiceServiceAdapter
    {

        public static void AuditNetPay(int soSysNo)
        {

            NetPayV31 message = new NetPayV31
            {
                Header = ServiceAdapterHelper.ApplyMessageHeader(),
                Body = new NetPayMessage { SOSysNo = soSysNo }
            };


            IMaintainNetPayV31 service = ServiceBroker.FindService<IMaintainNetPayV31>();


            try
            {
                //SimpleTypeDataContract<bool> BatchApproveGroupBuy(NetPayV31 msg);
                SimpleTypeDataContract<bool> ResultMsg = service.BatchApproveGroupBuy(message);

                ServiceAdapterHelper.DealServiceFault(ResultMsg.Faults);

            }
            finally
            {
                ServiceBroker.DisposeService<IMaintainNetPayV31>(service);
            }

        }

        public static void CreateAO(int soSysNo)
        {
            FinanceIncomeV31 message = new FinanceIncomeV31
            {
                Header = ServiceAdapterHelper.ApplyMessageHeader(),
                Body = new FinanceIncomeMessage()
            };

            message.Body.SOIncome = new SOIncomeMessage
            {
                OrderSysNo = soSysNo
            };

            message.Body.SOIncomeRefund = new SOIncomeRefundMessage 
            {
                 Note="团购订单作废",
                 RefundReason=null,
                 RefundPayType= SOIncomeRefundPayType.PrepayRefund
            };

            message.Body.Type = FinanceIncomeType.AO;


            ICreateSOIncomeRefundV31 service = ServiceBroker.FindService<ICreateSOIncomeRefundV31>();


            try
            {

                SimpleTypeDataContract<bool> ResultMsg = service.CreateNegativeFinanceIncome(message);

                ServiceAdapterHelper.DealServiceFault(ResultMsg.Faults);

            }
            finally
            {
                ServiceBroker.DisposeService<ICreateSOIncomeRefundV31>(service);
            }
 
        }
    }
}
