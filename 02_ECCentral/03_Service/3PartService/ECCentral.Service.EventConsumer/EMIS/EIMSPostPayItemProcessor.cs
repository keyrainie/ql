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
    public class EIMSPostPayItemProcessor : IConsumer<EIMSPayMessage>
    {
        public void HandleEvent(EIMSPayMessage eventMessage)
        {
            IEIMSInterfaceService service = WCFAdapter<IEIMSInterfaceService>.GetProxy();
            var msg = new EIMSMessage<InvoiceReceiveInfo>();
            msg.Header = new EIMSMessageHeader();
            EIMSMessageResult result;
            msg.Header.CompanyCode = eventMessage.CompanyCode;
            msg.Header.UserID = eventMessage.UserID;
            msg.Body = new InvoiceReceiveInfo();
            msg.Body.AcctInvoiceNumber = eventMessage.AcctInvoiceNumber;
            msg.Body.PayStatus = eventMessage.PayStatus;
            msg.Body.InvoiceNumber = eventMessage.InvoiceNumber;
            msg.Body.InvoiceStatus = eventMessage.InvoiceStatus;
            msg.Body.ReceiveAmount = eventMessage.ReceiveAmount;
            msg.Body.ReceiveDate = eventMessage.ReceiveDate;
            msg.Body.PostUser = eventMessage.PostUser;
            result = service.PostPayItem(msg);
            if (!string.IsNullOrEmpty(result.Error))
            {
                throw new BizException(result.Error);
            }
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
