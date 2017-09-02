using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECommerce.Enums
{
    /// <summary>
    /// 团购状态
    /// </summary>
    public enum GroupBuyingStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        [EnumMember]
        [XmlEnum("O")]
        CheckPending = 0,
        /// <summary>
        /// 就绪
        /// </summary>
        [Description("就绪")]
        [EnumMember]
        [XmlEnum("P")]
        Ready = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        [EnumMember]
        [XmlEnum("A")]
        Running = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        [EnumMember]
        [XmlEnum("F")]
        Finish = 3,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        [EnumMember]
        [XmlEnum("D")]
        Voided = -1
    }

    /// <summary>
    /// 虚拟团购团购券状态
    /// </summary>
    public enum GroupBuyingTicketStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        [Description("未使用")]
        [EnumMember]
        UnUse = 0,
        /// <summary>
        /// 已使用
        /// </summary>
        [Description("已使用")]
        [EnumMember]
        Used = 1,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        [EnumMember]
        Abandon = -1,
        /// <summary>
        /// 已过期
        /// </summary>
        [Description("已过期")]
        [EnumMember]
        Expired = -2,
        /// <summary>
        /// 已创建(未付款)
        /// </summary>
        [Description("已创建(未付款)")]
        [EnumMember]
        Created = -3
    }
}
