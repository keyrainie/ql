using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer.VendorPortal
{
    public class VendorPortalInvoiceChangeStatusMessageProcessor : IConsumer<VendorPortalInvoiceChangeStatusMessage>
    {
        #region IConsumer<VendorPortalInvoiceChangeStatusMessage> Members

        public void HandleEvent(VendorPortalInvoiceChangeStatusMessage eventMessage)
        {
            new VendorPortalSSBSender(eventMessage).Send();
        }

        #endregion IConsumer<VendorPortalInvoiceChangeStatusMessage> Members


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}