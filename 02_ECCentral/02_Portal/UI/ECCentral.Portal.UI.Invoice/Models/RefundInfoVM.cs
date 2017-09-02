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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 退款信息，兼容netpay和postpay
    /// </summary>
    public class RefundInfoVM : ModelBase
    {
        private string m_BankName;
        /// <summary>
        /// 开户银行
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
        /// 支行
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
        /// 银行卡号
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
        /// 持卡人
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
        /// 邮政地址
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
        /// 邮政编码
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
        /// 收款人
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
        /// 退款类型
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
        /// 退还现金
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(0\.[0-9]{0,2})|(\d{0,9}?(\.\d{0,2})?)$")]
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
        /// 退款备注
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
        /// 系统精度冗余
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

        /// <summary>
        /// 退款类型列表
        /// </summary>
        public List<KeyValuePair<RefundPayType?, string>> RefundPayTypeList
        {
            get
            {
                var refundPayTypeList = EnumConverter.GetKeyValuePairs<RefundPayType>(EnumConverter.EnumAppendItemType.All);
                refundPayTypeList.RemoveAll(x => x.Key == ECCentral.BizEntity.Invoice.RefundPayType.TransferPointRefund);
                return refundPayTypeList;
            }
        }
    }
}