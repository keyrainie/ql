using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.WMS
{
    [Serializable]
    public class PurchaseOrderCloseMessage : IEventMessage
    {
        [XmlElement("PONumber")]
        public string PONumber { get; set; }
        [XmlElement("Memo")]
        public string Memo { get; set; }
        [XmlIgnore]
        public string CompanyCode { get; set; }

        public string Subject
        {
            get { return "PurchaseOrderCloseMessage"; }
        }
    }

}
