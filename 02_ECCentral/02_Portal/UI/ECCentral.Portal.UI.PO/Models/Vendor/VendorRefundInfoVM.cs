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
using ECCentral.BizEntity.PO;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorRefundInfoVM : ModelBase
    {

        public VendorRefundInfoVM()
        {
            m_ItemList = new List<VendorRefundItemInfoVM>();
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private Int32? m_VendorSysNo;
        public Int32? VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        private String m_VendorName;
        public String VendorName
        {
            get { return this.m_VendorName; }
            set { this.SetValue("VendorName", ref m_VendorName, value); }
        }

        private Decimal? m_RefundCashAmt;
        public Decimal? RefundCashAmt
        {
            get { return this.m_RefundCashAmt; }
            set { this.SetValue("RefundCashAmt", ref m_RefundCashAmt, value); }
        }

        private VendorRefundPayType? m_PayType;
        public VendorRefundPayType? PayType
        {
            get { return this.m_PayType; }
            set { this.SetValue("PayType", ref m_PayType, value); }
        }

        private DateTime? m_CreateTime;
        public DateTime? CreateTime
        {
            get { return this.m_CreateTime; }
            set { this.SetValue("CreateTime", ref m_CreateTime, value); }
        }

        private Int32? m_CreateUserSysNo;
        public Int32? CreateUserSysNo
        {
            get { return this.m_CreateUserSysNo; }
            set { this.SetValue("CreateUserSysNo", ref m_CreateUserSysNo, value); }
        }

        private string m_CreateUserName;

        public string CreateUserName
        {
            get { return m_CreateUserName; }
            set { this.SetValue("CreateUserName", ref m_CreateUserName, value); }
        }


        private Int32? m_AbandonUserSysNo;
        public Int32? AbandonUserSysNo
        {
            get { return this.m_AbandonUserSysNo; }
            set { this.SetValue("AbandonUserSysNo", ref m_AbandonUserSysNo, value); }
        }

        private DateTime? m_AbandonTime;
        public DateTime? AbandonTime
        {
            get { return this.m_AbandonTime; }
            set { this.SetValue("AbandonTime", ref m_AbandonTime, value); }
        }

        private DateTime? m_PMAuditTime;
        public DateTime? PMAuditTime
        {
            get { return this.m_PMAuditTime; }
            set { this.SetValue("PMAuditTime", ref m_PMAuditTime, value); }
        }

        private Int32? m_PMUserSysNo;
        public Int32? PMUserSysNo
        {
            get { return this.m_PMUserSysNo; }
            set { this.SetValue("PMUserSysNo", ref m_PMUserSysNo, value); }
        }

        private string m_PMUserName;

        public string PMUserName
        {
            get { return m_PMUserName; }
            set { this.SetValue("PMUserName", ref m_PMUserName, value); }
        }


        private DateTime? m_PMDAuditTime;
        public DateTime? PMDAuditTime
        {
            get { return this.m_PMDAuditTime; }
            set { this.SetValue("PMDAuditTime", ref m_PMDAuditTime, value); }
        }

        private Int32? m_PMDUserSysNo;
        public Int32? PMDUserSysNo
        {
            get { return this.m_PMDUserSysNo; }
            set { this.SetValue("PMDUserSysNo", ref m_PMDUserSysNo, value); }
        }

        private string m_PMDUserName;

        public string PMDUserName
        {
            get { return m_PMDUserName; }
            set { this.SetValue("PMDUserName", ref m_PMDUserName, value); }
        }

        private DateTime? m_PMCCAuditTime;

        public DateTime? PMCCAuditTime
        {
            get { return m_PMCCAuditTime; }
            set { this.SetValue("PMCCAuditTime", ref m_PMCCAuditTime, value); }
        }


        private int? m_PMCCUserSysNo;

        public int? PMCCUserSysNo
        {
            get { return m_PMCCUserSysNo; }
            set { this.SetValue("PMCCUserSysNo", ref m_PMCCUserSysNo, value); }
        }

        private string m_PMCCUserName;

        public string PMCCUserName
        {
            get { return m_PMCCUserName; }
            set { this.SetValue("PMCCUserName", ref m_PMCCUserName, value); }
        }

        private String m_PMMemo;
        public String PMMemo
        {
            get { return this.m_PMMemo; }
            set { this.SetValue("PMMemo", ref m_PMMemo, value); }
        }

        private String m_PMDMemo;
        public String PMDMemo
        {
            get { return this.m_PMDMemo; }
            set { this.SetValue("PMDMemo", ref m_PMDMemo, value); }
        }

        private string m_PMCCMemo;

        public string PMCCMemo
        {
            get { return m_PMCCMemo; }
            set { this.SetValue("PMCCMemo", ref m_PMCCMemo, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private VendorRefundStatus? m_Status;
        public VendorRefundStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private List<VendorRefundItemInfoVM> m_ItemList;
        public List<VendorRefundItemInfoVM> ItemList
        {
            get { return this.m_ItemList; }
            set { this.SetValue("ItemList", ref m_ItemList, value); }
        }

        public string UserRole { get; set; }

        private bool? notPMAndPMD;

        public bool? NotPMAndPMD
        {
            get { return notPMAndPMD; }
            set { this.SetValue("NotPMAndPMD", ref notPMAndPMD, value); }
        }

    }
}
