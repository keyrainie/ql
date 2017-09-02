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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class ReconciliationQueryVM : ModelBase
    {
        private String m_OrderID;
        /// <summary>
        /// 单据ID
        /// </summary>
        [Validate(ValidateType.Regex, @"^(R3)?[1-9][0-9]{0,9}(\.(R3)?[1-9][0-9]{0,9}){0,10}$")]
        public String OrderID
        {
            get
            {
                return this.m_OrderID;
            }
            set
            {
                this.SetValue("OrderID", ref m_OrderID, value);
            }
        }
        
        /// <summary>
        /// 订单编号
        /// </summary>
        private String m_OrderSysNo;
        public String OrderSysNo
        {
            get
            {
                return this.m_OrderSysNo;
            }
            set
            {
                this.SetValue("OrderSysNo", ref m_OrderSysNo, value);
            }
        }

        /// <summary>
        /// 支付流水单号
        /// </summary>
        private String m_SerialNo;
        public String SerialNo
        {
            get
            {
                return this.m_SerialNo;
            }
            set
            {
                this.SetValue("SerialNo", ref m_SerialNo, value);
            }
        }

        private DateTime? m_CreateDateFrom;
        /// <summary>
        /// 创建时间起
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get
            {
                return this.m_CreateDateFrom;
            }
            set
            {
                this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value);
            }
        }

        private DateTime? m_CreateDateTo;
        /// <summary>
        /// 创建时间止
        /// </summary>
        public DateTime? CreateDateTo
        {
            get
            {
                return this.m_CreateDateTo;
            }
            set
            {
                this.SetValue("CreateDateTo", ref m_CreateDateTo, value);
            }
        }

        private DateTime? m_ConfirmDateFrom;
        /// <summary>
        /// 确认时间起
        /// </summary>
        public DateTime? ConfirmDateFrom
        {
            get
            {
                return this.m_ConfirmDateFrom;
            }
            set
            {
                this.SetValue("ConfirmDateFrom", ref m_ConfirmDateFrom, value);
            }
        }

        private DateTime? m_ConfirmDateTo;
        /// <summary>
        /// 确认时间止
        /// </summary>
        public DateTime? ConfirmDateTo
        {
            get
            {
                return this.m_ConfirmDateTo;
            }
            set
            {
                this.SetValue("ConfirmDateTo", ref m_ConfirmDateTo, value);
            }
        }


    }
}
