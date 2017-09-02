using System;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Shopping
{
    /// <summary>
    /// thankyou页面订单简单信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ThankyouOrderInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public int SOSysNo { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal SOAmount { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        [DataMember]
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        [DataMember]
        public int PayTypeSysNo { get; set; }
        /// <summary>
        /// 支付方式编号
        /// </summary>
        [DataMember]
        public string PayTypeID { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public string PayTypeName { get; set; }
        /// <summary>
        /// 是否是在线支付，0-否；1-是
        /// </summary>
        [DataMember]
        public int IsNet { get; set; }
        /// <summary>
        /// 订单温馨提示
        /// </summary>
        [DataMember]
        public string MemoForCustomer {get;set;}

        /// <summary>
        /// Gets or sets the ship type system no.
        /// </summary>
        /// <value>
        /// The ship type system no.
        /// </value>
        public int ShipTypeSysNo { get; set; }

        /// <summary>
        /// Gets or sets the name of the ship type.
        /// </summary>
        /// <value>
        /// The name of the ship type.
        /// </value>
        public string ShipTypeName { get; set; }
    }
}
