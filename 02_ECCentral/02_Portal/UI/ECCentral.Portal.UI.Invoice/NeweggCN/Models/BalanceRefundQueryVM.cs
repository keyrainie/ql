using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class BalanceRefundQueryVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get
            {
                return this.m_SysNo;
            }
            set
            {
                this.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private DateTime? m_CreateTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get
            {
                return this.m_CreateTimeFrom;
            }
            set
            {
                this.SetValue("CreateTimeFrom", ref m_CreateTimeFrom, value);
            }
        }

        private DateTime? m_CreateTimeTo;
        public DateTime? CreateTimeTo
        {
            get
            {
                return this.m_CreateTimeTo;
            }
            set
            {
                this.SetValue("CreateTimeTo", ref m_CreateTimeTo, value);
            }
        }

        private String m_CustomerID;
        [Validate(ValidateType.MaxLength, 50)]
        public String CustomerID
        {
            get
            {
                return this.m_CustomerID;
            }
            set
            {
                this.SetValue("CustomerID", ref m_CustomerID, value);
            }
        }

        private RefundPayType? m_RefundType;
        public RefundPayType? RefundType
        {
            get
            {
                return this.m_RefundType;
            }
            set
            {
                this.SetValue("RefundType", ref m_RefundType, value);
            }
        }

        private String m_Bank;
        [Validate(ValidateType.MaxLength, 50)]
        public String Bank
        {
            get
            {
                return this.m_Bank;
            }
            set
            {
                this.SetValue("Bank", ref m_Bank, value);
            }
        }

        private String m_BranchBank;
        [Validate(ValidateType.MaxLength, 50)]
        public String BranchBank
        {
            get
            {
                return this.m_BranchBank;
            }
            set
            {
                this.SetValue("BranchBank", ref m_BranchBank, value);
            }
        }

        private String m_CardOwner;
        [Validate(ValidateType.MaxLength, 50)]
        public String CardOwner
        {
            get
            {
                return this.m_CardOwner;
            }
            set
            {
                this.SetValue("CardOwner", ref m_CardOwner, value);
            }
        }

        private BalanceRefundStatus? m_Status;
        public BalanceRefundStatus? Status
        {
            get
            {
                return this.m_Status;
            }
            set
            {
                this.SetValue("Status", ref m_Status, value);
            }
        }

        private DateTime? m_CSAuditTimeFrom;
        public DateTime? CSAuditTimeFrom
        {
            get
            {
                return this.m_CSAuditTimeFrom;
            }
            set
            {
                this.SetValue("CSAuditTimeFrom", ref m_CSAuditTimeFrom, value);
            }
        }

        private DateTime? m_CSAuditTimeTo;
        public DateTime? CSAuditTimeTo
        {
            get
            {
                return this.m_CSAuditTimeTo;
            }
            set
            {
                this.SetValue("CSAuditTimeTo", ref m_CSAuditTimeTo, value);
            }
        }

        private DateTime? m_FinAuditTimeFrom;
        public DateTime? FinAuditTimeFrom
        {
            get
            {
                return this.m_FinAuditTimeFrom;
            }
            set
            {
                this.SetValue("FinAuditTimeFrom", ref m_FinAuditTimeFrom, value);
            }
        }

        private DateTime? m_FinAuditTimeTo;
        public DateTime? FinAuditTimeTo
        {
            get
            {
                return this.m_FinAuditTimeTo;
            }
            set
            {
                this.SetValue("FinAuditTimeTo", ref m_FinAuditTimeTo, value);
            }
        }

        private String m_ReferenceID;
        public String ReferenceID
        {
            get
            {
                return this.m_ReferenceID;
            }
            set
            {
                this.SetValue("ReferenceID", ref m_ReferenceID, value);
            }
        }

        private String m_WebChannelID;
        public String WebChannelID
        {
            get
            {
                return this.m_WebChannelID;
            }
            set
            {
                this.SetValue("WebChannelID", ref m_WebChannelID, value);
            }
        }

        #region 扩展属性

        public List<KeyValuePair<BalanceRefundStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<BalanceRefundStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public List<KeyValuePair<RefundPayType?, string>> RefundTypeList
        {
            get
            {
                var refundTypeList = EnumConverter.GetKeyValuePairs<RefundPayType>(EnumConverter.EnumAppendItemType.All);
                refundTypeList.RemoveAll(r => (r.Key != RefundPayType.PostRefund && r.Key != RefundPayType.BankRefund && r.Key != null));
                return refundTypeList;
            }
        }

        public List<WebChannelVM> WebChannelList
        {
            get
            {
                var webchannleList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
                webchannleList.Insert(0, new WebChannelVM()
                {
                    ChannelName = ResCommonEnum.Enum_All
                });
                return webchannleList;
            }
        }

        #endregion 扩展属性
    }
}