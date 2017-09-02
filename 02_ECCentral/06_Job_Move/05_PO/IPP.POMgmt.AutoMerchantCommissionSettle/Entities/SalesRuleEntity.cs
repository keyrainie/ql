using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MerchantCommissionSettle.Entities
{
    [XmlRoot("SalesRuleEntity")]
    [Serializable]
    public class SalesRuleEntity
    {
        public class SaleRuleItem
        {
            public decimal StartAmt { get; set; }

            public decimal? EndAmt { get; set; }

            public float Percentage { get; set; }

            public int Order { get; set; }
        }

        [XmlArray("Rules")]
        [XmlArrayItem("Rule")]
        public List<SaleRuleItem> Rules { get; set; }

        [XmlElement("MinCommissionAmt")]
        public Decimal MinCommissionAmt { get; set; }
    }
}
