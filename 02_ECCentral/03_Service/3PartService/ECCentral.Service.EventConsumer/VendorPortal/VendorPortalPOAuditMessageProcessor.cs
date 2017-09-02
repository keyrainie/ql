using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer.VendorPortal
{
    public class VendorPortalPOAuditMessageProcessor : IConsumer<VendorPortalPOAuditMessage>
    {
        #region IConsumer<VendorPortalPOAuditMessage> Members

        public void HandleEvent(VendorPortalPOAuditMessage eventMessage)
        {
            eventMessage.MsgType = AppSettingManager.GetSetting("PO", "VendorPortal_SSB_Header_MsgType");
            new VendorPortalSSBSender(eventMessage).Send();
        }

        #endregion IConsumer<VendorPortalPOAuditMessage> Members


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}