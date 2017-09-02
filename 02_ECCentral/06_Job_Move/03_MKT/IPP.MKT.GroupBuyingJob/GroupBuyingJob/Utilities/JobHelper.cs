using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities;

namespace IPP.MktToolMgmt.GroupBuyingJob
{
    public static class JobHelper
    {
        public static void SendMail(string body)
        {
            if (ConfigurationManager.AppSettings["SendMailFlag"].ToString().ToUpper() == "FALSE")
            {
                return;
            }
            MailEntity mail = new MailEntity();
            mail.Body = body;
            mail.From = ConfigurationManager.AppSettings["EmailFrom"];
            mail.To = ConfigurationManager.AppSettings["EmailTo"];
            mail.CC = ConfigurationManager.AppSettings["EmailCC"];

            if (String.IsNullOrEmpty(mail.To))
            {
                return;
            }
            mail.Subject = "团购Job出现异常";
            EmailServiceAdapter.SendEmail(mail);
        }
    }
}
