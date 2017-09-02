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
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.SO.Models
{
    /// <summary>
    /// 礼品卡使用日志
    /// </summary>
    public class GiftCardRedeemLogVM : ModelBase
    {
        private int? m_TransactionNumber;
        public int? TransactionNumber
        {
            get { return this.m_TransactionNumber; }
            set { this.SetValue("TransactionNumber", ref m_TransactionNumber, value); }
        }

        /// <summary>
        /// 卡号
        /// </summary>
        private String m_Code;
        public String Code
        {
            get { return this.m_Code; }
            set { this.SetValue("Code", ref m_Code, value); }
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        private int? m_CustomerSysNo;
        public int? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        /// <summary>
        /// 使用余额
        /// </summary>
        private Decimal? m_Amount;
        public Decimal? Amount
        {
            get { return this.m_Amount; }
            set { this.SetValue("Amount", ref m_Amount, value); }
        }

        /// <summary>
        /// 状态 A:有效 D:无效 
        /// </summary>
        private ValidStatus m_Status;
        public ValidStatus Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        /// <summary>
        /// 使用单据的编号
        /// </summary>
        private int? m_ActionSysNo;
        public int? ActionSysNo 
        {
            get { return this.m_ActionSysNo; }
            set { this.SetValue("ActionSysNo", ref m_ActionSysNo, value); }
        }

        /// <summary>
        /// 类型，比如：SO
        /// </summary>
        private ActionType m_ActionType;
        public ActionType ActionType
        {
            get { return this.m_ActionType; }
            set { this.SetValue("ActionType", ref m_ActionType, value); }
        }

        /// <summary>
        /// 礼品卡总金额
        /// </summary>
        private Decimal? m_TotalAmount;
        public Decimal? TotalAmount
        {
            get { return this.m_TotalAmount; }
            set { this.SetValue("TotalAmount", ref m_TotalAmount, value); }
        }

        /// <summary>
        /// 可用余额
        /// </summary>
        private Decimal? m_AvailAmount;
        public Decimal? AvailAmount
        {
            get { return this.m_AvailAmount; }
            set { this.SetValue("AvailAmount", ref m_AvailAmount, value); }
        }

        /// <summary>
        /// 类型
        /// </summary>
        private CardMaterialType m_Type;
        public CardMaterialType Type     
        {
            get { return this.m_Type; }
            set { this.SetValue("Type", ref m_Type, value); }
        }
    }
}
