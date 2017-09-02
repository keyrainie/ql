using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECommerce.Enums
{
    public enum CustomerContributionType
    {
        /// <summary>
        /// 无贡献
        /// </summary>
        [Description("")]
        [EnumMember]
        [XmlEnum("N")]
        None = 0,

        /// <summary>
        /// 助教(T)
        /// </summary>
        [Description("助教")]
        [EnumMember]
        [XmlEnum("T")]
        Assistant = 1,

        /// <summary>
        /// 讲师(L)
        /// </summary>
        [Description("讲师")]
        [EnumMember]
        [XmlEnum("L")]
        Docent = 2,

        /// <summary>
        /// 副教授(A)
        /// </summary>
        [Description("副教授")]
        [EnumMember]
        [XmlEnum("A")]
        AdjunctProfessor = 3,

        /// <summary>
        /// 教授(P)
        /// </summary>
        [Description("教授")]
        [EnumMember]
        [XmlEnum("P")]
        Professor = 4,

    }
}