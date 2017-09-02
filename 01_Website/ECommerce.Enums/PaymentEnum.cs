using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ECommerce.Enums
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum NetPayType
    {
        [Description("银行在线支付")]
        [EnumMember]
        [XmlEnum("1")]
        BankPayType = 1,

        [Description("支付平台支付")]
        [EnumMember]
        [XmlEnum("2")]
        PlatformPayType = 2,


        [Description("存储卡支付")]
        [EnumMember]
        [XmlEnum("3")]
        DebitCartPayType = 3,

        //[Description("礼品券支付")]
        //[EnumMember]
        //[XmlEnum("4")]
        //GiftCartPayType = 4
    }

    /// <summary>
    /// 支付类别
    /// </summary>
    public enum PaymentCategory
    {
        /// <summary>
        /// 在线支付
        /// </summary>
        [Description("在线支付")]
        OnlinePay = 1,
        /// <summary>
        /// 货到付款
        /// </summary>
        [Description("货到付款")]
        PayWhenRecv = 2
    }
}
