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
using System.Collections.Generic;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RefundBalanceMaintainVM : ModelBase
    {
        public RefundBalanceMaintainVM()
        {
            this.RefundPayTypeList = EnumConverter.GetKeyValuePairs<RefundPayType>();
            this.RefundPayTypeList.RemoveAll(p => p.Key == ECCentral.BizEntity.Invoice.RefundPayType.TransferPointRefund);
            this.RefundPayTypeList.RemoveAll(p => p.Key == ECCentral.BizEntity.Invoice.RefundPayType.GiftCardRefund);
        }
        /// <summary>
        /// 系统编号 
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 原始的退款单系统编号--用于创建退款调整单查询数据
        /// </summary>
        public int? OriginalRefundSysNo { get; set; }

        #region 退款单展示信息
        /// <summary>
        /// 退款单号
        /// </summary>
        public string RefundID { get; set; }
        
        private RefundPayType? refundPayType;
        /// <summary>
        /// 退款类型
        /// </summary>
        public RefundPayType? RefundPayType
        {
            get { return refundPayType; }
            set { base.SetValue("RefundPayType", ref refundPayType, value); }
        }
        /// <summary>
        /// SO系统编号
        /// </summary>
        public int? OriginalSOSysNo { get; set; }

        /// <summary>
        /// 客户账号
        /// </summary>
        public string CustomerID { get; set; }

        private int? productSysNo;
        /// <summary>
        /// 用于选择调整类型的商品编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private decimal? cashAmt;
        /// <summary>
        /// 调整金额
        /// </summary>
        public decimal? CashAmt
        {
            get { return cashAmt; }
            set { base.SetValue("CashAmt", ref cashAmt, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string note;
        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }
        
        public RefundBalanceStatus? Status { get; set; }
        #endregion

        #region 银行展示信息
        /// <summary>
        /// 开户银行
        /// </summary>
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
        /// <summary>
        /// 银行支行
        /// </summary>
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
        /// <summary>
        /// 银行卡号
        /// </summary>
        private string cardNo;
        [Validate(ValidateType.Required)]
        public string CardNo
        {
            get
            {
                return cardNo;
            }
            set
            {
                SetValue("CardNo", ref cardNo, value);
            }
        }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
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
        /// <summary>
        /// 邮政汇款地址
        /// </summary>
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
        /// <summary>
        /// 邮政编码
        /// </summary>
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
        /// <summary>
        /// 收款人姓名
        /// </summary>
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
        /// <summary>
        /// 备注
        /// </summary>
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

        #region 其他
        public int? CustomerSysNo { get; set; }

        public decimal? GiftCardAmt { get; set; }

        public int? PointAmt { get; set; }

        public string CashAmountShow
        {
            get
            {
                decimal point = (this.PointAmt ?? 0);
                decimal total= (this.CashAmt ?? 0m) + (this.GiftCardAmt ?? 0m) + (point / 10);
                if (total == 0)
                {
                    return String.Empty;
                }
                return total.ToString("f2");
            }
            //set { base.SetValue("CashAmt", ref cashAmt, value); }
        }
         
        public List<KeyValuePair<RefundPayType?, string>> RefundPayTypeList { get; set; }
        #endregion
    }
}
