using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.VendorPortal
{
    public class VendorPortalMessageBodyBase
    {
        [XmlIgnore]
        public VendorPortalActionType Action
        {
            get;
            set;
        }

        [XmlIgnore]
        public string Comment
        {
            get;
            set;
        }

        [XmlIgnore]
        public string Tag
        {
            get;
            set;
        }

        [XmlIgnore]
        public string MsgType
        {
            get;
            set;
        }
    }
}