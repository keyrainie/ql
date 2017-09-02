using System;
using System.Collections.Generic;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class SaleIncomeQueryResultVM : ModelBase
    {
        public SaleIncomeQueryResultVM()
        {
            ResultList = new List<SaleIncomeVM>();
            Statistic = new StatisticCollection<SaleIncomeQueryStatisticVM>();
        }

        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        private List<SaleIncomeVM> m_ResultList;
        /// <summary>
        /// 查询结果列表
        /// </summary>
        public List<SaleIncomeVM> ResultList
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

        private StatisticCollection<SaleIncomeQueryStatisticVM> m_Statistic;
        /// <summary>
        /// 统计信息
        /// </summary>
        public StatisticCollection<SaleIncomeQueryStatisticVM> Statistic
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
    }

    public class SaleIncomeVM : ModelBase
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
        /// 收款单系统编号
        /// </summary>
        public int? SysNo
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

        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int? CustomerSysNo
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
        /// 单据ID
        /// </summary>
        public string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是并单
        /// </summary>
        public bool? IsCombine
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
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? IncomeStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? OrderAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预收款金额
        /// </summary>
        public decimal? PrepayAmt
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
        /// 积分
        /// </summary>
        public decimal? PointPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 优惠券
        /// </summary>
        public decimal? PomotionAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal? TariffAmt
        {
            get;
            set;
        }


        private decimal? m_IncomeAmt;
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? IncomeAmt
        {
            get
            {
                return m_IncomeAmt;
            }
            set
            {
                base.SetValue("IncomeAmt", ref m_IncomeAmt, value);
                m_IncomeAmtForEdit = m_IncomeAmt.ToString();
            }
        }

        /// <summary>
        /// 界面绑定实收金额时使用，为了配合验证，使用String类型
        /// </summary>
        private string m_IncomeAmtForEdit;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d+(\.\d{0,})?$")]
        public string IncomeAmtForEdit
        {
            get
            {
                return m_IncomeAmtForEdit;
            }
            set
            {
                decimal incomeAmt;
                IncomeAmt = decimal.TryParse(m_IncomeAmtForEdit, out incomeAmt) ? incomeAmt : (decimal?)null;
                base.SetValue("IncomeAmtForEdit", ref m_IncomeAmtForEdit, value);
            }
        }

        /// <summary>
        /// 退还积分
        /// </summary>
        public int? ReturnPointAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 退还现金
        /// </summary>
        public decimal? ReturnCash
        {
            get;
            set;
        }

        /// <summary>
        /// 转礼品卡
        /// </summary>
        public decimal? RefundGiftCard
        {
            get;
            set;
        }

        /// <summary>
        /// 精度冗余
        /// </summary>
        public decimal? ToleranceAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 运费收入
        /// </summary>
        public decimal? ShipPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 基本运费
        /// </summary>
        public decimal? ShippingFee
        {
            get;
            set;
        }

        /// <summary>
        /// 打包费
        /// </summary>
        public decimal? PackageFee
        {
            get;
            set;
        }

        /// <summary>
        /// 挂号费
        /// </summary>
        public decimal? RegisteredFee
        {
            get;
            set;
        }

        /// <summary>
        /// 运费成本
        /// </summary>
        public decimal? ShipCost
        {
            get;
            set;
        }

        public DateTime? IncomeTime
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
        /// 出库时间
        /// </summary>
        public DateTime? OutTime
        {
            get;
            set;
        }

        /// <summary>
        /// 确认收款时间
        /// </summary>
        public DateTime? ConfirmTime
        {
            get;
            set;
        }

        /// <summary>
        /// 网关收款时间
        /// </summary>
        public DateTime? PayedDate
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

        private string m_ReferenceID;
        /// <summary>
        /// 凭证号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ReferenceID
        {
            get
            {
                return m_ReferenceID;
            }
            set
            {
                base.SetValue("ReferenceID", ref m_ReferenceID, value);
            }
        }

        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RMARefundPayType
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

        /// <summary>
        /// Pos预付（货到付款相关）
        /// </summary>
        public decimal? PosPrePay { get; set; }
        /// <summary>
        /// Pos现金（货到付款相关）
        /// </summary>
        public decimal? PosCash { get; set; }
        /// <summary>
        /// Pos银行卡（货到付款相关）
        /// </summary>
        public decimal? PosBankCard { get; set; }

        /// <summary>
        /// 成本金额
        /// </summary>
        public decimal? CostAmout
        {
            get;
            set;
        }

        /// <summary>
        /// 商品金额
        /// </summary>
        public decimal? ProductPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式系统编码
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string PayTypeName
        {
            get;
            set;
        }
    }

    public class SaleIncomeQueryStatisticVM : ModelBase, IStatisticInfo
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public StatisticType? StatisticType
        {
            get;
            set;
        }

        public decimal? OrderAmt
        {
            get;
            set;
        }

        public decimal? IncomeAmt
        {
            get;
            set;
        }

        public decimal? AlreadyIncomeAmt
        {
            get;
            set;
        }

        public decimal? PrepayAmt
        {
            get;
            set;
        }

        public decimal? ShipPrice
        {
            get;
            set;
        }

        public decimal? ReturnCash
        {
            get;
            set;
        }

        public decimal? ToleranceAmt
        {
            get;
            set;
        }

        public int? ReturnPoint
        {
            get;
            set;
        }

        #region IStatisticInfo Members

        public string ToStatisticText()
        {
            return string.Format(ECCentral.Portal.UI.Invoice.Resources.ResSaleIncomeQuery.Message_StatisticInfo
                        , this.StatisticType.ToDescription()
                        , ConstValue.Invoice_ToCurrencyString(this.OrderAmt)
                        , ConstValue.Invoice_ToCurrencyString(this.IncomeAmt)
                        , ConstValue.Invoice_ToCurrencyString(this.AlreadyIncomeAmt)
                        , ConstValue.Invoice_ToCurrencyString(this.PrepayAmt.Value)
                        , ConstValue.Invoice_ToCurrencyString(this.ShipPrice.Value));
        }

        #endregion IStatisticInfo Members
    }
}