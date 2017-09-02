using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventConsumer.VendorPortal
{
    public class VendorPortalPOConfirmMessageProcessor : IConsumer<VendorPortalPOConfirmMessage>
    {
        #region IConsumer<VendorPortalPOConfirmMessage> Members

        public void HandleEvent(VendorPortalPOConfirmMessage eventMessage)
        {
            eventMessage.MsgType = AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_MsgType");
            new VendorPortalSSBSender(eventMessage).Send();
        }
        #endregion IConsumer<VendorPortalPOConfirmMessage> Members


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

}