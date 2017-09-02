using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.VendorPortal
{
    [Serializable]
    [XmlRoot("InvoiceChangeStatusSSBEntity")]
    public class VendorPortalInvoiceChangeStatusMessage : VendorPortalMessageBodyBase, IEventMessage
    {
        // Properties
        [XmlElement("EditUser")]
        public string EditUser
        {
            get;
            set;
        }
        [XmlElement("Note")]
        public string Note
        {
            get;
            set;
        }
        [XmlElement("Status")]
        public string Status
        {
            get;
            set;
        }
        [XmlElement("SysNo")]
        public int? SysNo
        {
            get;
            set;
        }

        public string Subject
        {
            get { return "VendorPortalInvoiceChangeStatusMessage"; }
        }
    }
}