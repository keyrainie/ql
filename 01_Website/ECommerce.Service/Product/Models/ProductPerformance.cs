using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Facade.Product.Models
{
    public class LongDescription
    {
         [XmlElement("Group")]
        public List<Group> Groups { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class Group
    {

        [XmlElement("Property")]
        public List<Property> Propertys { get; set; }
        [XmlAttribute]
        public string GroupName { get; set; }
    }

    public class Property
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }
}
