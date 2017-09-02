using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.BizEntity.RMA
{
    [Serializable]
    [XmlRoot("Publish", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false)]
    public class SSBV3Base
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
}