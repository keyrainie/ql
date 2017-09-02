using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 支付信息视图模型，兼容netpay和postpay
    /// </summary>
    public class PayInfoVM : ModelBase
    {
        private string m_PayTypeSysNo;
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public string PayTypeSysNo
        {
            get
            {
                return this.m_PayTypeSysNo;
            }
            set
            {
                this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);
            }
        }

        private string m_SOSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get
            {
                return this.m_SOSysNo;
            }
            set
            {
                this.SetValue("SOSysNo", ref m_SOSysNo, value);
            }
        }

        private decimal? m_SOTotalAmount;
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal? SOTotalAmt
        {
            get
            {
                return this.m_SOTotalAmount;
            }
            set
            {
                this.SetValue("SOTotalAmt", ref m_SOTotalAmount, value);
            }
        }

        private decimal? m_ReceivableAmt;
        /// <summary>
        /// 应收款金额
        /// </summary>
        public decimal? ReceivableAmt
        {
            get
            {
                return m_ReceivableAmt;
            }
            set
            {
                base.SetValue("ReceivableAmt", ref m_ReceivableAmt, value);
            }
        }

        private decimal? m_PrepayAmt;
        /// <summary>
        /// 预付款金额
        /// </summary>
        public decimal? PrepayAmt
        {
            get
            {
                return this.m_PrepayAmt;
            }
            set
            {
                this.SetValue("PrepayAmt", ref m_PrepayAmt, value);
            }
        }

        private string m_PayAmt;
        /// <summary>
        /// 实收金额
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d+(\.\d{0,})?$")]
        public string PayAmt
        {
            get
            {
                return this.m_PayAmt;
            }
            set
            {
                this.SetValue("PayAmt", ref m_PayAmt, value);
            }
        }

        private decimal? m_GiftCardPayAmt;
        /// <summary>
        /// 支付的礼品卡金额
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get
            {
                return this.m_GiftCardPayAmt;
            }
            set
            {
                this.SetValue("GiftCardPayAmt", ref m_GiftCardPayAmt, value);
            }
        }

        private string m_RelatedSOSysNo;
        /// <summary>
        ///  CS确认的订单号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^(\\d+)$")]
        public string RelatedSOSysNo
        {
            get
            {
                return m_RelatedSOSysNo;
            }
            set
            {
                base.SetValue("RelatedSOSysNo", ref m_RelatedSOSysNo, value);
            }
        }

        #region 延迟加载的属性

        public static List<PayType> NetPayTypeList
        {
            get;
            set;
        }

        public static List<PayType> PostPayTypeList
        {
            get;
            set;
        }

        #endregion 延迟加载的属性
    }
}