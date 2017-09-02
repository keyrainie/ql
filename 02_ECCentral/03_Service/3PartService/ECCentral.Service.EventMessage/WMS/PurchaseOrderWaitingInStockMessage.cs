using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.WMS
{

    [Serializable]
    public class PurchaseOrderWaitingInStockMessage : IEventMessage
    {
        [XmlIgnore]
        public string SendType { get; set; }
        [XmlAttribute("PONumber")]
        public string PONumber { get; set; }
        [XmlAttribute("CompanyCode")]
        public string CompanyCode { get; set; }
        [XmlAttribute("VendorSysNo")]
        public string VendorSysNo { get; set; }
        [XmlAttribute("WarehouseNumber")]
        public string WarehouseNumber { get; set; }
        [XmlAttribute("POType")]
        public string POType { get; set; }
        [XmlAttribute("AccessoriesInfo")]
        public string AccessoriesInfo { get; set; }
        [XmlAttribute("Memo")]
        public string Memo { get; set; }
        [XmlAttribute("PMContact")]
        public string PMContact { get; set; }
        [XmlAttribute("ETATime")]
        public string ETATime { get; set; }
        [XmlAttribute("ETAHalfDay")]
        public string ETAHalfDay { get; set; }

        [XmlElement("POItem")]
        public List<POItemInfoMessage> POItems { get; set; }


        public string Subject
        {
            get { return "PurchaseOrderWaitingInStockMessage"; }
        }
    }
    [Serializable]
    public class POItemInfoMessage
    {
        [XmlAttribute("ProductSysNo")]
        public string ProductSysNo { get; set; }
        [XmlAttribute("ItemNumber")]
        public string ItemNumber { get; set; }
        [XmlAttribute("ProductName")]
        public string ProductName { get; set; }
        [XmlAttribute("PurchaseQty")]
        public string PurchaseQty { get; set; }
        [XmlAttribute("Price")]
        public string Price { get; set; }
        [XmlAttribute("Weight")]
        public string Weight { get; set; }
        [XmlAttribute("BatchInfo")]
        public string BatchInfo { get; set; }
    }
}
