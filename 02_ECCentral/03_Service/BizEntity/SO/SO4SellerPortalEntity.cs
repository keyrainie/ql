using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.SO
{
    #region Base Entity

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SSBEntityBaseV3
    {
        [XmlElement("FromService")]
        public string FromService { get; set; }

        [XmlElement("ToService")]
        public string ToService { get; set; }

        [XmlElement("RouteTable")]
        public RouteTable RouteTable { get; set; }

    }

    public class RouteTable
    {
        [XmlElement("Article", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
        public Article Article { get; set; }
    }

    public class Article
    {
        [XmlElement("ArticleCategory")]
        public string ArticleCategory { get; set; }

        [XmlElement("ArticleType1")]
        public string ArticleType1 { get; set; }

        [XmlElement("ArticleType2")]
        public string ArticleType2 { get; set; }
    }

    #endregion

    #region SO Shipping Out

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SOShippedEntity : SSBEntityBaseV3
    {
        [XmlElement("Node")]
        public SOShippedNote Node { get; set; }
    }

    public class SOShippedNote
    {
        [XmlElement("RequestRoot")]
        public SOShippedRequestRoot RequestRoot { get; set; }
    }

    public class SOShippedRequestRoot
    {
        [XmlElement("MessageHeader")]
        public SOShippedMessageHead MessageHead { get; set; }

        [XmlElement("Body")]
        public SOShippedBody Body { get; set; }
    }

    public class SOShippedMessageHead
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

    public class SOShippedBody
    {
        [XmlElement("ShipOrderMsg")]
        public SOShippedShipOrderMsg ShipOrderMsg { get; set; }
    }

    public class SOShippedShipOrderMsg
    {
        [XmlElement("InUser")]
        public string InUser { get; set; }

        [XmlElement("SalesOrder")]
        public List<SalesOrderInfo> SalesOrder { get; set; }
    }

    public class SalesOrderInfo
    {
        [XmlElement("SONumber")]
        public int SONumber { get; set; }

        [XmlElement("METShipViaCode")]
        public string METShipViaCode { get; set; }

        [XmlElement("METPackageNumber")]
        public string METPackageNumber { get; set; }
        [XmlIgnore]
        public string InUser { get; set; }
        [XmlIgnore]
        public string CompanyCode { get; set; }
    }

    #endregion

    #region SO Status Change

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SOReceiveStatusEntity : SSBEntityBaseV3
    {
        [XmlElement("Node")]
        public SOReceiveStatusNote Node { get; set; }
    }

    public class SOReceiveStatusNote
    {
        [XmlElement("RequestRoot")]
        public SOReceiveStatusRequestRoot RequestRoot { get; set; }

    }

    public class SOReceiveStatusRequestRoot
    {
        [XmlElement("MessageHeader")]
        public SOReceiveStatusHeader MessageHeader;

        [XmlElement("Body")]
        public SOReceiveStatusBody Body;
    }

    public class SOReceiveStatusHeader
    {
        [XmlElement("ApplicationName")]
        public string ApplicationName;

        [XmlElement("Version")]
        public string Version;

        [XmlElement("MessageType")]
        public string MessageType;

        [XmlElement("CompanyCode")]
        public string CompanyCode;

        [XmlElement("ReferenceNumber")]
        public string ReferenceNumber;
    }

    public class SOReceiveStatusBody
    {
        [XmlElement("ChanageSOStatusMsg")]
        public ChanageSOStatusMsg Msg { get; set; }
    }

    public class ChanageSOStatusMsg
    {
        [XmlElement("InUser")]
        public string InUser { get; set; }

        [XmlElement("SalesOrder")]
        public List<SalesOrderStatusEntity> SalesOrderStatusList { get; set; }
    }

    public class SalesOrderStatusEntity
    {
        [XmlElement("SONumber")]
        public int SOSysNo { get; set; }

        [XmlElement("SOStatus")]
        public int? SOStatus { get; set; }

        [XmlElement("ReceivingStatus")]
        public string ReceivingStatus { get; set; }
    }

    #endregion

    #region SO Invoice

    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SOInvoicePrintedInfoEnity : SSBEntityBaseV3
    {
        [XmlElement("Node")]
        public SOInvoicePrintedNote Node { get; set; }

    }

    public class SOInvoicePrintedNote
    {
        [XmlElement("RequestRoot")]
        public SOInvoicePrintedRequestRoot RequestRoot { get; set; }

    }

    public class SOInvoicePrintedRequestRoot
    {
        [XmlElement("MessageHeader")]
        public SOInvoicePrintedHeader MessageHeader;

        [XmlElement("Body")]
        public SOInvoicePrintedBody Body;
    }

    public class SOInvoicePrintedHeader
    {
        [XmlElement("Language")]
        public string Language;

        [XmlElement("Sender")]
        public string Sender;

        [XmlElement("CompanyCode")]
        public string CompanyCode;

        [XmlElement("Action")]
        public string Action;

        [XmlElement("Version")]
        public string Version;

        [XmlElement("Type")]
        public string Type;

        [XmlElement("OriginalGUID")]
        public string OriginalGUID;

    }

    public class SOInvoicePrintedBody
    {
        [XmlElement("SalesOrder")]
        public SOInvoicePrintedSalesOrder SalesOrder { get; set; }
    }

    public class SOInvoicePrintedSalesOrder
    {
        [XmlAttribute("SONumber")]
        public int SONumber { get; set; }

        [XmlAttribute("InvoiceNumber")]
        public string InvoiceNumber { get; set; }
    }

    #endregion
}
