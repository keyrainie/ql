using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECommerce.Enums
{
    public enum CustomerMark
    {
        [Description("没有标记")]
        [EnumMember]
        [XmlEnum("0")]
        NoMark = 0,

        [Description("大使未激活")]
        [EnumMember]
        [XmlEnum("1")]
        AmbassadorUnactive = 1,

        [Description("大使已激活")]
        [EnumMember]
        [XmlEnum("2")]
        AmbassadorActive = 2,
        [Description("大使已取消")]
        [EnumMember]
        [XmlEnum("3")]
        AmbassadorCancel = 3
    }
}