using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 礼品卡信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class GiftCardInfo : IIdentity, ICompany, IWebChannel
    {
        /// <summary>
        /// 卡号
        /// </summary>
        [DataMember]
        public string CardCode { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// 条形码？
        /// </summary>
        [DataMember]
        public string BarCode { get; set; }

        /// <summary>
        /// 面值
        /// </summary>
        [DataMember]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [DataMember]
        public decimal AvailAmount { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [DataMember]
        public CustomerInfo Customer { get; set; }

        /// <summary>
        /// 绑定客户
        /// </summary>
        [DataMember]
        public CustomerInfo BindingCustomer { get; set; }

        /// <summary>
        /// 有效日期开始
        /// </summary>
        [DataMember]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 有效日期结束
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 礼品卡类型
        /// </summary>
        [DataMember]
        public GiftCardType CardType { get; set; }

        /// <summary>
        /// 礼品卡状态
        /// </summary>
        [DataMember]
        public GiftCardStatus Status { get; set; }

        /// <summary>
        /// 购买订单号
        /// </summary>
        [DataMember]
        public SOInfo ReferenceSO { get; set; }

        /// <summary>
        /// ?需要确认
        /// </summary>
        [DataMember]
        public string ReferenceID { get; set; }

        /// <summary>
        /// bober add 
        /// </summary>
        [DataMember]
        public ECCentral.BizEntity.MKT.YNStatus IsHold { get; set; }

        [DataMember]
        public DateTime? PreEndDate { get; set; }

        [DataMember]
        public DateTime? InDate { get; set; }

        [DataMember]
        public UserInfo InUser {get;set;}

        [DataMember]
        public DateTime? EditDate { get; set; }

        [DataMember]
        public UserInfo EditUser { get; set; }

        [DataMember]
        public int? SysNo { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public WebChannel WebChannel { get; set; }

        [DataMember]
        public string CurrencyCode { get; set; }

    }
}
