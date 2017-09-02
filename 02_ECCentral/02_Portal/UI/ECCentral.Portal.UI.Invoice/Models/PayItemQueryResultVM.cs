using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PayItemQueryResultVM : ModelBase
    {
        private List<PayItemVM> m_ResultList;
        public List<PayItemVM> ResultList
        {
            get
            {
                return m_ResultList.DefaultIfNull();
            }
            set
            {
                base.SetValue("ResultList", ref m_ResultList, value);
            }
        }

        public PayItemQueryStatisticVM Statistic
        {
            get;
            set;
        }

        public int TotalCount
        {
            get;
            set;
        }
    }

    public class PayItemVM : ModelBase
    {
        private bool m_IsChecked;
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        /// <summary>
        /// 应付款编号
        /// </summary>
        public int? PaySysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 付款单系统编号
        /// </summary>
        public int? PayItemSysNo
        {
            get;
            set;
        }

        public int? OrderSysNo
        {
            get;
            set;
        }

        public int? BatchNumber
        {
            get;
            set;
        }

        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }

        public int? CurrencySysNo
        {
            get;
            set;
        }

        public string CurrencyDesc
        {
            get;
            set;
        }

        public decimal? m_PayAmt;
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? PayAmt
        {
            get
            {
                return m_PayAmt;
            }
            set
            {
                base.SetValue("PayAmt", ref m_PayAmt, value);
                m_PayAmtForEdit = value.ToString();
            }
        }

        private string m_PayAmtForEdit;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^((((-)?[1-9][0-9]{0,9})|0)(\\.[0-9]{0,2})?)?$")]
        public string PayAmtForEdit
        {
            get
            {
                return m_PayAmtForEdit;
            }
            set
            {
                decimal payAmt;
                PayAmt = decimal.TryParse(m_PayAmtForEdit, out payAmt) ? payAmt : (decimal?)null;
                base.SetValue("PayAmtForEdit", ref m_PayAmtForEdit, value);
            }
        }

        public PayItemStyle? PayStyle
        {
            get;
            set;
        }

        public string VendorName
        {
            get;
            set;
        }

        public string VendorSysNo
        {
            get;
            set;
        }

        public PayItemStatus? Status
        {
            get;
            set;
        }

        public DateTime? CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 估计付款时间
        /// </summary>
        public DateTime? EstimatePayTime
        {
            get;
            set;
        }

        public DateTime? PayTime
        {
            get;
            set;
        }

        private string m_ReferenceID;
        /// <summary>
        /// 凭证号
        /// </summary>
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

        /// <summary>
        /// 应付款发票状态
        /// </summary>
        public PayableInvoiceStatus? InvoiceStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 应付款发票实际状态
        /// </summary>
        public PayableInvoiceFactStatus? InvoiceFactStatus
        {
            get;
            set;
        }

        public DateTime? InvoiceUpdateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 付款单对应的供应商是否已锁定
        /// </summary>
        public bool? IsVendorHolded
        {
            get;
            set;
        }

        public string UpdateInvoiceUserName
        {
            get;
            set;
        }

        private string m_Note;
        /// <summary>
        /// 注解
        /// </summary>
        [Validate(ValidateType.MaxLength, 200)]
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

        public string InvoiceNumber
        {
            get;
            set;
        }

        public decimal? ReturnPoint
        {
            get;
            set;
        }

        public string BankGLAccount
        {
            get;
            set;
        }

        public SapImportedStatus? SapImportedStatus
        {
            get;
            set;
        }//   -- 导入状态

        public DateTime? SAPPostDate
        {
            get;
            set;
        }// --导入时间

        public string SAPDocNo
        {
            get;
            set;
        }//    --导入凭证号

        //付款结算公司
        public string PaySettleCompany 
        { 
            get; 
            set; 
        }

        public string PaySettleCompanyStr
        {
            get
            {
                return ECCentral.Portal.Basic.Utilities.EnumConverter.GetDescription(this.PaySettleCompany, typeof(ECCentral.BizEntity.PO.PaySettleCompany));
            }
        }
    }

    public class PayItemQueryStatisticVM : ModelBase, IStatisticInfo
    {
        /// <summary>
        /// 所有合计
        /// </summary>
        public decimal AllPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 本页合计
        /// </summary>
        public decimal PagePayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 选中记录的金额
        /// </summary>
        public List<decimal> SelectedPayAmtList
        {
            get;
            set;
        }

        #region IStatisticInfo Members

        public string ToStatisticText()
        {
            int count = 0;
            decimal totalAmt = 0M;

            if (SelectedPayAmtList != null)
            {
                count = SelectedPayAmtList.Count;
                totalAmt = SelectedPayAmtList.Sum();
            }
            return string.Format(ECCentral.Portal.UI.Invoice.Resources.ResPayItemQuery.Message_StatisticInfo
                , ConstValue.Invoice_ToCurrencyString(PagePayAmt)
                , count
                , ConstValue.Invoice_ToCurrencyString(totalAmt)
                , ConstValue.Invoice_ToCurrencyString(AllPayAmt));
        }

        #endregion IStatisticInfo Members
    }
}
