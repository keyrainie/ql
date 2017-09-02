using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RefundVM : ModelBase
    {
        public RefundVM()
        {            
            this.RefundItems = new List<RefundItemVM>();
            this.RefundStatusList = EnumConverter.GetKeyValuePairs<RMARefundStatus>(EnumConverter.EnumAppendItemType.Select);
            this.RefundPayTypeList = EnumConverter.GetKeyValuePairs<RefundPayType>();            
            this.RefundPayTypeList.RemoveAll(p => p.Key == ECCentral.BizEntity.Invoice.RefundPayType.TransferPointRefund);
            this.RefundPayTypeList.RemoveAll(p => p.Key == ECCentral.BizEntity.Invoice.RefundPayType.GiftCardRefund);
            this.RefundPayType = ECCentral.BizEntity.Invoice.RefundPayType.PrepayRefund;
            this.Stocks = new List<StockInfo>();
            this.RefundReasons = new List<CodeNamePair>();            
        }       

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }
      
        private String m_RefundID;
        public String RefundID
        {
            get { return this.m_RefundID; }
            set { this.SetValue("RefundID", ref m_RefundID, value); }
        }

        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private Int32? m_CustomerSysNo;
        public Int32? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        private DateTime? m_AuditTime;
        public DateTime? AuditTime
        {
            get { return this.m_AuditTime; }
            set { this.SetValue("AuditTime", ref m_AuditTime, value); }
        }

        private Int32? m_AuditUserSysNo;
        public Int32? AuditUserSysNo
        {
            get { return this.m_AuditUserSysNo; }
            set { this.SetValue("AuditUserSysNo", ref m_AuditUserSysNo, value); }
        }

        private DateTime? m_RefundTime;
        public DateTime? RefundTime
        {
            get { return this.m_RefundTime; }
            set { this.SetValue("RefundTime", ref m_RefundTime, value); }
        }

        private Int32? m_RefundUserSysNo;
        public Int32? RefundUserSysNo
        {
            get { return this.m_RefundUserSysNo; }
            set { this.SetValue("RefundUserSysNo", ref m_RefundUserSysNo, value); }
        }

        private string m_CompensateShipPrice;
        [Validate(ValidateType.Regex, @"^\d{0,15}(\.\d+)?$")]
        public string CompensateShipPrice
        {
            get { return m_CompensateShipPrice; }
            set
            {
                this.SetValue("CompensateShipPrice", ref m_CompensateShipPrice, value);
            }
        }

        private Decimal? m_SOCashPointRate;
        public Decimal? SOCashPointRate
        {
            get { return this.m_SOCashPointRate; }
            set { this.SetValue("SOCashPointRate", ref m_SOCashPointRate, value); }
        }

        private Decimal? m_orgCashAmt;
        public Decimal? OrgCashAmt
        {
            get { return this.m_orgCashAmt; }
            set { this.SetValue("OrgCashAmt", ref m_orgCashAmt, value); }
        }

        private Decimal? m_OrgGiftCardAmt;
        public Decimal? OrgGiftCardAmt
        {
            get { return this.m_OrgGiftCardAmt; }
            set { this.SetValue("OrgGiftCardAmt", ref m_OrgGiftCardAmt, value); }
        }

        private Decimal? m_GiftCardAmt;
        public Decimal? GiftCardAmt
        {
            get { return this.m_GiftCardAmt; }
            set { this.SetValue("GiftCardAmt", ref m_GiftCardAmt, value); }
        }

        private Int32? m_orgPointAmt;
        public Int32? OrgPointAmt
        {
            get { return this.m_orgPointAmt; }
            set { this.SetValue("OrgPointAmt", ref m_orgPointAmt, value); }
        }

        private Int32? m_DeductPointFromAccount;
        public Int32? DeductPointFromAccount
        {
            get { return this.m_DeductPointFromAccount; }
            set { this.SetValue("DeductPointFromAccount", ref m_DeductPointFromAccount, value); }
        }

        private Decimal? m_DeductPointFromCurrentCash;
        public Decimal? DeductPointFromCurrentCash
        {
            get { return this.m_DeductPointFromCurrentCash; }
            set { this.SetValue("DeductPointFromCurrentCash", ref m_DeductPointFromCurrentCash, value); }
        }

        private Decimal? m_CashAmt;
        public Decimal? CashAmt
        {
            get { return this.m_CashAmt; }
            set { this.SetValue("CashAmt", ref m_CashAmt, value); }
        }

        private Int32? m_PointAmt;
        public Int32? PointAmt
        {
            get { return this.m_PointAmt; }
            set { this.SetValue("PointAmt", ref m_PointAmt, value); }
        }

        private RefundPayType? m_RefundPayType;
        public RefundPayType? RefundPayType
        {
            get { return this.m_RefundPayType; }
            set { this.SetValue("RefundPayType", ref m_RefundPayType, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private RMARefundStatus? m_Status;
        public RMARefundStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private CashFlagStatus? m_CashFlag;
        public CashFlagStatus? CashFlag
        {
            get { return this.m_CashFlag; }
            set { this.SetValue("CashFlag", ref m_CashFlag, value); }
        }

        private String m_FinanceNote;        
        public String FinanceNote
        {
            get { return this.m_FinanceNote; }
            set { this.SetValue("FinanceNote", ref m_FinanceNote, value); }
        }

        private String m_InvoiceLocation;
        public String InvoiceLocation
        {
            get { return this.m_InvoiceLocation; }
            set { this.SetValue("InvoiceLocation", ref m_InvoiceLocation, value); }
        }

        private String m_SOInvoiceNo;
        public String SOInvoiceNo
        {
            get { return this.m_SOInvoiceNo; }
            set { this.SetValue("SOInvoiceNo", ref m_SOInvoiceNo, value); }
        }

        private string invoiceNo;
		public string InvoiceNo 
		{ 
			get
			{
				return invoiceNo;
			}			
			set
			{
				SetValue("InvoiceNo", ref invoiceNo, value);
			} 
		}
		
        private Int32? m_RefundReason;
        [Validate(ValidateType.Required)]
        public Int32? RefundReason
        {
            get { return this.m_RefundReason; }
            set { this.SetValue("RefundReason", ref m_RefundReason, value); }
        }

        private Boolean? m_CheckIncomeStatus;
        public Boolean? CheckIncomeStatus
        {
            get { return this.m_CheckIncomeStatus; }
            set { this.SetValue("CheckIncomeStatus", ref m_CheckIncomeStatus, value); }
        }

        private List<RefundItemVM> m_RefundItems;
        public List<RefundItemVM> RefundItems
        {
            get { return this.m_RefundItems; }
            set { this.SetValue("RefundItems", ref m_RefundItems, value); }
        }

        private Boolean m_HasPriceprotectPoint;
        public Boolean HasPriceprotectPoint
        {
            get { return this.m_HasPriceprotectPoint; }
            set { this.SetValue("HasPriceprotectPoint", ref m_HasPriceprotectPoint, value); }
        }

        private Int32? m_PriceprotectPoint;
        public Int32? PriceprotectPoint
        {
            get { return this.m_PriceprotectPoint; }
            set { this.SetValue("PriceprotectPoint", ref m_PriceprotectPoint, value); }
        }       	

        #region 扩展信息
        public Visibility PromotionSOVisible
        {
            get
            {
                return string.IsNullOrEmpty(PromotionSO) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string PromotionSO
        {
            get
            {
                if (CouponCodeLog != null && CouponCodeLog.UsedOrderSysNo != null)
                {                    
                    return string.Format(ResRefundQuery.Msg_UsedOrderSysNo, this.SOSysNo, 
                        this.CouponCodeLog.PromotionCode, this.CouponCodeLog.PromotionAmountType, this.CouponCodeLog.UsedOrderSysNo);
                }
                return string.Empty;
            }
        }
        private bool isRelateCash;
        /// <summary>
        /// 是否涉及现金
        /// </summary>
		public bool IsRelateCash 
		{ 
			get
			{
                return isRelateCash;
			}			
			set
			{
                SetValue("IsRelateCash", ref isRelateCash, value);

                this.CashFlag = value ? CashFlagStatus.Yes : CashFlagStatus.No;                
			} 
		}
        /// <summary>
        /// 特殊订单描述
        /// </summary>
        public string SpecialSOTypeDesc { get; set; }
        public PromotionCode_Customer_Log CouponCodeLog { get; set; }
        public string CustomerName { get; set; }
        public List<StockInfo> Stocks { get; set; }
        public List<CodeNamePair> RefundReasons { get; set; }
        public List<KeyValuePair<RMARefundStatus?, string>> RefundStatusList { get; set; }
        public List<KeyValuePair<RefundPayType?, string>> RefundPayTypeList { get; set; }
        #endregion

        #region 银行退款单信息
        private string bankName;
        [Validate(ValidateType.Required)]
        public string BankName
        {
            get
            {
                return bankName;
            }
            set
            {
                SetValue("BankName", ref bankName, value);
            }
        }

        private string branchBankName;
        [Validate(ValidateType.Required)]
        public string BranchBankName
        {
            get
            {
                return branchBankName;
            }
            set
            {
                SetValue("BranchBankName", ref branchBankName, value);
            }
        }

        private string cardNumber;
        [Validate(ValidateType.Required)]
        public string CardNumber
        {
            get
            {
                return cardNumber;
            }
            set
            {
                SetValue("CardNumber", ref cardNumber, value);
            }
        }

        private string cardOwnerName;
        [Validate(ValidateType.Required)]
        public string CardOwnerName
        {
            get
            {
                return cardOwnerName;
            }
            set
            {
                SetValue("CardOwnerName", ref cardOwnerName, value);
            }
        }

        private string postAddredd;
        [Validate(ValidateType.Required)]
        public string PostAddress
        {
            get
            {
                return postAddredd;
            }
            set
            {
                SetValue("PostAddress", ref postAddredd, value);
            }
        }

        private string postCode;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.ZIP)]
        public string PostCode
        {
            get
            {
                return postCode;
            }
            set
            {
                SetValue("PostCode", ref postCode, value);
            }
        }

        private string receiverName;
        [Validate(ValidateType.Required)]
        public string ReceiverName
        {
            get
            {
                return receiverName;
            }
            set
            {
                SetValue("ReceiverName", ref receiverName, value);
            }
        }	

        private string incomeNote;
        [Validate(ValidateType.Required)]
		public string IncomeNote 
		{ 
			get
			{
				return incomeNote;
			}			
			set
			{
				SetValue("IncomeNote", ref incomeNote, value);
			} 
		}
		
        #endregion
    }   
}
