using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class POSPayQueryResultVM : ModelBase
    {
        public POSPayQueryResultVM()
        {
            ResultList = new List<POSPayVM>();
            Statistic = new List<POSPayQueryStatisticVM>();
        }

        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }

        private List<POSPayVM> m_ResultList;
        /// <summary>
        /// 查询结果
        /// </summary>
        public List<POSPayVM> ResultList
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

        private List<POSPayQueryStatisticVM> m_Statistic;
        /// <summary>
        /// 统计信息
        /// </summary>
        public List<POSPayQueryStatisticVM> Statistic
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

    /// <summary>
    /// 查询结果信息
    /// </summary>
    public class POSPayVM : ModelBase
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
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
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
        /// POS支付方式
        /// </summary>
        public POSPayType POSPayType
        {
            get;
            set;
        }

        ///<summary>
        ///POS终端号
        ///</summary>
        public string POSTerminalID
        {
            get;
            set;
        }

        /// <summary>
        /// POS收款时间
        /// </summary>
        public DateTime? PayedDate
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
        /// POS收款金额
        /// </summary>
        public decimal? PayedAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 确认金额
        /// </summary>
        public decimal? ConfirmedAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 单据预收金额
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
        /// 实收金额
        /// </summary>
        public decimal? SOIncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 自动确认状态
        /// </summary>
        public AutoConfirmStatus? AutoConfirmStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? SOIncomeStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单编号
        /// </summary>
        public int? SOIncomeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string ConfirmedInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 关联的订单是否是并单
        /// </summary>
        public bool? IsCombine
        {
            get;
            set;
        }

        /// <summary>
        /// 合单号
        /// </summary>
        public string CombineNumber
        {
            get;
            set;
        }

        #region UI扩展属性

        /// <summary>
        /// 将SOIncomeStatus转换成其文字描述，如果Status为NULL的话则转换成字符串“--”
        /// </summary>
        public string SOIncomeStatusString
        {
            get
            {
                return SOIncomeStatus.HasValue ? SOIncomeStatus.Value.ToDescription() : "--";
            }
        }

        #endregion UI扩展属性
    }

    /// <summary>
    /// 统计信息
    /// </summary>
    public sealed class POSPayQueryStatisticVM : ModelBase
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public StatisticType StatisticType
        {
            get;
            set;
        }

        private decimal? m_OrderAmt;
        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? OrderAmt
        {
            get
            {
                return m_OrderAmt;
            }
            set
            {
                base.SetValue("OrderAmt", ref m_OrderAmt, value);
            }
        }

        private decimal? m_PayedAmt;
        /// <summary>
        /// POS收款金额
        /// </summary>
        public decimal? PayedAmt
        {
            get
            {
                return m_PayedAmt;
            }
            set
            {
                base.SetValue("PayedAmt", ref m_PayedAmt, value);
            }
        }

        private decimal? m_ConfirmedAmt;
        /// <summary>
        /// 已确认金额
        /// </summary>
        public decimal? ConfirmedAmt
        {
            get
            {
                return m_ConfirmedAmt;
            }
            set
            {
                base.SetValue("ConfirmedAmt", ref m_ConfirmedAmt, value);
            }
        }

        private decimal? m_SOIncomeAmt;
        /// <summary>
        /// 收款单实收金额
        /// </summary>
        public decimal? SOIncomeAmt
        {
            get
            {
                return m_SOIncomeAmt;
            }
            set
            {
                base.SetValue("SOIncomeAmt", ref m_SOIncomeAmt, value);
            }
        }

        private decimal? m_DiffAmt;
        /// <summary>
        /// 差异金额（POS收款-实收）
        /// </summary>
        public decimal? DiffAmt
        {
            get
            {
                return m_DiffAmt;
            }
            set
            {
                base.SetValue("DiffAmt", ref m_DiffAmt, value);
            }
        }
    }
}