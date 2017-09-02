using System;
using ECommerce.Enums;

namespace ECommerce.Entity.Invoice
{
    /// <summary>
    /// 退款信息
    /// </summary>
    public class SOIncomeRefundInfo : EntityBase
    {
        #region 基本信息

        /// <summary>
        /// 财务收款单编号
        /// </summary>
        public int? SOIncomeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 退款单类型
        /// </summary>
        public RefundOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RefundPayType
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public RefundStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 业务单据系统编号，如果是RO单，则是RO单编号
        /// </summary>
        public int? OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人系统编号
        /// </summary>
        public int? AuditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUserName
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 退款备注
        /// </summary>
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// 是否物流拒收
        /// </summary>
        public bool? HaveAutoRMA
        {
            get;
            set;
        }

        /// <summary>
        /// 退款原因
        /// </summary>
        public int? RefundReason
        {
            get;
            set;
        }

        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinNote
        {
            get;
            set;
        }

        /// <summary>
        /// 销售渠道编号（冗余）
        /// </summary>
        public string ChannelID
        {
            get;
            set;
        }

        #endregion 基本信息

        #region 财务信息

        /// <summary>
        /// 退还现金
        /// </summary>
        public decimal? RefundCashAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 退还积分
        /// </summary>
        public int? RefundPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 系统精度冗余(>=0)
        /// </summary>
        public decimal? ToleranceAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 退还礼品卡
        /// </summary>
        public decimal? RefundGiftCard
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// 用于创建财务负收款单（NegativeSOIncome）时判断是否优先退现金
        /// </summary>
        public decimal? PayAmount
        {
            get;
            set;
        }

        #endregion 财务信息

        #region 银行信息

        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// 支行
        /// </summary>
        public string BranchBankName
        {
            get;
            set;
        }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 持卡人
        /// </summary>
        public string CardOwnerName
        {
            get;
            set;
        }

        #endregion 银行信息

        #region 邮政信息

        /// <summary>
        /// 邮政地址
        /// </summary>
        public string PostAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 邮政编号
        /// </summary>
        public string PostCode
        {
            get;
            set;
        }

        /// <summary>
        /// 收款人
        /// </summary>
        public string ReceiverName
        {
            get;
            set;
        }

        #endregion 邮政信息

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

    }
}