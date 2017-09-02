using System.Xml.Serialization;

namespace ECCentral.Service.Utility
{
    public class MessageEntity
    {
        [XmlAttribute("name")]
        public string KeyName;

        [XmlText]
        public string Text;
    }
}
