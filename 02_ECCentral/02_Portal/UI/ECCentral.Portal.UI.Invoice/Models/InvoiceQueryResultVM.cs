using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 分公司收款单查询结果VM
    /// </summary>
    public class InvoiceQueryResultVM : ModelBase
    {
        public InvoiceQueryResultVM()
        {
            ResultList = new List<InvoiceVM>();
            Statistic = new StatisticCollection<InvoiceQueryStatisticVM>();
        }

        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        private List<InvoiceVM> m_ResultList;
        public List<InvoiceVM> ResultList
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

        private StatisticCollection<InvoiceQueryStatisticVM> m_Statistic;
        /// <summary>
        /// 分公司收款单统计放在客户端计算
        /// </summary>
        public StatisticCollection<InvoiceQueryStatisticVM> Statistic
        {
            get
            {
                return m_Statistic;
            }
            set
            {
                base.SetValue("Statistic", ref m_Statistic, value);
            }
        }

        /// <summary>
        /// 金额信息
        /// </summary>
        public InvoiceAmtVM InvoiceAmt
        {
            get;
            set;
        }
    }

    public class InvoiceVM : ModelBase
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
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单类型
        /// </summary>
        public SOIncomeOrderStyle? IncomeStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public SOIncomeOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 单据系统编号
        /// </summary>
        public int? OrderSysNo
        {
            get;
            set;
        }

        public int? NewOrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户系统编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据ID
        /// </summary>
        public string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? IncomeStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库名
        /// </summary>
        public string StockName
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string StockID
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
        /// 发票金额
        /// </summary>
        public decimal? InvoiceAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预收金额
        /// </summary>
        public decimal? PrepayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 代收金额
        /// </summary>
        public decimal? UnionAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡金额
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? SOTotalAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 建立人系统编号
        /// </summary>
        private int? IncomeUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 建立人
        /// </summary>
        public string IncomeUser
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人系统编号
        /// </summary>
        public int? ConfirmUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string ConfirmUser
        {
            get;
            set;
        }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo
        {
            get;
            set;
        }

        /// <summary>
        /// 发票录入时间
        /// </summary>
        public DateTime? InvoiceCreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 建立时间
        /// </summary>
        public DateTime? IncomeTime
        {
            get;
            set;
        }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? ConfirmTime
        {
            get;
            set;
        }

        public SapImportedStatus? SapImportedStatus
        {
            get;
            set;
        }//   -- 导入状态

        public DateTime? SAPPostDate
        {
            get;
            set;
        }// --导入时间

        public string SAPDocNo
        {
            get;
            set;
        }//    --导入凭证号

        public string SapInFailedReason
        {
            get;
            set;
        } //失败原因
    }

    public class InvoiceQueryStatisticVM : ModelBase, IStatisticInfo
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public StatisticType StatisticType
        {
            get;
            set;
        }

        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal? InvoiceAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 发票金额（含税）
        /// </summary>
        public decimal? InvoiceAmtWithTax
        {
            get;
            set;
        }

        /// <summary>
        /// 税金
        /// </summary>
        public decimal? InvoiceTax
        {
            get;
            set;
        }

        /// <summary>
        /// 预收金额
        /// </summary>
        public decimal? PrepayAmt
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
        /// 礼品卡支付金额
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡支付金额（含税）
        /// </summary>
        public decimal? GiftCardPayAmtWithTax
        {
            get;
            set;
        }

        /// <summary>
        /// 代收金额
        /// </summary>
        public decimal? UnionAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? SOTotalAmt
        {
            get;
            set;
        }

        #region IStatisticInfo Members

        public string ToStatisticText()
        {
            return string.Format(ResInvoiceQuery.Message_StatisticInfoFormat
            , this.StatisticType.ToDescription()
            , ConstValue.Invoice_ToCurrencyString(this.InvoiceAmt)
            , ConstValue.Invoice_ToCurrencyString(this.SOTotalAmt)
            , ConstValue.Invoice_ToCurrencyString(this.InvoiceAmtWithTax)
            , ConstValue.Invoice_ToCurrencyString(this.InvoiceTax)
            , ConstValue.Invoice_ToCurrencyString(this.PrepayAmt)
            , ConstValue.Invoice_ToCurrencyString(this.IncomeAmt)
            , ConstValue.Invoice_ToCurrencyString(this.GiftCardPayAmt)
            , ConstValue.Invoice_ToCurrencyString(this.GiftCardPayAmtWithTax)
            , ConstValue.Invoice_ToCurrencyString(this.UnionAmt.Value)
            );
        }

        #endregion IStatisticInfo Members
    }

    /// <summary>
    /// 金额信息，用户客户端计算统计值
    /// </summary>
    public class InvoiceAmtVM : ModelBase
    {
        /// <summary>
        /// 发票金额（不包含代收金额）
        /// </summary>
        public decimal TotalAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal IncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预收金额
        /// </summary>
        public decimal PrepayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡支付金额
        /// </summary>
        public decimal GiftCardPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 代收金额
        /// </summary>
        public decimal UnionAmt
        {
            get;
            set;
        }

        /// <summary>
        /// SO单总金额
        /// </summary>
        public decimal TotalAmtForSOOnly
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? TSOTotalAmt
        {
            get;
            set;
        }
    }
}