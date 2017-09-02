using System;
using ECommerce.Enums;

namespace ECommerce.Entity.Payment
{
    [Serializable]
    public class PayTypeInfo
    {
        /// <summary>
        /// SysNo
        /// </summary>
        public int PayTypeID { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public string PayTypeCode { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string PayTypeName { get; set; }

        /// <summary>
        /// 支付方式描述
        /// </summary>
        public string PayTypeDesc { get; set; }

        /// <summary>
        /// 支付方式到账期限
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// 支付方式页面
        /// </summary>
        public string PaymentPage { get; set; }

        /// <summary>
        /// 支付方式利率
        /// </summary>
        public decimal PayRate { get; set; }

        /// <summary>
        /// 是否通过网络支付
        /// </summary>
        public int IsNet { get; set; }

        /// <summary>
        /// IsPayWhenRecv
        /// </summary>
        public int IsPayWhenRecv { get; set; }

        /// <summary>
        /// 是否显示在前台
        /// </summary>
        public int IsOnlineShow { get; set; }

        /// <summary>
        /// 在线支付类型
        /// </summary>
        public NetPayType NetPayType { get; set; }
    }
}