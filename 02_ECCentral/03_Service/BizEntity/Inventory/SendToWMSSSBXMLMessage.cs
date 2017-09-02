using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.Inventory
{
    [XmlRoot("Node", Namespace = null, IsNullable = false)]
    public class SendToWMSSSBXMLMessage
    {
        [XmlElement("RequestRoot")]
        public RequestRoot RequestRoot { get; set; }
    }
    public class RequestRoot
    {
        [XmlElement("MessageHeader")]
        public MessageHeader MessageHeader { get; set; }
        [XmlElement("Body")]
        public Body Body { get; set; }
    }
    public class MessageHeader
    {
        [XmlElement("Language")]
        public string Language { get; set; }
        [XmlElement("Sender")]
        public string Sender { get; set; }
        [XmlElement("CompanyCode")]
        public string CompanyCode { get; set; }
        [XmlElement("Action")]
        public string Action { get; set; }
        [XmlElement("Version")]
        public string Version { get; set; }
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlElement("OriginalGUID")]
        public string OriginalGUID { get; set; }
    }
    public class Body
    {
        [XmlElement("Operation")]
        public Operation Operation { get; set; }
    }
    public class Operation
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
        [XmlAttribute("Number")]
        public string Number { get; set; }
        [XmlAttribute("User")]
        public string User { get; set; }
        [XmlAttribute("Memo")]
        public string Memo { get; set; }
        [XmlAttribute("WarehouseNumber")]
        public string WarehouseNumber { get; set; }
        [XmlElement("Item")]
        public List<Item> Item { get; set; }
    }
    public class Item
    {
        [XmlAttribute("ProductSysNo")]
        public string ProductSysNo { get; set; }
        [XmlAttribute("Quantity")]
        public string Quantity { get; set; }

        [XmlElement("ProductBatch")]
        public ProductBatch ProductBatch { get; set; }
    }
    public class ProductBatch
    {
        [XmlAttribute("BatchNumber")]
        public string BatchNumber { get; set; }
        [XmlAttribute("Quantity")]
        public string Quantity { get; set; }
    }
}
