using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECommerce.Enums
{
    public enum AvtarImageStatus
    {
        /// <summary>
        /// 显示
        /// </summary>
        [EnumMember]
        [XmlEnum("A")]
        A,//Active,

        /// <summary>
        /// 不显示 
        /// </summary>
        [EnumMember]
        [XmlEnum("D")]
        D//Deactive,
    }
}