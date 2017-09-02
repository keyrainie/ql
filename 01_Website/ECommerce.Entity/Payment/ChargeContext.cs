using System;
using System.Runtime.Serialization;
using System.Collections.Specialized;

using ECommerce.Entity.Order;
using System.Collections.Generic;

namespace ECommerce.Entity.Payment
{
    /// <summary>
    /// 支付上下文
    /// </summary>
    [Serializable]
    [DataContract]
    public class ChargeContext
    {
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        [DataMember]
        public int PaymentModeId { get; set; }

        /// <summary>
        /// 订单信息
        /// </summary>
        [DataMember]
        public PayOrderInfo SOInfo { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [DataMember]
        public NameValueCollection RequestForm { get; set; }

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
                //微信支付的话第三方商家同样使用TLYH的商户账号
                if (merchant == null && PaymentInfo.PaymentMode.Id == "112")
                {
                    merchant = this.PaymentInfo.PaymentMode.MerchantList.Find(m => m.MerchantSysNo == 1);
                }

                return merchant;
            }
        }
    }

    /// <summary>
    /// 支付方式配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaymentSetting
    {
        /// <summary>
        /// 基础公共配置
        /// </summary>
        [DataMember]
        public PaymentBase PaymentBase { get; set; }

        /// <summary>
        /// 支付方式的具体配置
        /// </summary>
        [DataMember]
        public PaymentMode PaymentMode { get; set; }
    }

    /// <summary>
    /// 基础公共配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaymentBase
    {
        /// <summary>
        /// 基地址
        /// </summary>
        [DataMember]
        public string BaseUrl { get; set; }
    }

    /// <summary>
    /// 支付方式的具体配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaymentMode
    {
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [DataMember]
        public string RequestType { get; set; }

        /// <summary>
        /// 处理程序
        /// </summary>
        [DataMember]
        public string ChargeProcessor { get; set; }

        /// <summary>
        /// 支付地址
        /// </summary>
        [DataMember]
        public string PaymentUrl { get; set; }

        /// <summary>
        /// 支付前台回调地址
        /// </summary>
        [DataMember]
        public string PaymentCallbackUrl { get; set; }

        /// <summary>
        /// 支付后台回调地址
        /// </summary>
        [DataMember]
        public string PaymentBgCallbackUrl { get; set; }

        /// <summary>
        /// 退款地址
        /// </summary>
        [DataMember]
        public string RefundUrl { get; set; }

        /// <summary>
        /// 退款回调地址
        /// </summary>
        [DataMember]
        public string RefundCallbackUrl { get; set; }

        /// <summary>
        /// 银行公钥证书 / 商户号
        /// </summary>
        [DataMember]
        public string BankCert { get; set; }

        /// 银行公钥证书密码 / 文件路径
        /// </summary>
        [DataMember]
        public string BankCertKey { get; set; }

        /// <summary>
        /// 每个接口自定义配置
        /// </summary>
        [DataMember]
        public Dictionary<string, string> CustomConfigs { get; set; }

        [DataMember]
        public List<PaymentModeMerchant> MerchantList { get; set; }

        /// <summary>
        /// 是否debug，0-否；1-是
        /// </summary>
        [DataMember]
        public string Debug { get; set; }
    }

    /// <summary>
    /// 商家配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaymentModeMerchant
    {
        /// <summary>
        /// 商户的系统编号
        /// </summary>
        [DataMember]
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [DataMember]
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [DataMember]
        public string MerchantNO { get; set; }

        /// <summary>
        /// 商户密钥证书
        /// </summary>
        [DataMember]
        public string MerchantCert { get; set; }

        /// <summary>
        /// 商户密钥证书密码
        /// </summary>
        [DataMember]
        public string MerchantCertKey { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        [DataMember]
        public string CurCode { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [DataMember]
        public string Encoding { get; set; }

        /// <summary>
        /// 每个接口自定义配置
        /// </summary>
        [DataMember]
        public Dictionary<string, string> CustomConfigs { get; set; }
        [DataMember]
        public string CBTSRC_NCode { get; set; }
        [DataMember]
        public string CBTREC_NCode { get; set; }
        [DataMember]
        public string PayCurrencyCode { get; set; }
    }
}
