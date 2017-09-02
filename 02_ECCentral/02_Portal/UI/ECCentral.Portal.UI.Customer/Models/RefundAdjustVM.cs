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
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class RefundAdjustVM : ModelBase
    {
        #region 查询得到的属性
        private int adjustSysNo;
        public int AdjustSysNo
        {
            get { return adjustSysNo; }
            set
            {
                base.SetValue("AdjustSysNo", ref adjustSysNo, value);
            }
        }
        public int? RequestSysNo { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string SOSysNo { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public string RequestID { get; set; }

        public string CustomerID { get; set; }

        public int? CustomerSysNo { get; set; }

        private decimal? cashAmt;
        public decimal? CashAmt
        {
            get { return cashAmt; }
            set { base.SetValue("CashAmt", ref cashAmt, value); }
        }

        private decimal? giftCardAmt;
        public decimal? GiftCardAmt
        {
            get { return giftCardAmt; }
            set { base.SetValue("GiftCardAmt", ref giftCardAmt, value); }
        }
        private int? pointAmt;
        public int? PointAmt
        {
            get { return pointAmt; }
            set { base.SetValue("PointAmt", ref pointAmt, value); }
        }
        private string note;
        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }
        private RefundAdjustStatus? status;
        public RefundAdjustStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        private RefundPayType refundPayType;
        public RefundPayType RefundPayType
        {
            get { return refundPayType; }
            set
            {
                base.SetValue("RefundPayType", ref refundPayType, value);
            }
        }

        private RefundAdjustType? adjustType;
        public RefundAdjustType? AdjustOrderType
        {
            get { return adjustType; }
            set
            {
                base.SetValue("AdjustOrderType", ref adjustType, value);
            }
        }

        public string StatusDesc
        {
            get
            {
                switch (Status)
                {
                    case RefundAdjustStatus.Abandon:
                        return "已作废";
                    case RefundAdjustStatus.Audited:
                        return "审核通过";
                    case RefundAdjustStatus.AuditRefuesed:
                        return "审核拒绝";
                    case RefundAdjustStatus.Initial:
                        return "已创建";
                    case RefundAdjustStatus.WaitingAudit:
                        return "待审核";
                    case RefundAdjustStatus.Refunded:
                        return "已退款";
                    default:
                        return string.Empty;
                }
            }
        }

        public string RefundPayTypeDesc
        {
            get
            {
                switch (RefundPayType)
                {
                    case RefundPayType.BankRefund:
                        return "银行转账";
                    case RefundPayType.NetWorkRefund:
                        return "网关直接退款";
                    case BizEntity.Invoice.RefundPayType.PrepayRefund:
                        return "退入余额账户";
                    default:
                        return "--";
                }
            }
        }

        public string AdjustTypeDesc
        {
            get
            {
                switch (AdjustOrderType)
                {
                    case RefundAdjustType.ShippingAdjust:
                        return "运费补偿";
                    case RefundAdjustType.Other:
                        return "其他";
                    case RefundAdjustType.EnergySubsidy:
                        return "节能补贴";
                    default:
                        return "--";
                }
            }
        }

        private int? createUserSysNo;

        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private string createUserName;
        public string CreateUserName
        {
            get { return createUserName; }
            set { base.SetValue("CreateUserName", ref createUserName, value); }
        }
        private DateTime? createTime;
        public DateTime? CreateTime
        {
            get { return createTime; }
            set { base.SetValue("CreateTime", ref createTime, value); }
        }
        private string refundUserName;
        public string RefundUserName
        {
            get { return refundUserName; }
            set { base.SetValue("RefundUserName", ref refundUserName, value); }
        }
        private DateTime? refundTime;
        public DateTime? RefundTime
        {
            get { return refundTime; }
            set { base.SetValue("RefundTime", ref refundTime, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get
            {
                return this.m_CompanyCode;
            }
            set
            {
                this.SetValue("CompanyCode", ref m_CompanyCode, value);
            }
        }
        #endregion

        #region 转化为界面展示用

        //private bool isChecked;
        //public bool IsChecked
        //{
        //    get { return isChecked; }
        //    set
        //    {
        //        base.SetValue("IsChecked", ref isChecked, value);
        //    }
        //}
        public int PointAmtDesc
        {
            get
            {
                this.PointAmt = this.PointAmt ?? 0;
                return this.PointAmt.Value;
            }
        }

        public decimal GiftCardAmtDesc
        {
            get
            {
                this.GiftCardAmt = this.GiftCardAmt ?? 0.00m;
                return this.GiftCardAmt.Value;
            }
        }

        public string CreateUserAndCreateTime
        {
            get { return this.CreateUserName + "[" + this.CreateTime + "]"; }
        }

        public string RefundUserAndRundTime
        {
            get
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(this.RefundUserName))
                    result = this.RefundUserName;
                if (this.RefundTime != null)
                    return result + "[" + this.RefundTime + "]";
                else
                    return string.Empty;
            }
        }
        #endregion

        #region 银行信息

        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// 支行
        /// </summary>
        public string BranchBankName
        {
            get;
            set;
        }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 持卡人
        /// </summary>
        public string CardOwnerName
        {
            get;
            set;
        }

        #endregion 银行信息
    }
}
