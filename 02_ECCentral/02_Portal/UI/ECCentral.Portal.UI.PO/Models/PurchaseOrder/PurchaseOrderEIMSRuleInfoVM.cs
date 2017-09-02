using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{

    public class PurchaseOrderEIMSRuleInfoVM : ModelBase
    {
        public PurchaseOrderEIMSRuleInfoVM()
        {
            m_RebateScheme = new EIMSRuleRebateSchemeVM();
            m_RebateSchemeTransactions = new List<EIMSRuleRebateSchemeTransactionVM>();
        }

        private Int32? m_RuleNumber;
        public Int32? RuleNumber
        {
            get { return this.m_RuleNumber; }
            set { this.SetValue("RuleNumber", ref m_RuleNumber, value); }
        }

        private String m_AssignedCode;
        public String AssignedCode
        {
            get { return this.m_AssignedCode; }
            set { this.SetValue("AssignedCode", ref m_AssignedCode, value); }
        }

        private String m_VendorNumber;
        public String VendorNumber
        {
            get { return this.m_VendorNumber; }
            set { this.SetValue("VendorNumber", ref m_VendorNumber, value); }
        }

        private String m_VendorName;
        public String VendorName
        {
            get { return this.m_VendorName; }
            set { this.SetValue("VendorName", ref m_VendorName, value); }
        }

        private String m_PM;
        public String PM
        {
            get { return this.m_PM; }
            set { this.SetValue("PM", ref m_PM, value); }
        }

        private String m_StockName;
        public String StockName
        {
            get { return this.m_StockName; }
            set { this.SetValue("StockName", ref m_StockName, value); }
        }

        private String m_RuleName;
        public String RuleName
        {
            get { return this.m_RuleName; }
            set { this.SetValue("RuleName", ref m_RuleName, value); }
        }

        private String m_Description;
        public String Description
        {
            get { return this.m_Description; }
            set { this.SetValue("Description", ref m_Description, value); }
        }

        private String m_DepartmentNumber;
        public String DepartmentNumber
        {
            get { return this.m_DepartmentNumber; }
            set { this.SetValue("DepartmentNumber", ref m_DepartmentNumber, value); }
        }

        private String m_EIMSType;
        public String EIMSType
        {
            get { return this.m_EIMSType; }
            set { this.SetValue("EIMSType", ref m_EIMSType, value); }
        }

        private Int32? m_EIMSTypeNumber;
        public Int32? EIMSTypeNumber
        {
            get { return this.m_EIMSTypeNumber; }
            set { this.SetValue("EIMSTypeNumber", ref m_EIMSTypeNumber, value); }
        }

        private PurchaseOrderEIMSRuleType? m_enumEIMSType;
        public PurchaseOrderEIMSRuleType? enumEIMSType
        {
            get { return this.m_enumEIMSType; }
            set { this.SetValue("enumEIMSType", ref m_enumEIMSType, value); }
        }

        private String m_ReceiveType;
        public String ReceiveType
        {
            get { return this.m_ReceiveType; }
            set { this.SetValue("ReceiveType", ref m_ReceiveType, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private String m_BillingCycle;
        public String BillingCycle
        {
            get { return this.m_BillingCycle; }
            set { this.SetValue("BillingCycle", ref m_BillingCycle, value); }
        }

        private EIMSRuleRebateSchemeVM m_RebateScheme;
        public EIMSRuleRebateSchemeVM RebateScheme
        {
            get { return this.m_RebateScheme; }
            set { this.SetValue("RebateScheme", ref m_RebateScheme, value); }
        }

        private List<EIMSRuleRebateSchemeTransactionVM> m_RebateSchemeTransactions;
        public List<EIMSRuleRebateSchemeTransactionVM> RebateSchemeTransactions
        {
            get { return this.m_RebateSchemeTransactions; }
            set { this.SetValue("RebateSchemeTransactions", ref m_RebateSchemeTransactions, value); }
        }

    }

    public class EIMSRuleRebateSchemeVM : ModelBase
    {
        private Int32? m_TransactionNumber;
        public Int32? TransactionNumber
        {
            get { return this.m_TransactionNumber; }
            set { this.SetValue("TransactionNumber", ref m_TransactionNumber, value); }
        }

        private Decimal? m_RebateAmount;
        public Decimal? RebateAmount
        {
            get { return this.m_RebateAmount; }
            set { this.SetValue("RebateAmount", ref m_RebateAmount, value); }
        }

        private Decimal? m_RebatePercentage;
        public Decimal? RebatePercentage
        {
            get { return this.m_RebatePercentage; }
            set { this.SetValue("RebatePercentage", ref m_RebatePercentage, value); }
        }

        private String m_RebateBaseType;
        public String RebateBaseType
        {
            get { return this.m_RebateBaseType; }
            set { this.SetValue("RebateBaseType", ref m_RebateBaseType, value); }
        }

        private String m_RebateSchemeType;
        public String RebateSchemeType
        {
            get { return this.m_RebateSchemeType; }
            set { this.SetValue("RebateSchemeType", ref m_RebateSchemeType, value); }
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

    }

    public class EIMSRuleRebateSchemeTransactionVM : ModelBase
    {
        private Decimal? m_Percent;
        public Decimal? Percent
        {
            get { return this.m_Percent; }
            set { this.SetValue("Percent", ref m_Percent, value); }
        }

        private Decimal? m_RebatePerUnit;
        public Decimal? RebatePerUnit
        {
            get { return this.m_RebatePerUnit; }
            set { this.SetValue("RebatePerUnit", ref m_RebatePerUnit, value); }
        }

        private Decimal? m_LowerLimitValue;
        public Decimal? LowerLimitValue
        {
            get { return this.m_LowerLimitValue; }
            set { this.SetValue("LowerLimitValue", ref m_LowerLimitValue, value); }
        }

        private Decimal? m_UpperLimitValue;
        public Decimal? UpperLimitValue
        {
            get { return this.m_UpperLimitValue; }
            set { this.SetValue("UpperLimitValue", ref m_UpperLimitValue, value); }
        }

        private String m_RebateBaseType;
        public String RebateBaseType
        {
            get { return this.m_RebateBaseType; }
            set { this.SetValue("RebateBaseType", ref m_RebateBaseType, value); }
        }

    }
}
