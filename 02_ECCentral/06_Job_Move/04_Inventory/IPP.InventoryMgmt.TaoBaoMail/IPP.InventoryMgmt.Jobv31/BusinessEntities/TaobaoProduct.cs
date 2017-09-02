using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IPP.InventoryMgmt.JobV31.BusinessEntities
{
    [XmlRoot("items_inventory_get_response",Namespace="")]
    public class TaobaoResponse
    {
        private bool _IsList = true;
        [XmlElement("items")]
        public TaobaoProductCollection Response { get; set; }

        [XmlAttribute("list")]
        public bool IsList { get { return _IsList; } set { _IsList = value; } }

        [XmlElement("total_results")]
        public int Records { get; set; }
    }

    public class TaobaoProductCollection
    {
        [XmlElement("item")]
        public List<TaobaoProduct> ProductCollection { get; set; }
    }
    
    public class TaobaoProduct
    {
        [XmlElement("num_iid")]
        public string NumberID { get; set; }

        [XmlElement("outer_id")]
        public string ProductID { get; set; }

        [XmlElement("num")]
        public int Qty { get; set; }

        public string Status { get; set; }
    }
}
