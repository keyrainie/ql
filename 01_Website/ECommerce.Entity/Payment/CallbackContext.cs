using System;
using System.Runtime.Serialization;
using System.Collections.Specialized;

using ECommerce.Entity.Order;
using ECommerce.Enums;

namespace ECommerce.Entity.Payment
{
    /// <summary>
    /// 支付回调上下文
    /// </summary>
    [Serializable]
    [DataContract]
    public class CallbackContext
    {
        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public int PaymentModeId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public int SOSysNo { get; set; }

        /// <summary>
        /// 订单信息
        /// </summary>
        [DataMember]
        public PayOrderInfo SOInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EPaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// 相应参数
        /// </summary>
        [DataMember]
        public NameValueCollection ResponseForm { get; set; }

        /// <summary>
        /// 支付方式配置
        /// </summary>
        [DataMember]
        public PaymentSetting PaymentInfo { get; set; }

        /// <summary>
        /// 支付方式商家配置
        /// </summary>
        [DataMember]
        public PaymentModeMerchant PaymentInfoMerchant
        {
            get
            {
                PaymentModeMerchant merchant = null;
                if (this.PaymentInfo != null && this.PaymentInfo.PaymentMode != null
                    && this.PaymentInfo.PaymentMode.MerchantList != null
                    && this.SOInfo != null && this.SOInfo.SellerSysNo > 0)
                    merchant = this.PaymentInfo.PaymentMode.MerchantList.Find(m => m.MerchantSysNo == this.SOInfo.SellerSysNo);

                return merchant;
            }
        }
    }
}
