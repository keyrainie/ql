using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IEmailDA))]
    public class EmailDA : IEmailDA
    {
        #region IEmailDA Members

        /// <summary>
        /// 发送异步邮件(外部邮件）
        /// </summary>
        /// <param name="mailInfo"></param>
        /// <returns></returns>
        public MailMessage InsertMail(MailMessage mailInfo)
        {
            string getEmailToName = mailInfo.ToName;
            string getEmailCCName = mailInfo.CCName;
            string getEmailBCCName = mailInfo.BCCName;

            DataCommand dc = DataCommandManager.GetDataCommand("InsertEmail");
            dc.SetParameterValue("@MailAddress", getEmailToName);
            dc.SetParameterValue("@MailSubject", mailInfo.Subject);
            dc.SetParameterValue("@MailBody", mailInfo.Body);
            dc.SetParameterValue("@Status", 0);
            dc.SetParameterValue("@CreateTime", DateTime.Now);
            dc.SetParameterValue("@SendTime", DBNull.Value);
            dc.SetParameterValue("@CCMailAddress", getEmailCCName);
            dc.SetParameterValue("@BCMailAddress", getEmailBCCName);
            dc.SetParameterValue("@MailFrom", mailInfo.FromName);
            dc.SetParameterValue("@MailSenderName", mailInfo.DisplaySenderName);
            dc.SetParameterValue("@CompanyCode", "8601");
            dc.SetParameterValue("@LanguageCode", "zh-CN");
            dc.SetParameterValue("@Priority", mailInfo.Priority);
            dc.ExecuteScalar<int>();
            return mailInfo;
        }


        /// <summary>
        /// 发送异步邮件(内部邮件)
        /// </summary>
        /// <param name="mailInfo"></param>
        /// <returns></returns>
        public MailMessage InsertInternalMail(MailMessage mailInfo)
        {
            string getEmailToName = mailInfo.ToName;
            string getEmailCCName = mailInfo.CCName;
            string getEmailBCCName = mailInfo.BCCName;

            DataCommand dc = DataCommandManager.GetDataCommand("InsertEmail_Internal");
            dc.SetParameterValue("@MailAddress", getEmailToName);
            dc.SetParameterValue("@CCMailAddress", getEmailCCName);
            dc.SetParameterValue("@BCMailAddress", getEmailBCCName);
            dc.SetParameterValue("@MailSubject", mailInfo.Subject);
            dc.SetParameterValue("@MailBody", mailInfo.Body);
            dc.SetParameterValue("@Status", 0);
            dc.SetParameterValue("@Priority", mailInfo.Priority);
            dc.SetParameterValue("@SendTime", DBNull.Value);
            dc.SetParameterValue("@CreateTime", DateTime.Now);
            dc.SetParameterValue("@CompanyCode", "8601");
            dc.SetParameterValue("@LanguageCode", "zh-CN");
            dc.SetParameterValue("@StoreCompanyCode", "8601");
            dc.ExecuteScalar<int>();
            return mailInfo;
        }

        #endregion

    }
}
