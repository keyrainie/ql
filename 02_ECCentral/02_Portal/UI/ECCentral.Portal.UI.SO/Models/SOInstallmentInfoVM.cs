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
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOInstallmentInfoVM : ModelBase
    {
        //private bool hasViewRight = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PrivilegeKey_SOInstalmentReView);
        //private bool hasEidtRight = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PrivilegeKey_SOInstalmentMaintain);
        //private bool hasSeeFullInstallmentContractNumber = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PrivilegeKey_CanSeeFullInstallmentContractNumber);

        private Int32 m_SysNo;
        public Int32 SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32 m_SOSysNo;
        public Int32 SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private String m_RealName;
        public String RealName
        {
            get { return this.m_RealName; }
            set { this.SetValue("RealName", ref m_RealName, value); }
        }

        private String m_IDNumber;
        public String IDNumber
        {
            get { return this.m_IDNumber; }
            set { this.SetValue("IDNumber", ref m_IDNumber, value); }
        }

        private String m_CreditCardNumber;
        public String CreditCardNumber
        {
            get { return this.m_CreditCardNumber; }
            set { this.SetValue("CreditCardNumber", ref m_CreditCardNumber, value); }
        }

        private String m_CreditCardNumberReal;
        public String CreditCardNumberReal
        {
            get { return this.m_CreditCardNumberReal; }
            set { this.SetValue("CreditCardNumberReal", ref m_CreditCardNumberReal, value); }
        }

        private String m_CreditCardNumberEnc;
        public String CreditCardNumberEnc
        {
            get { return this.m_CreditCardNumberEnc; }
            set { this.SetValue("CreditCardNumberEnc", ref m_CreditCardNumberEnc, value); }
        }

        private String m_ExpireDate;
        public String ExpireDate
        {
            get { return this.m_ExpireDate; }
            set { this.SetValue("ExpireDate", ref m_ExpireDate, value); }
        }

        private InstalmentStatus? m_Status;
        public InstalmentStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private Int32 m_BankSysNo;
        public Int32 BankSysNo
        {
            get { return this.m_BankSysNo; }
            set { this.SetValue("BankSysNo", ref m_BankSysNo, value); }
        }

        private String m_BankName;
        public String BankName
        {
            get { return this.m_BankName; }
            set { this.SetValue("BankName", ref m_BankName, value); }
        }

        private Int32 m_PhaseCount;
        public Int32 PhaseCount
        {
            get { return this.m_PhaseCount; }
            set { this.SetValue("PhaseCount", ref m_PhaseCount, value); }
        }

        private String m_ContractNumber;
        public String ContractNumber
        {
            get { return this.m_ContractNumber; }
            set { this.SetValue("ContractNumber", ref m_ContractNumber, value); }
        }

        private int? m_PayTypeSysNo;
        public int? PayTypeSysNo
        {
            get { return this.m_PayTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value); }
        }

        private string m_InUser;
        public string InUser
        {
            get { return this.m_InUser; }
            set { this.SetValue("InUser", ref m_InUser, value); }
        }

        private string m_EditUser;
        public string EditUser
        {
            get { return this.m_EditUser; }
            set { this.SetValue("EditUser", ref m_EditUser, value); }
        }

        private string m_ExpireMonth;
        public string ExpireMonth
        {
            get { return this.m_ExpireMonth; }
            set { this.SetValue("ExpireMonth", ref m_ExpireMonth, value); }
        }

        private string m_ExpireYear;
        public string ExpireYear
        {
            get { return this.m_ExpireYear; }
            set { this.SetValue("ExpireYear", ref m_ExpireYear, value); }
        }

    }
}
