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

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class ReconciliationVM : ModelBase
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

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        /// <summary>
        /// 订单号
        /// </summary>
        //private Int32? m_SOSysNo;
        //public Int32? SOSysNo
        //{
        //    get { return this.m_SOSysNo; }
        //    set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        //}

        /// <summary>
        /// 支付流水号
        /// </summary>
        private String m_SerialNo;
        public String SerialNo
        {
            get { return this.m_SerialNo; }
            set { this.SetValue("SerialNo", ref m_SerialNo, value); }
        }

        /// <summary>
        /// 订单号
        /// </summary>
        private Int32? m_OrderSysNo;
        public Int32? OrderSysNo
        {
            get { return this.m_OrderSysNo; }
            set { this.SetValue("OrderSysNo", ref m_OrderSysNo, value); }
        }

        /// <summary>
        /// 提交订单时间
        /// </summary>
        private DateTime? m_IncomeTime;
        public DateTime? IncomeTime
        {
            get { return this.m_IncomeTime; }
            set { this.SetValue("IncomeTime", ref m_IncomeTime, value); }
        }

        /// <summary>
        /// 提交确认时间
        /// </summary>
        private DateTime? m_ConfirmTime;
        public DateTime? ConfirmTime
        {
            get { return this.m_ConfirmTime; }
            set { this.SetValue("ConfirmTime", ref m_ConfirmTime, value); }
        }

        /// <summary>
        /// 支付平台处理时间
        /// </summary>
        private DateTime? m_ProcessingTime;
        public DateTime? ProcessingTime
        {
            get { return this.m_ProcessingTime; }
            set { this.SetValue("ProcessingTime", ref m_ProcessingTime, value); }
        }

        /// <summary>
        /// 订单金额
        /// </summary>
        Decimal? m_OrderAmt;
        public Decimal? OrderAmt
        {
            get { return this.m_OrderAmt; }
            set { this.SetValue("OrderAmt", ref m_OrderAmt, value); }
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        Decimal? m_IncomeAmt;
        public Decimal? IncomeAmt
        {
            get { return this.m_IncomeAmt; }
            set { this.SetValue("IncomeAmt", ref m_IncomeAmt, value); }
        }

        /// <summary>
        /// 东方支付金额
        /// </summary>
        Decimal? m_TotalAmount;
        public Decimal? TotalAmount
        {
            get { return this.m_TotalAmount; }
            set { this.SetValue("TotalAmount", ref m_TotalAmount, value); }
        }

        /// <summary>
        /// 商品金额（退商品金额）
        /// </summary>
        Decimal? m_ProductAmount;
        public Decimal? ProductAmount
        {
            get { return this.m_ProductAmount; }
            set { this.SetValue("ProductAmount", ref m_ProductAmount, value); }
        }

    }
}
