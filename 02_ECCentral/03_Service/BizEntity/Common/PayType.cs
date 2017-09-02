using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 支付方式信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PayType : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 支付方式编号
        /// </summary>
        [DataMember]
        public string PayTypeID { get; set; }

        /// <summary>
        /// 支付方式描述
        /// </summary>
        [DataMember]
        public string PayTypeName { get; set; }

        /// <summary>
        /// 是否在线支付
        /// </summary>
        [DataMember]
        public bool? IsNet { get; set; }
        /// <summary>
        /// 是否在线支付的转换属性 对应 IsNet
        /// </summary>
        [DataMember]
        public SYNStatus? IsNetEnum
        {
            get
            {
                return (IsNet.HasValue && IsNet.Value) ? SYNStatus.Yes : SYNStatus.No;
            }
            set
            {
                IsNet = value.HasValue ? (value.Equals(SYNStatus.Yes) ? true : false) : (bool?)null;
            }
        }

        /// <summary>
        /// 是否货到付款
        /// </summary>
        [DataMember]
        public bool? IsPayWhenRecv { get; set; }
        /// <summary>
        /// 是否货到付款的转换属性 对应 IsPayWhenRecv
        /// </summary>
        [DataMember]
        public SYNStatus? IsPayWhenRecvEnum
        {
            get
            {
                return (IsPayWhenRecv.HasValue && IsPayWhenRecv.Value) ? SYNStatus.Yes : SYNStatus.No;
            }
            set
            {              
                IsPayWhenRecv = value.HasValue ? (value.Equals(SYNStatus.Yes) ? true : false) : (bool?)null;
            }
        }

        /// <summary>
        /// 手续费率
        /// </summary>
        [DataMember]
        public decimal? PayRate { get; set; }
        
        /// <summary>
        /// 支付方式描述
        /// </summary>
        [DataMember]
        public string PayTypeDesc { get; set; }

        /// <summary>
        /// 到账周期
        /// </summary>
        [DataMember]
        public string Period { get; set; }

        /// <summary>
        /// 支付页面
        /// </summary>
        [DataMember]
        public string PaymentPage { get; set; }

        /// <summary>
        /// 是否前台显示
        /// </summary>
        [DataMember]
        public HYNStatus? IsOnlineShow { get; set; }

        /// <summary>
        /// 显示优先级
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 网上支付类型
        /// </summary>
        [DataMember]
        public NetPayType? NetPayType { get; set; }

        /// <summary>
        /// 公司编码 默认8601
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// CBT(上海跨境电商商务进口服务平台)中的商户号，也用于东方支付的商户节点号。
        /// </summary>
        [DataMember]
        public string CBTMerchantCode { get; set; }

        /// <summary>
        /// CBT中的商户名称
        /// </summary>
        [DataMember]
        public string CBTMerchantName { get; set; }

        /// <summary>
        /// 订单申报支付接口主体验签密钥
        /// </summary>
        [DataMember]
        public string CBTSODeclarePaymentSecretKey { get; set; }
    }
}