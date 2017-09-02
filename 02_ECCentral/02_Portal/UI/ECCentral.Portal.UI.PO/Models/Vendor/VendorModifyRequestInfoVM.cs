using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorModifyRequestInfoVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_RequestSysNo;
        public Int32? RequestSysNo
        {
            get { return this.m_RequestSysNo; }
            set { this.SetValue("RequestSysNo", ref m_RequestSysNo, value); }
        }

        private VendorRank? m_Rank;
        public VendorRank? Rank
        {
            get { return this.m_Rank; }
            set { this.SetValue("Rank", ref m_Rank, value); }
        }

        private Int32 m_VendorSysNo;
        public Int32 VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        private string m_VendorName;
        public string VendorName
        {
            get { return this.m_VendorName; }
            set { this.SetValue("VendorName", ref m_VendorName, value); }
        }

        private VendorPayTermsItemInfoVM m_PayPeriodType;
        public VendorPayTermsItemInfoVM PayPeriodType
        {
            get { return this.m_PayPeriodType; }
            set { this.SetValue("PayPeriodType", ref m_PayPeriodType, value); }
        }

        private DateTime? m_ValidDate;
        public DateTime? ValidDate
        {
            get { return this.m_ValidDate; }
            set { this.SetValue("ValidDate", ref m_ValidDate, value); }
        }

        private DateTime? m_ExpiredDate;
        public DateTime? ExpiredDate
        {
            get { return this.m_ExpiredDate; }
            set { this.SetValue("ExpiredDate", ref m_ExpiredDate, value); }
        }

        private Decimal? m_ContractAmt;
        public Decimal? ContractAmt
        {
            get { return this.m_ContractAmt; }
            set { this.SetValue("ContractAmt", ref m_ContractAmt, value); }
        }

        private VendorModifyRequestStatus? m_Status;
        public VendorModifyRequestStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private String m_LanguageCode;
        public String LanguageCode
        {
            get { return this.m_LanguageCode; }
            set { this.SetValue("LanguageCode", ref m_LanguageCode, value); }
        }

        private Int32? m_CurrencySysNo;
        public Int32? CurrencySysNo
        {
            get { return this.m_CurrencySysNo; }
            set { this.SetValue("CurrencySysNo", ref m_CurrencySysNo, value); }
        }

        private String m_StoreCompanyCode;
        public String StoreCompanyCode
        {
            get { return this.m_StoreCompanyCode; }
            set { this.SetValue("StoreCompanyCode", ref m_StoreCompanyCode, value); }
        }

        private VendorModifyRequestType? m_RequestType;
        public VendorModifyRequestType? RequestType
        {
            get { return this.m_RequestType; }
            set { this.SetValue("RequestType", ref m_RequestType, value); }
        }

        private String m_AgentLevel;
        public String AgentLevel
        {
            get { return this.m_AgentLevel; }
            set { this.SetValue("AgentLevel", ref m_AgentLevel, value); }
        }

        private Int32? m_ManufacturerSysNo;
        public Int32? ManufacturerSysNo
        {
            get { return this.m_ManufacturerSysNo; }
            set { this.SetValue("ManufacturerSysNo", ref m_ManufacturerSysNo, value); }
        }

        private Int32? m_C2SysNo;
        public Int32? C2SysNo
        {
            get { return this.m_C2SysNo; }
            set { this.SetValue("C2SysNo", ref m_C2SysNo, value); }
        }

        private Int32? m_C3SysNo;
        public Int32? C3SysNo
        {
            get { return this.m_C3SysNo; }
            set { this.SetValue("C3SysNo", ref m_C3SysNo, value); }
        }

        private SettleType? m_SettleType;
        public SettleType? SettleType
        {
            get { return this.m_SettleType; }
            set { this.SetValue("SettleType", ref m_SettleType, value); }
        }

        private VendorSettlePeriodType? m_SettlePeriodType;
        public VendorSettlePeriodType? SettlePeriodType
        {
            get { return this.m_SettlePeriodType; }
            set { this.SetValue("SettlePeriodType", ref m_SettlePeriodType, value); }
        }

        private Decimal? m_SettlePercentage;
        public Decimal? SettlePercentage
        {
            get { return this.m_SettlePercentage; }
            set { this.SetValue("SettlePercentage", ref m_SettlePercentage, value); }
        }

        private String m_SendPeriod;
        public String SendPeriod
        {
            get { return this.m_SendPeriod; }
            set { this.SetValue("SendPeriod", ref m_SendPeriod, value); }
        }

        private Int32? m_BrandSysNo;
        public Int32? BrandSysNo
        {
            get { return this.m_BrandSysNo; }
            set { this.SetValue("BrandSysNo", ref m_BrandSysNo, value); }
        }

        private VendorModifyActionType? m_ActionType;
        public VendorModifyActionType? ActionType
        {
            get { return this.m_ActionType; }
            set { this.SetValue("ActionType", ref m_ActionType, value); }
        }

        private Int32? m_VendorManufacturerSysNo;
        public Int32? VendorManufacturerSysNo
        {
            get { return this.m_VendorManufacturerSysNo; }
            set { this.SetValue("VendorManufacturerSysNo", ref m_VendorManufacturerSysNo, value); }
        }

        private String m_Content;
        public String Content
        {
            get { return this.m_Content; }
            set { this.SetValue("Content", ref m_Content, value); }
        }

        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        private String m_BuyWeekDay;
        public String BuyWeekDay
        {
            get { return this.m_BuyWeekDay; }
            set { this.SetValue("BuyWeekDay", ref m_BuyWeekDay, value); }
        }

        /// <summary>
        /// ×Ô¶¯ÉóºË
        /// </summary>
        private bool? m_AutoAudit;
        public bool? AutoAudit
        {
            get { return this.m_AutoAudit; }
            set { this.SetValue("AutoAudit", ref m_AutoAudit, value); }
        }


    }
}
