using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventConsumer.VendorPortal
{
    public class VendorPortalPORetreatMessageProcessor : IConsumer<VendorPortalPORetreatMessage>
    {
        #region IConsumer<VendorPortalPORetreatMessage> Members

        public void HandleEvent(VendorPortalPORetreatMessage eventMessage)
        {
            eventMessage.MsgType = AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_MsgType");
            new VendorPortalSSBSender(eventMessage).Send();
        }

        #endregion IConsumer<VendorPortalPORetreatMessage> Members


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

}