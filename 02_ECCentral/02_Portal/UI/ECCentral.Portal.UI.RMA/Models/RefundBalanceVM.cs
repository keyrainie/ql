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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Converters;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RefundBalanceVM : ModelBase
    {
        #region 查询得到的属性
        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }
        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }

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
        private RefundBalanceStatus? status;
        public RefundBalanceStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        private RefundStatus? auditStatus;
        public RefundStatus? AuditStatus
        {
            get { return auditStatus; }
            set
            {
                base.SetValue("AuditStatus", ref auditStatus, value);
            }
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
        #endregion

        #region 转化为界面展示用

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }
        public string StatusDesc
        {
            get
            {
                return this.Status.ToDescription();//EnumConverter.GetDescription(this.Status, typeof(RefundBalanceStatus));
            }
        }
        public string AuditStatusDesc
        {
            get
            {
                if (this.AuditStatus == null && this.CashAmt >= 0)
                {
                    return ResRefundBalance.WaitingAuditStatus;
                }
                else if (this.AuditStatus != null && this.CashAmt >= 0)
                {
                    return this.AuditStatus.ToDescription();// EnumConverter.GetDescription(this.AuditStatus, typeof(RefundStatus));
                }
                else
                {
                    return ResRefundBalance.UnneededAuditStatus;
                }
            }
        }
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
        #endregion
    }
}
