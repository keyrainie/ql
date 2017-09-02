using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.EIMS;
using ECCentral.Service.Utility;
using Newegg.EIMSCN.ServiceInterfaces.DataContracts;
using Newegg.EIMSCN.ServiceInterfaces.ServiceContracts;

namespace ECCentral.Service.EventConsumer.EMIS
{
    public class EIMSInvoiceInfoForConsignProcessor : IConsumer<EIMSInvoiceInfoForConsignMessage>
    {
        public void HandleEvent(EIMSInvoiceInfoForConsignMessage eventMessage)
        {
            IEIMSInterfaceService service = WCFAdapter<IEIMSInterfaceService>.GetProxy();
            EIMSInvoiceInfo result = service.GetEIMSInvoiceInfoByInvoiceNumberAndReceiveType(eventMessage.InvoiceNumber, eventMessage.CompanyCode, eventMessage.ReceiveType);
            if (result != null)
            {
                eventMessage.IsError = false;
                eventMessage.PM = result.PM;
                eventMessage.RemnantReturnPoint = result.CurrentAmount;
                eventMessage.SysNo = result.InvoiceNumber;
                eventMessage.ReturnPointName = result.InvoiceName;
                eventMessage.VendorSysNo = result.VendorNumber;
            }
            else
            {
                eventMessage.IsError = true;
            }
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
