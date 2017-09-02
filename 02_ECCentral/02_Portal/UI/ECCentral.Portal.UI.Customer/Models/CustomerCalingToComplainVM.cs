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

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerCalingToComplainVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private String m_ComplainID;
        public String ComplainID
        {
            get { return this.m_ComplainID; }
            set { this.SetValue("ComplainID", ref m_ComplainID, value); }
        }

        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private String m_ComplainType;
        public String ComplainType
        {
            get { return this.m_ComplainType; }
            set { this.SetValue("ComplainType", ref m_ComplainType, value); }
        }

        private String m_ComplainSourceType;
        public String ComplainSourceType
        {
            get { return this.m_ComplainSourceType; }
            set { this.SetValue("ComplainSourceType", ref m_ComplainSourceType, value); }
        }

        private String m_Subject;
        [Validate(ValidateType.Required)]
        public String Subject
        {
            get { return this.m_Subject; }
            set { this.SetValue("Subject", ref m_Subject, value); }
        }

        private Int32? m_CustomerSysNo;
        public Int32? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        private String m_CustomerEmail;
        public String CustomerEmail
        {
            get { return this.m_CustomerEmail; }
            set { this.SetValue("CustomerEmail", ref m_CustomerEmail, value); }
        }

        private String m_CustomerPhone;
        public String CustomerPhone
        {
            get { return this.m_CustomerPhone; }
            set { this.SetValue("CustomerPhone", ref m_CustomerPhone, value); }
        }

        private String m_ComplainContent;
        public String ComplainContent
        {
            get { return this.m_ComplainContent; }
            set { this.SetValue("ComplainContent", ref m_ComplainContent, value); }
        }

        private DateTime? m_ComplainTime;
        public DateTime? ComplainTime
        {
            get { return this.m_ComplainTime; }
            set { this.SetValue("ComplainTime", ref m_ComplainTime, value); }
        }
        public int CallsEventSysNo { get; set; }
    }
}
