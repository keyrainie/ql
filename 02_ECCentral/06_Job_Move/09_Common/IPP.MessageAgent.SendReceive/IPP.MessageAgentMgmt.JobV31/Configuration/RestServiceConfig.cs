using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace IPP.MessageAgent.SendReceive.JobV31.Configuration
{
    [XmlRoot("restServiceConfig", Namespace = "http://IPP/RestService")]
    public class RestServiceConfig
    {
        [XmlElement("restService")]
        public RestServiceCollection RestServices { get; set; }
    }

    public class RestService
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("baseUrl")]
        public string BaseUrl { get; set; }

        [XmlAttribute("relativeUrl")]
        public string RelativeUrl { get; set; }
    }

    public class RestServiceCollection : KeyedCollection<string, RestService>
    {
        protected override string GetKeyForItem(RestService item)
        {
            return item.Name;
        }
    }
}
