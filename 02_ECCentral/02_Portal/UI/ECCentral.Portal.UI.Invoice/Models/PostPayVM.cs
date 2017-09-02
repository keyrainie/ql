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
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 维护PostPay时的ViewModel
    /// </summary>
    public class PostPayVM : ModelBase
    {
        public PostPayVM()
        {
            PayInfo = new PayInfoVM();
            RefundInfo = new RefundInfoVM();
            IsForceCheck = false;
        }

        private int? m_SysNo;
        public int? SysNo
        {
            get
            {
                return m_SysNo;
            }
            set
            {
                this.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private bool? m_IsForceCheck;
        /// <summary>
        /// 是否强制核收
        /// </summary>
        public bool? IsForceCheck
        {
            get
            {
                return m_IsForceCheck;
            }
            set
            {
                base.SetValue("IsForceCheck", ref m_IsForceCheck, value);
            }
        }

        private decimal? m_RemainAmt;
        /// <summary>
        /// 收款单剩余金额
        /// </summary>
        public decimal? RemainAmt
        {
            get
            {
                return m_RemainAmt;
            }
            set
            {
                base.SetValue("RemainAmt", ref m_RemainAmt, value);
            }
        }

        private List<ConfirmedOrderVM> m_ConfirmedOrderList;
        /// <summary>
        ///  CS确认的订单号
        /// </summary>
        public List<ConfirmedOrderVM> ConfirmedOrderList
        {
            get
            {
                return m_ConfirmedOrderList;
            }
            set
            {
                base.SetValue("ConfirmedOrderList", ref m_ConfirmedOrderList, value);
            }
        }

        private PayInfoVM m_PayInfo;
        /// <summary>
        /// 支付信息
        /// </summary>
        public PayInfoVM PayInfo
        {
            get
            {
                return m_PayInfo;
            }
            set
            {
                base.SetValue("PayInfo", ref m_PayInfo, value);
            }
        }

        private RefundInfoVM m_RefundInfo;
        /// <summary>
        /// 退款信息
        /// </summary>
        public RefundInfoVM RefundInfo
        {
            get
            {
                return m_RefundInfo;
            }
            set
            {
                base.SetValue("RefundInfo", ref m_RefundInfo, value);
            }
        }
    }

    public class ConfirmedOrderVM : ModelBase
    {
        private PostIncomeConfirmStatus? m_ConfirmStatus;
        /// <summary>
        /// 确认状态
        /// </summary>
        public PostIncomeConfirmStatus? ConfirmStatus
        {
            get
            {
                return m_ConfirmStatus;
            }
            set
            {
                base.SetValue("ConfirmStatus", ref m_ConfirmStatus, value);
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
    }
}