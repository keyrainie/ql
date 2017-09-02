using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 订单基本信息VM
    /// </summary>
    public class SOBaseInfoVM : ModelBase
    {
        private int? m_PayTypeSysNo;
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get
            {
                return m_PayTypeSysNo;
            }
            set
            {
                base.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);
            }
        }

        private int? m_SOSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
        {
            get
            {
                return m_SOSysNo;
            }
            set
            {
                base.SetValue("SOSysNo", ref m_SOSysNo, value);
            }
        }

        private decimal? m_SOTotalAmount;
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal? SOTotalAmount
        {
            get
            {
                return m_SOTotalAmount;
            }
            set
            {
                base.SetValue("SOTotalAmount", ref m_SOTotalAmount, value);
            }
        }

        private decimal? m_ReceivableAmount;
        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal? ReceivableAmount
        {
            get
            {
                return m_ReceivableAmount;
            }
            set
            {
                base.SetValue("ReceivableAmount", ref m_ReceivableAmount, value);
            }
        }

        private decimal? m_PrepayAmount;
        /// <summary>
        /// 预付款金额
        /// </summary>
        public decimal? PrepayAmount
        {
            get
            {
                return m_PrepayAmount;
            }
            set
            {
                base.SetValue("PrepayAmount", ref m_PrepayAmount, value);
            }
        }

        private decimal? m_PointPay;
        /// <summary>
        /// 支付的积分
        /// </summary>
        public decimal? PointPay
        {
            get
            {
                return m_PointPay;
            }
            set
            {
                base.SetValue("PointPay", ref m_PointPay, value);
            }
        }

        private decimal? m_GiftCardPay;
        /// <summary>
        /// 支付的礼品卡
        /// </summary>
        public decimal? GiftCardPay
        {
            get
            {
                return m_GiftCardPay;
            }
            set
            {
                base.SetValue("GiftCardPay", ref m_GiftCardPay, value);
            }
        }

        public SOStatus? m_Status;

        public SOStatus? Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                base.SetValue("Status", ref m_Status, value);
            }
        }
    }
}