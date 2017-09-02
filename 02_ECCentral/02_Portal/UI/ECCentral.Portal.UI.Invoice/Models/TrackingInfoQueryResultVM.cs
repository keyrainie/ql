using System;
using System.Collections.Generic;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class TrackingInfoQueryResultVM : ModelBase
    {
        private List<TrackingInfoVM> m_ResultList;
        public List<TrackingInfoVM> ResultList
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
        /// 统计信息
        /// </summary>
        public TrackingInfoQueryStatisticVM Statistic
        {
            get;
            set;
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

    public class TrackingInfoVM : ModelBase
    {
        private bool m_IsChecked;
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

        public int? SysNo
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
        /// 收款来源方式
        /// </summary>
        public NetPaySource? Source
        {
            get;
            set;
        }

        public string EditUser
        {
            get;
            set;
        }

        public DateTime? EditDate
        {
            get;
            set;
        }

        public DateTime? OutDate
        {
            get;
            set;
        }

        public DateTime? AuditDate
        {
            get;
            set;
        }

        public int? OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式系统编号
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

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string ShipTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 配送员姓名
        /// </summary>
        public string DeliveryManName
        {
            get;
            set;
        }

        /// <summary>
        /// 处理进度
        /// </summary>
        public TrackingInfoStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal? OrderAmt
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
        /// 礼品卡金额
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get;
            set;
        }

        private string m_IncomeAmt;
        /// <summary>
        /// 实收金额
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^((((-)?[1-9][0-9]{0,12})|0)(\\.[0-9]{0,2})?)?$")]
        public string IncomeAmt
        {
            get
            {
                return m_IncomeAmt;
            }
            set
            {
                base.SetValue("IncomeAmt", ref m_IncomeAmt, value);
            }
        }

        /// <summary>
        /// 未收金额
        /// </summary>
        public decimal? UnpayedAmt
        {
            get;
            set;
        }

        public SOIncomeStatus? OrderStatus
        {
            get;
            set;
        }

        private string m_ResponsibleUserName;
        /// <summary>
        /// 责任人姓名
        /// </summary>
        public string ResponsibleUserName
        {
            get
            {
                return m_ResponsibleUserName;
            }
            set
            {
                base.SetValue("ResponsibleUserName", ref m_ResponsibleUserName, value);
            }
        }

        private int? m_LossType;
        /// <summary>
        /// 损失类型ID
        /// </summary>
        public int? LossType
        {
            get
            {
                return m_LossType;
            }
            set
            {
                base.SetValue("LossType", ref m_LossType, value);
            }
        }

        /// <summary>
        /// 损失类型(文字描述)
        /// </summary>
        public string LossTypeDesc
        {
            get;
            set;
        }

        public DateTime PredictedFinishTime
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string CSNote
        {
            get;
            set;
        }

        public string InvoiceNote
        {
            get;
            set;
        }

        public string FinanceNote
        {
            get;
            set;
        }

        public string AppendNote
        {
            get;
            set;
        }

        public SOIncomeOrderStyle? IncomeStyle
        {
            get;
            set;
        }

        public TrackingInfoStyle? InType
        {
            get;
            set;
        }

        /// <summary>
        /// 连接单据号
        /// RO_Balance 为RO单号
        /// </summary>
        public int LinkSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }
    }

    public class TrackingInfoQueryStatisticVM : ModelBase, IStatisticInfo
    {
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmt
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
        /// 礼品卡金额
        /// </summary>
        public decimal GiftCardPayAmt
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
        /// 未收金额
        /// </summary>
        public decimal UnpayedAmt
        {
            get;
            set;
        }

        #region IStatisticInfo Members

        public string ToStatisticText()
        {
            StringBuilder statisticInfo = new StringBuilder();
            statisticInfo.AppendLine(string.Format(ECCentral.Portal.UI.Invoice.Resources.ResARWindowQuery.Message_StatisticPrompInfo
                , ConstValue.Invoice_ToCurrencyString(OrderAmt)
                , ConstValue.Invoice_ToCurrencyString(PrepayAmt)
                , ConstValue.Invoice_ToCurrencyString(GiftCardPayAmt)
                , ConstValue.Invoice_ToCurrencyString(IncomeAmt)
                , ConstValue.Invoice_ToCurrencyString(UnpayedAmt))
            );
            return statisticInfo.ToString();
        }

        #endregion IStatisticInfo Members
    }
}