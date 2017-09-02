using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class AuditRefundQueryResultVM : ModelBase
    {
        private List<AuditRefundVM> m_ResultList;
        public List<AuditRefundVM> ResultList
        {
            get
            {
                return m_ResultList.DefaultIfNull();
            }
            set
            {
                base.SetValue("ResultList", ref m_ResultList, value);
            }
        }

        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }
    }

    public class AuditRefundVM : ModelBase
    {
        private bool m_IsChecked;
        /// <summary>
        /// 记录是否被选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        /// <summary>
        /// 销售-退款单系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户系统编号
        /// </summary>
        public int CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public RefundStatus? AuditStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public RefundOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 退款单号
        /// </summary>
        public int? RMANumber
        {
            get;
            set;
        }

        /// <summary>
        /// 退款原因系统编号
        /// </summary>
        public int RefundReasonSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 退款原因
        /// </summary>
        public string RMAReason
        {
            get;
            set;
        }

        /// <summary>
        /// 退款状态
        /// </summary>
        public SOIncomeStatus? RefundStatus
        {
            get;
            set;
        }

        /// <summary>
        ///  退款类型
        /// </summary>
        public RefundPayType? RefundPayType
        {
            get;
            set;
        }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal? RefundCashAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? IncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 是否RMA物流拒收
        /// </summary>
        public bool? ShipRejected //HaveAutoRMA
        {
            get;
            set;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 来源
        /// </summary>
        public NetPaySource? Source
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser
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
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser
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

        #region Refund_BankInfo

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// 分行名称
        /// </summary>
        public string BranchBankName
        {
            get;
            set;
        }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string CardOwnerName
        {
            get;
            set;
        }

        /// <summary>
        /// 邮政地址
        /// </summary>
        public string PostAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostCode
        {
            get;
            set;
        }

        /// <summary>
        /// 收款人
        /// </summary>
        public string CashReceiver
        {
            get;
            set;
        }

        /// <summary>
        /// 系统精度冗余
        /// </summary>
        public decimal? ToleranceAmt
        {
            get;
            set;
        }

        #endregion Refund_BankInfo

        /// <summary>
        /// 付款类型系统编号
        /// </summary>
        public int PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 付款类型名称
        /// </summary>
        public string PayTypeName
        {
            get;
            set;
        }

        public string ExternalKey
        {
            get;
            set;
        }

        public int? EditUserSysNo
        {
            get;
            set;
        }

        public DateTime? EditTime
        {
            get;
            set;
        }

        public string FinNote
        {
            get;
            set;
        }

        public string EditUser
        {
            get;
            set;
        }

        /// <summary>
        /// 支付宝提供两种支付方式选择
        /// 1.直接交易（免手续费。 注：系统会先收取订单总金额1%手续费，待货物出库后，手续费会以积分方式返还到您的账户中）
        /// 2.中介交易（客户承担订单总金额1%手续费)
        /// </summary>
        public bool IsDirectAlipay
        {
            get;
            set;
        }

        /// <summary>
        /// 转礼品卡金额
        /// </summary>
        public decimal? RefundGiftCard
        {
            get;
            set;
        }

        /// <summary>
        /// 转积分
        /// </summary>
        public int RefundPoint
        {
            get;
            set;
        }

        public int SpecialSOTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 交易流水号
        /// </summary>
        public string OutOrderNo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付平台
        /// </summary>
        public string PartnerName
        {
            get;
            set;
        }

        /// <summary>
        /// 退款状态
        /// </summary>
        public WLTRefundStatus WLTRefundStatus
        {
            get;
            set;
        }
    }
}