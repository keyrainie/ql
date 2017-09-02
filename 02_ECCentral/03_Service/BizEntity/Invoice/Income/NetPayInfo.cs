using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 网上支付付款信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class NetPayInfo : IIdentity, ICompany
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
        /// 网上支付来源
        /// </summary>
        [DataMember]
        public NetPaySource? Source
        {
            get;
            set;
        }

        /// <summary>
        /// 审批人用户系统编号
        /// </summary>
        [DataMember]
        public int? ApproveUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审批时间
        /// </summary>
        [DataMember]
        public DateTime? ApproveTime
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
        /// 网上支付状态
        /// </summary>
        [DataMember]
        public NetPayStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 复查人用户系统编号
        /// </summary>
        [DataMember]
        public int? ReviewedUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 复查时间
        /// </summary>
        [DataMember]
        public DateTime? ReviewedTime
        {
            get;
            set;
        }

        /// <summary>
        /// CS确认的订单号
        /// </summary>
        [DataMember]
        public int? RelatedSoSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 主订单号
        /// </summary>
        [DataMember]
        public int? MasterSoSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 外部引用键
        /// </summary>
        [DataMember]
        public string ExternalKey
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
        public decimal? PrePayAmt
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
        /// 支付的礼品卡金额
        /// </summary>
        [DataMember]
        public decimal? GiftCardPayAmt
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

        [DataMember]
        public int? InputUserSysNo
        {
            get;
            set;

        }

        #endregion ICompany Members
    }
}