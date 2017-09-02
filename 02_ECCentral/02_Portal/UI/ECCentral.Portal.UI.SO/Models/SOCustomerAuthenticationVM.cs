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
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOCustomerAuthenticationVM : ModelBase
    {
        public SOCustomerAuthenticationVM()
        {
            this.GenderList = EnumConverter.GetKeyValuePairs<Gender>(EnumConverter.EnumAppendItemType.None);
            this.IDCardTypeList = EnumConverter.GetKeyValuePairs<IDCardType>(EnumConverter.EnumAppendItemType.None);
        }

        private int? m_SysNo;
        public int? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private int? m_SOSysNo;
        public int? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private int? m_CustomerSysNo;
        public int? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        public string m_Name;
        public string Name
        {
            get { return this.m_Name; }
            set { this.SetValue("Name", ref m_Name, value); }
        }

        private IDCardType? m_IDCardType;
        public IDCardType? IDCardType
        {
            get { return this.m_IDCardType; }
            set { this.SetValue("IDCardType", ref m_IDCardType, value); }
        }

        private string m_IDCardNumber;
        public string IDCardNumber
        {
            get { return this.m_IDCardNumber; }
            set { this.SetValue("IDCardNumber", ref m_IDCardNumber, value); }
        }

        private string m_Birthday;
        public string Birthday
        {
            get { return this.m_Birthday; }
            set { this.SetValue("Birthday", ref m_Birthday, value); }
        }

        private string m_PhoneNumber;
        public string PhoneNumber
        {
            get { return this.m_PhoneNumber; }
            set { this.SetValue("PhoneNumber", ref m_PhoneNumber, value); }
        }

        private string m_Email;
        public string Email
        {
            get { return this.m_Email; }
            set { this.SetValue("Email", ref m_Email, value); }
        }


        private string m_Address;
        public string Address
        {
            get { return this.m_Address; }
            set { this.SetValue("Address", ref m_Address, value); }
        }

        private Gender? m_Gender;
        public Gender? Gender
        {
            get { return this.m_Gender; }
            set { this.SetValue("Gender", ref m_Gender, value); }
        }

        public List<KeyValuePair<Gender?, string>> GenderList { get; set; }

        public List<KeyValuePair<IDCardType?, string>> IDCardTypeList { get; set; }
    }
}
