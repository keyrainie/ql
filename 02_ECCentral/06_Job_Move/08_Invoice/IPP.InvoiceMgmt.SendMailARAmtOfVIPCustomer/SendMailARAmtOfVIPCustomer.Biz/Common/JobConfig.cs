using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SendMailARAmtOfVIPCustomer.Biz.Common
{
    public class JobConfig
    {
        /// <summary>
        /// 邮件主题模板
        /// </summary>
        public static string MailSubject
        {
            get { return ConfigurationManager.AppSettings["MailSubjectTemplate"]; }
        }

        /// <summary>
        /// 邮件正文模板
        /// </summary>
        public static string MailBody
        {
            get { return ConfigurationManager.AppSettings["MailBodyTemplate"]; }
        }

        /// <summary>
        /// 日期格式化字串
        /// </summary>
        public static string DateFormat
        {
            get { return ConfigurationManager.AppSettings["DateFormat"]; }
        }

        /// <summary>
        /// 邮件接收人地址列表
        /// </summary>
        public static string MailRecvAddress
        {
            get { return ConfigurationManager.AppSettings["MailRecvAddress"]; }
        }

        /// <summary>
        /// 邮件发送人地址
        /// </summary>
        public static string MailSendAddress
        {
            get { return ConfigurationManager.AppSettings["SendMailAddress"]; }
        }

        public static string CompanyCode
        {
            get { return ConfigurationManager.AppSettings["SendMailCompanyCode"]; }
        }
    }
}
