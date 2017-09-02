using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class BalanceRefundVM : ModelBase
    {
        public int? SysNo
        {
            get;
            set;
        }

        public int? CustomerSysNo
        {
            get;
            set;
        }

        public string CustomerID
        {
            get;
            set;
        }

        public string CustomerName
        {
            get;
            set;
        }

        public BalanceRefundStatus? Status
        {
            get;
            set;
        }

        public decimal? ReturnPrepayAmt
        {
            get;
            set;
        }

        public string CreateUserName
        {
            get;
            set;
        }

        public DateTime? CreateTime
        {
            get;
            set;
        }

        public string CSAuditUserName
        {
            get;
            set;
        }

        public DateTime? CSAuditTime
        {
            get;
            set;
        }

        public string AuditUserName
        {
            get;
            set;
        }

        public DateTime? AuditTime
        {
            get;
            set;
        }

        private RefundPayType? m_RefundPayType;
        [Validate(ValidateType.Required)]
        public RefundPayType? RefundPayType
        {
            get
            {
                return m_RefundPayType;
            }
            set
            {
                base.SetValue("RefundPayType", ref m_RefundPayType, value);
            }
        }

        private string m_Note;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 500)]
        public string Note
        {
            get
            {
                return m_Note;
            }
            set
            {
                base.SetValue("Note", ref m_Note, value);
            }
        }

        private string m_BankName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 25)]
        public string BankName
        {
            get
            {
                return m_BankName;
            }
            set
            {
                base.SetValue("BankName", ref m_BankName, value);
            }
        }

        private string m_BranchBankName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 50)]
        public string BranchBankName
        {
            get
            {
                return m_BranchBankName;
            }
            set
            {
                base.SetValue("BranchBankName", ref m_BranchBankName, value);
            }
        }

        private string m_CardNumber;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 25)]
        public string CardNumber
        {
            get
            {
                return m_CardNumber;
            }
            set
            {
                base.SetValue("CardNumber", ref m_CardNumber, value);
            }
        }

        private string m_CardOwnerName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 50)]
        public string CardOwnerName
        {
            get
            {
                return m_CardOwnerName;
            }
            set
            {
                base.SetValue("CardOwnerName", ref m_CardOwnerName, value);
            }
        }

        private string m_PostAddress;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 150)]
        public string PostAddress
        {
            get
            {
                return m_PostAddress;
            }
            set
            {
                base.SetValue("PostAddress", ref m_PostAddress, value);
            }
        }

        private string m_PostCode;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.ZIP)]
        public string PostCode
        {
            get
            {
                return m_PostCode;
            }
            set
            {
                base.SetValue("PostCode", ref m_PostCode, value);
            }
        }

        private string m_ReceiverName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 25)]
        public string ReceiverName
        {
            get
            {
                return m_ReceiverName;
            }
            set
            {
                base.SetValue("ReceiverName", ref m_ReceiverName, value);
            }
        }

        private string m_ReferenceID;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 20)]
        public string ReferenceID
        {
            get
            {
                return m_ReferenceID;
            }
            set
            {
                base.SetValue("ReferenceID", ref m_ReferenceID, value);
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
                refundTypeList.RemoveAll(r => (r.Key != ECCentral.BizEntity.Invoice.RefundPayType.PostRefund
                    && r.Key != ECCentral.BizEntity.Invoice.RefundPayType.BankRefund && r.Key != null));
                return refundTypeList;
            }
        }

        #endregion 扩展属性
    }
}