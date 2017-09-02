using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.VendorPortal
{
    [Serializable]
    [XmlRoot("MessageHeader")]
    public class VendorPortalMessageHeader
    {
        public VendorPortalMessageHeader()
        {
            Action = VendorPortalActionType.Create.ToString();
        }
        [XmlElement("Language")]
        public string Language
        {
            get;
            set;
        }
        [XmlElement("Sender")]
        public string Sender
        {
            get;
            set;
        }
        [XmlElement("CompanyCode")]
        public string CompanyCode
        {
            get;
            set;
        }
        [XmlElement("Action")]
        public string Action
        {
            get;
            set;
        }

        [XmlElement("Namespace")]
        public string Namespace
        {
            get;
            set;
        }
        [XmlElement("Version")]
        public string Version
        {
            get;
            set;
        }
        [XmlElement("Type")]
        public string Type
        {
            get;
            set;
        }
        [XmlElement("OriginalGUID")]
        public string OriginalGUID
        {
            get;
            set;
        }
        [XmlElement("Comment")]
        public string Comment
        {
            get;
            set;
        }
        [XmlElement("Tag")]
        public string Tag
        {
            get;
            set;
        }
    }

    public enum VendorPortalActionType
    {
        Create,
        Void,
        Dispatch,
        Hold,
        Update,
        Batch
    }
}