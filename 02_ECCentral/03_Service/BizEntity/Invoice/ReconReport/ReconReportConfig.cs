using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ECCentral.BizEntity.Invoice.ReconReport
{
    public class Email
    {
        private string m_MailSubject = string.Empty;
        private string m_MailFrom = string.Empty;
        private string m_MailTo = string.Empty;
        private string m_MailCC = string.Empty;
        private string m_MailBody = string.Empty;

        public Email(string date)
        {
            m_MailSubject = ConfigurationSettings.AppSettings["MailSubject"] + "(" + date + ")";
            m_MailFrom = ConfigurationSettings.AppSettings["MailFrom"];
            m_MailTo = ConfigurationSettings.AppSettings["MailTo"];
        }

        public string MailSubject
        {
            get { return m_MailSubject; }
            set { m_MailSubject = value; }
        }
        public string MailFrom
        {
            get { return m_MailFrom; }
            set { m_MailFrom = value; }
        }
        public string MailTo
        {
            get { return m_MailTo; }
            set { m_MailTo = value; }
        }
        public string MailCC
        {
            get { return m_MailCC; }
            set { m_MailCC = value; }
        }
        public string MailBody
        {
            get { return m_MailBody; }
            set { m_MailBody = value; }
        }
    }

    public class ReconReportConfig
    {
        public static string BasicDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public enum CreateType
        {
            /// <summary>
            /// Job
            /// </summary>
            Job = 0,
            /// <summary>
            /// Web
            /// </summary>
            Web = 1
        }

    }

    public class FTP
    {
        public static string Address = ConfigurationSettings.AppSettings["Address"];
        public static string UserName = ConfigurationSettings.AppSettings["UserName"];
        public static string Password = ConfigurationSettings.AppSettings["Password"];
    }
}
