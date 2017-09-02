using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.Refund
{
    /// <summary>
    /// 余额退款实体
    /// </summary>
    public class BalanceRefundInfo : IIdentity, ICompany
    {
        public int? CustomerSysNo
        {
            get;
            set;
        }

        public RefundPayType? RefundPayType
        {
            get;
            set;
        }

        public Decimal? ReturnPrepayAmt
        {
            get;
            set;
        }

        public string BankName
        {
            get;
            set;
        }

        public string BranchBankName
        {
            get;
            set;
        }

        public string CardNumber
        {
            get;
            set;
        }

        public string CardOwnerName
        {
            get;
            set;
        }

        public string PostAddress
        {
            get;
            set;
        }

        public string PostCode
        {
            get;
            set;
        }

        public string ReceiverName
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public BalanceRefundStatus? Status
        {
            get;
            set;
        }

        public string ReferenceID
        {
            get;
            set;
        }

        public string ChannelID
        {
            get;
            set;
        }

        public int? AuditUserSysNo
        {
            get;
            set;
        }

        public DateTime? AuditTime
        {
            get;
            set;
        }

        public int? CSAuditUserSysNo
        {
            get;
            set;
        }

        public DateTime? CSAuditTime
        {
            get;
            set;
        }

        public int? CreateUserSysNo
        {
            get;
            set;
        }

        #region IIdentity Members

        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members
    }
}