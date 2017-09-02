using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 邮局、电汇付款信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PostPayInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式编号
        /// </summary>
        [DataMember]
        public int? PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        [DataMember]
        public decimal? PayAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public PostPayStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// CS确认的订单号
        /// </summary>
        [DataMember]
        public int? ConfirmedSOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单剩余金额
        /// </summary>
        [DataMember]
        public decimal? RemainAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 订单拆分时的订单主单号
        /// </summary>
        [DataMember]
        public int? MasterSoSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal? OrderAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预付款金额
        /// </summary>
        [DataMember]
        public decimal? PrepayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 支付的积分
        /// </summary>
        [DataMember]
        public decimal? PointPay
        {
            get;
            set;
        }

        /// <summary>
        /// 支付的礼品卡
        /// </summary>
        [DataMember]
        public decimal? GiftCardPay
        {
            get;
            set;
        }

        /// <summary>
        /// 销售渠道编号（冗余）
        /// </summary>
        [DataMember]
        public string ChannelID
        {
            get;
            set;
        }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>
        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members
    }
}