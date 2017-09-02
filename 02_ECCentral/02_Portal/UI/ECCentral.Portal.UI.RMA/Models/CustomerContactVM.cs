using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class CustomerContactVM : ModelBase
    {
        public CustomerContactVM()
        {
        }

        /// <summary>
        /// 用于控制控件是否只读或可用
        /// </summary>
        private bool isPop;
		public bool IsPop 
		{ 
			get
			{
				return isPop;
			}			
			set
			{
				SetValue("IsPop", ref isPop, value);
			} 
		}		

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_RMARequestSysno;
        public Int32? RMARequestSysno
        {
            get { return this.m_RMARequestSysno; }
            set { this.SetValue("RMARequestSysno", ref m_RMARequestSysno, value); }
        }

        private String m_ReceiveContact;
        public String ReceiveContact
        {
            get { return this.m_ReceiveContact; }
            set { this.SetValue("ReceiveContact", ref m_ReceiveContact, value); }
        }

        private Int32? m_ReceiveAreaSysNo;
        [Validate(ValidateType.Required)]
        public Int32? ReceiveAreaSysNo
        {
            get { return this.m_ReceiveAreaSysNo; }
            set { this.SetValue("ReceiveAreaSysNo", ref m_ReceiveAreaSysNo, value); }
        }

        private String m_ReceiveName;
        public String ReceiveName
        {
            get { return this.m_ReceiveName; }
            set { this.SetValue("ReceiveName", ref m_ReceiveName, value); }
        }

        private String m_ReceivePhone;
        public String ReceivePhone
        {
            get { return this.m_ReceivePhone; }
            set { this.SetValue("ReceivePhone", ref m_ReceivePhone, value); }
        }

        private String m_ReceiveCellPhone;
        public String ReceiveCellPhone
        {
            get { return this.m_ReceiveCellPhone; }
            set { this.SetValue("ReceiveCellPhone", ref m_ReceiveCellPhone, value); }
        }

        private String m_ReceiveAddress;
        public String ReceiveAddress
        {
            get { return this.m_ReceiveAddress; }
            set { this.SetValue("ReceiveAddress", ref m_ReceiveAddress, value); }
        }

        private String m_ReceiveZip;
        public String ReceiveZip
        {
            get { return this.m_ReceiveZip; }
            set { this.SetValue("ReceiveZip", ref m_ReceiveZip, value); }
        }

        private Int32? m_RefundPayType;
        [Validate(ValidateType.Required)]
        public Int32? RefundPayType
        {
            get { return this.m_RefundPayType; }
            set { this.SetValue("RefundPayType", ref m_RefundPayType, value); }
        }

        private String m_BranchBankName;
        public String BranchBankName
        {
            get { return this.m_BranchBankName; }
            set { this.SetValue("BranchBankName", ref m_BranchBankName, value); }
        }

        private String m_CardNumber;
        public String CardNumber
        {
            get { return this.m_CardNumber; }
            set { this.SetValue("CardNumber", ref m_CardNumber, value); }
        }

        private String m_CardOwnerName;
        public String CardOwnerName
        {
            get { return this.m_CardOwnerName; }
            set { this.SetValue("CardOwnerName", ref m_CardOwnerName, value); }
        }

        public List<CodeNamePair> RefundPayTypes { get; set; }
    }
}
