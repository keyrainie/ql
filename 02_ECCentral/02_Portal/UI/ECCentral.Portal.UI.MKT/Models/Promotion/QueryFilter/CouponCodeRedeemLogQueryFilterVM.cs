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
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter
{
    public class CouponCodeRedeemLogQueryFilterVM :ModelBase
    {
        public CouponCodeRedeemLogQueryFilterVM()
        {
            m_PageInfo = new PagingInfo();
        }
        private PagingInfo m_PageInfo;
        public PagingInfo PageInfo
        {
            get { return this.m_PageInfo; }
            set { this.SetValue("PageInfo", ref m_PageInfo, value); }
        }

        private string m_CouponBeginNo;
        [Validate(ValidateType.Interger)]
        public string CouponBeginNo
        {
            get { return this.m_CouponBeginNo; }
            set { this.SetValue("CouponBeginNo", ref m_CouponBeginNo, value); }
        }

        private string m_CouponEndNo;
        [Validate(ValidateType.Interger)]
        public string CouponEndNo
        {
            get { return this.m_CouponEndNo; }
            set { this.SetValue("CouponEndNo", ref m_CouponEndNo, value); }
        }

        private string m_CouponName;
        public string CouponName
        {
            get { return this.m_CouponName; }
            set { this.SetValue("CouponName", ref m_CouponName, value); }
        }
        private string m_CouponCode;
        public string CouponCode
        {
            get { return this.m_CouponCode; }
            set { this.SetValue("CouponCode", ref m_CouponCode, value); }
        }
        private string m_CustomerID;
        public string CustomerID
        {
            get { return this.m_CustomerID; }
            set { this.SetValue("CustomerID", ref m_CustomerID, value); }
        }
        private CouponCodeUsedStatus? m_CouponCodeStatus;
        public CouponCodeUsedStatus? CouponCodeStatus
        {
            get { return this.m_CouponCodeStatus; }
            set { this.SetValue("CouponCodeStatus", ref m_CouponCodeStatus, value); }
        }
        private string m_SOID;
        public string SOID
        {
            get { return this.m_SOID; }
            set { this.SetValue("SOID", ref m_SOID, value); }
        }
        private DateTime? m_BeginUseDate;
        public DateTime? BeginUseDate
        {
            get { return this.m_BeginUseDate; }
            set { this.SetValue("BeginUseDate", ref m_BeginUseDate, value); }
        }
        private DateTime? m_EndUseDate;
        public DateTime? EndUseDate
        {
            get { return this.m_EndUseDate; }
            set { this.SetValue("EndUseDate", ref m_EndUseDate, value); }
        }
    }
}
