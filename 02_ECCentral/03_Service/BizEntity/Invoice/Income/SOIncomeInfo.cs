using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 销售收款单
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOIncomeInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 单据类型
        /// </summary>
        [DataMember]
        public SOIncomeOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 收款类型
        /// </summary>
        [DataMember]
        public SOIncomeOrderStyle? IncomeStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单状态
        /// </summary>
        [DataMember]
        public SOIncomeStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        [DataMember]
        public int? OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        [DataMember]
        public decimal? OrderAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        [DataMember]
        public decimal? IncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 应收金额
        /// </summary>
        [DataMember]
        public decimal? PayAmount
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
        /// 礼品卡支付金额
        /// </summary>
        [DataMember]
        public decimal? GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 确认时间
        /// </summary>
        [DataMember]
        public DateTime? ConfirmTime
        {
            get;
            set;
        }

        /// <summary>
        /// 确认操作人编号
        /// </summary>
        [DataMember]
        public int? ConfirmUserSysNo
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
        /// 用户编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo
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
        /// 凭证号
        /// </summary>
        [DataMember]
        public string ReferenceID
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
        /// 流水号
        /// </summary>
        [DataMember]
        public string ExternalKey
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