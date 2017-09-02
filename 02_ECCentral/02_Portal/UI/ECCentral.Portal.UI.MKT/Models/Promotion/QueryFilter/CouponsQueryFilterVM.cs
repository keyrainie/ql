using System;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.MKT.Models
{
    public class CouponsQueryFilterViewModel : ModelBase
    {
        private PagingInfo m_PageInfo;
        public PagingInfo PageInfo
        {
            get { return this.m_PageInfo; }
            set { this.SetValue("PageInfo", ref m_PageInfo, value); }
        }

        private String m_ChannelID="1";
        public String ChannelID
        {
            get { return this.m_ChannelID; }
            set { this.SetValue("ChannelID", ref m_ChannelID, value); }
        }

        private string m_CouponsSysNoFrom;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "必须是整数，且大于0")]
        public string CouponsSysNoFrom
        {
            get { return this.m_CouponsSysNoFrom; }
            set { this.SetValue("CouponsSysNoFrom", ref m_CouponsSysNoFrom, value); }
        }

        private string m_CouponsSysNoTo;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "必须是整数，且大于0")]
        public string CouponsSysNoTo
        {
            get { return this.m_CouponsSysNoTo; }
            set { this.SetValue("CouponsSysNoTo", ref m_CouponsSysNoTo, value); }
        }

        private String m_CouponsName;
        public String CouponsName
        {
            get { return this.m_CouponsName; }
            set { this.SetValue("CouponsName", ref m_CouponsName, value); }
        }

        private String m_CouponCode;
        public String CouponCode
        {
            get { return this.m_CouponCode; }
            set { this.SetValue("CouponCode", ref m_CouponCode, value); }
        }

        private CouponsStatus? m_Status;
        public CouponsStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private String m_CreateUser;
        public String CreateUser
        {
            get { return this.m_CreateUser; }
            set { this.SetValue("CreateUser", ref m_CreateUser, value); }
        }

        private String m_AuditUser;
        public String AuditUser
        {
            get { return this.m_AuditUser; }
            set { this.SetValue("AuditUser", ref m_AuditUser, value); }
        }

        private DateTime? m_CreateDateFrom;
        public DateTime? CreateDateFrom
        {
            get { return this.m_CreateDateFrom; }
            set { this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value); }
        }

        private DateTime? m_CreateDateTo;
        public DateTime? CreateDateTo
        {
            get { return this.m_CreateDateTo; }
            set { this.SetValue("CreateDateTo", ref m_CreateDateTo, value); }
        }

        private DateTime? m_BeginDate;
        public DateTime? BeginDate
        {
            get { return this.m_BeginDate; }
            set { this.SetValue("BeginDate", ref m_BeginDate, value); }
        }

        private DateTime? m_EndDate;
        public DateTime? EndDate
        {
            get { return this.m_EndDate; }
            set { this.SetValue("EndDate", ref m_EndDate, value); }
        }

        public bool HasCouponCodeApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CouponCode_Approve); }
        }

        public bool HasCouponCodeStopApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CouponCodeStop_Approve); }
        }

        public bool HasCouponCodeEditPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_CouponCode_Edit); }
        }

        private int? m_MerchantSysNo;
        public int? MerchantSysNo
        {
            get { return this.m_MerchantSysNo; }
            set { this.SetValue("MerchantSysNo", ref m_MerchantSysNo, value); }
        }
    }
}
