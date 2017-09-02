using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Invoice;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
namespace ECCentral.Portal.UI.SO.Models
{
    public class RefundInfoVM : ModelBase
    {
        private string m_BankName;
        /// <summary>
        /// ��������
        /// </summary>
        [Validate(ValidateType.Required)]
        public string BankName
        {
            get
            {
                return this.m_BankName;
            }
            set
            {
                this.SetValue("BankName", ref m_BankName, value);
            }
        }

        private string m_BranchBankName;
        /// <summary>
        /// ֧��
        /// </summary>
        [Validate(ValidateType.Required)]
        public string BranchBankName
        {
            get
            {
                return this.m_BranchBankName;
            }
            set
            {
                this.SetValue("BranchBankName", ref m_BranchBankName, value);
            }
        }

        private string m_CardNumber;
        /// <summary>
        /// ���п���
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CardNumber
        {
            get
            {
                return this.m_CardNumber;
            }
            set
            {
                this.SetValue("CardNumber", ref m_CardNumber, value);
            }
        }

        private string m_CardOwnerName;
        /// <summary>
        /// �ֿ���
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CardOwnerName
        {
            get
            {
                return this.m_CardOwnerName;
            }
            set
            {
                this.SetValue("CardOwnerName", ref m_CardOwnerName, value);
            }
        }

        private string m_PostAddress;
        /// <summary>
        /// ������ַ
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 100)]
        public string PostAddress
        {
            get
            {
                return this.m_PostAddress;
            }
            set
            {
                this.SetValue("PostAddress", ref m_PostAddress, value);
            }
        }

        private string m_PostCode;
        /// <summary>
        /// ��������
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d{6}$")]
        public string PostCode
        {
            get
            {
                return this.m_PostCode;
            }
            set
            {
                this.SetValue("PostCode", ref m_PostCode, value);
            }
        }

        private string m_CashReceiver;
        /// <summary>
        /// �տ���
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CashReceiver
        {
            get
            {
                return this.m_CashReceiver;
            }
            set
            {
                this.SetValue("CashReceiver", ref m_CashReceiver, value);
            }
        }

        private RefundPayType? m_RefundPayType;
        /// <summary>
        /// �˿�����
        /// </summary>
        [Validate(ValidateType.Required)]
        public RefundPayType? RefundPayType
        {
            get
            {
                return this.m_RefundPayType;
            }
            set
            {
                this.SetValue("RefundPayType", ref m_RefundPayType, value);
            }
        }

        private string m_RefundCashAmt;
        /// <summary>
        /// �˻��ֽ�
        /// </summary>
        public string RefundCashAmt
        {
            get
            {
                return this.m_RefundCashAmt;
            }
            set
            {
                this.SetValue("RefundCashAmt", ref m_RefundCashAmt, value);
            }
        }

        private string m_RefundMemo;
        /// <summary>
        /// �˿ע
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 200)]
        public string RefundMemo
        {
            get
            {
                return this.m_RefundMemo;
            }
            set
            {
                this.SetValue("RefundMemo", ref m_RefundMemo, value);
            }
        }

        private decimal? m_ToleranceAmt;
        /// <summary>
        /// ϵͳ��������
        /// </summary>
        public decimal? ToleranceAmt
        {
            get
            {
                return this.m_ToleranceAmt;
            }
            set
            {
                this.SetValue("ToleranceAmt", ref m_ToleranceAmt, value);
            }
        }

        private int? m_RefundReason;
        /// <summary>
        /// �˿�ԭ��
        /// </summary>
        public int? RefundReason
        {
            get { return m_RefundReason; }
            set
            {
                this.SetValue("RefundReason", ref m_RefundReason, value);
            }
        }

        /// <summary>
        /// �˿�ԭ���б�
        /// </summary>
        public List<KeyValuePair<RefundPayType?, string>> RefundPayTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<RefundPayType>();
            }
        }
    }
}
