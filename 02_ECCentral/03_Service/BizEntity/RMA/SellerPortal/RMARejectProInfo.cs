using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.RMA
{
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class RMARejectProInfo : SSBV3Base
    {
        [XmlElement("Node")]
        public RMARejectRequest Node { get; set; }
    }

    public class RMARejectRequest
    {
        [XmlElement("RequestRoot")]
        public RMAReject RMARejectRequestRoot { get; set; }
    }

    public class RMAReject
    {
        [XmlElement("MessageHeader")]
        public RMARejectMessageHeader MessageHeader { get; set; }

        [XmlElement("Body")]
        public RMARejectBody Body { get; set; }
    }

    public class SalesOrder
    {
        [XmlElement("SONumber")]
        public int SONumber
        {
            get;
            set;
        }
    }

    public class RMARejectMsg
    {
        [XmlElement("InUser")]
        public string InUser { get; set; }

        [XmlElement("SalesOrder")]
        public List<SalesOrder> OrdersList
        {
            get;
            set;
        }
    }

    public class RMARejectMessageHeader
    {
        [XmlElement("Language")]
        public string Language { get; set; }

        [XmlElement("Sender")]
        public string Sender { get; set; }

        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }

        [XmlElement("Action")]
        public string Action { get; set; }

        [XmlElement("Namespace")]
        public string Namespace { get; set; }

        [XmlElement("Version")]
        public string Version { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("OriginalGUID")]
        public string OriginalGUID { get; set; }
    }

    public class RMARejectBody
    {
        [XmlElement("RMARejectMsg")]
        public RMARejectMsg RMARejectMsg { get; set; }
    }
}
