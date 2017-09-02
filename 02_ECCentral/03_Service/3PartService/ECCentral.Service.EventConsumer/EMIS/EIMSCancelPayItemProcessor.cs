using ECCentral.BizEntity;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;
using Newegg.EIMSCN.ServiceInterfaces.DataContracts;
using Newegg.EIMSCN.ServiceInterfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventConsumer
{
    public class EIMSCancelPayItemProcessor : IConsumer<EIMSCancelPayMessage>
    {
        public void HandleEvent(EIMSCancelPayMessage eventMessage)
        {
            IEIMSInterfaceService service = WCFAdapter<IEIMSInterfaceService>.GetProxy();
            var msg = new EIMSMessage<CancelPayItem>();
            msg.Header = new EIMSMessageHeader();
            msg.Header.CompanyCode = eventMessage.CompanyCode;
            msg.Header.UserID = eventMessage.UserID;
            msg.Body = new CancelPayItem
            {
                AcctinvoiceNumber = eventMessage.AcctinvoiceNumber,
                EIMSInvoiceNumber = eventMessage.EIMSInvoiceNumber,
                invoiceStatus = (InvoiceStatus)eventMessage.InvoiceStatus,
                payStatus = eventMessage.PayStatus == -1 ? PayableStatus.Abandon : (PayableStatus)eventMessage.PayStatus
            };

            var result = service.CancelPayItem(msg);
            if (!string.IsNullOrEmpty(result))
            {
                throw new BizException(result);
            }
        }


        public ExecuteMode ExecuteMode
        {
            get { return Utility.ExecuteMode.Sync; }
        }
    }
}
