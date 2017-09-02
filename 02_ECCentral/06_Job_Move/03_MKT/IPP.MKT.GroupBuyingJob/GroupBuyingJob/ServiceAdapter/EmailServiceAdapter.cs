using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities;
using ECCentral.BizEntity.Common;

namespace IPP.MktToolMgmt.GroupBuyingJob
{
    public class EmailServiceAdapter
    {
        public static void SendEmail(MailEntity mailEntity)
        {
            try
            {
                string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
                string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];

                MailInfo mInfo = new MailInfo();
                mInfo.Subject = mailEntity.Subject;
                mInfo.Body = mailEntity.Body;
                mInfo.FromName = mailEntity.From;
                mInfo.ToName = mailEntity.To;
                mInfo.CCName = mailEntity.CC;
                mInfo.Priority = 1;

                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;
                var ar = client.Create("/Message/SendMail", mInfo, out error);

                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    throw new Exception(error.Faults[0].ErrorDescription);
                }
            }
            catch (Exception ex)
            {
                string errorLog = ConfigurationManager.AppSettings["ErrorLog"];
                Log.WriteLog("邮件发送失败！\r\n" + ex.ToString(), errorLog);
            }
        }
    }
}
