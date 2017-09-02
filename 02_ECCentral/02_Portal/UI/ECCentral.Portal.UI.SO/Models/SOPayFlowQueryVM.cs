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

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOPayFlowQueryVM : ModelBase
    {
        public bool IsTrue { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        private string m_BillNo;
        public string BillNo
        {
            get { return this.m_BillNo; }
            set { this.SetValue("BillNo", ref m_BillNo, value); }
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        string m_PayAmount;
        public string PayAmount
        {
            get { return this.m_PayAmount; }
            set { this.SetValue("PayAmount", ref m_PayAmount, value); }
        }

        /// <summary>
        /// 交易状态
        /// </summary>
        private string m_TrxState;
        public string TrxState
        {
            get { return this.m_TrxState; }
            set { this.SetValue("TrxState", ref m_TrxState, value); }
        }

        /// <summary>
        /// 支付平台处理时间
        /// </summary>
        private string m_RdoTime;
        public string RdoTime
        {
            get { return this.m_RdoTime; }
            set { this.SetValue("RdoTime", ref m_RdoTime, value); }
        }
    }
}
