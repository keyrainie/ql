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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class RefundAdjustMaintainVM : ModelBase
    {
        /// <summary>
        /// 系统编号 
        /// </summary>
        public int? AdjustSysNo { get; set; }

        /// <summary>
        /// RMA申请单系统编号
        /// </summary>
        public int? RequestSysNo { get; set; }
        /// <summary>
        /// RMA申请单号
        /// </summary>
        private string requestId;
        public string RequestID
        {
            get { return requestId; }
            set { base.SetValue("RequestID", ref requestId, value); }
        }

        private RefundPayType? refundPayType;
        /// <summary>
        /// 退款类型
        /// </summary>
        [Validate(ValidateType.Required)]
        public RefundPayType? RefundPayType
        {
            get { return refundPayType; }
            set { base.SetValue("RefundPayType", ref refundPayType, value); }
        }

        public string soSysNo;
        /// <summary>
        /// SO系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return soSysNo; }
            set { base.SetValue("SOSysNo", ref soSysNo, value); }
        }

        public string customerID;
        /// <summary>
        /// 客户账号
        /// </summary>
        public string CustomerID
        {
            get { return customerID; }
            set { base.SetValue("CustomerID", ref customerID, value); }
        }

        private int? productSysNo;
        /// <summary>
        /// 用于选择类型的商品编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string cashAmt;
        /// <summary>
        /// 补偿金额
        /// </summary>
        public string CashAmt
        {
            get { return cashAmt; }
            set { base.SetValue("CashAmt", ref cashAmt, value); }
        }

        public RefundAdjustType? AdjustOrderType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        private string note;
        [Validate(ValidateType.Required)]
        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        public string Action { get; set; }

        public RefundAdjustStatus? Status { get; set; }

        #region 银行展示信息
        /// <summary>
        /// 开户银行
        /// </summary>
        private string bankName;
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
        #endregion

        #region 其他
        public int? CustomerSysNo { get; set; }

        public decimal? GiftCardAmt { get; set; }

        public int? PointAmt { get; set; }

        [Validate(ValidateType.Required)]
        //[Validate(ValidateType.Regex, @"^(0{1}|([1-9]\d{0,11}))(\.(\d){1,2})?$",ErrorMessageResourceName="Msg_IsNotMoney",ErrorMessageResourceType=typeof(ResRefundAdjust))]
        public string CashAmountShow
        {
            get
            {
                decimal point = 0m;
                if (decimal.TryParse(this.CashAmt, out point))
                {
                    return point.ToString("f2");
                }
                else
                    return this.CashAmt;
                
            }
            set { base.SetValue("CashAmountShow", ref cashAmt, value); }
        }

        public List<KeyValuePair<RefundPayType?, string>> RefundPayTypeList { get; set; }
        #endregion
    }
}
