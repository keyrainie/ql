using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Contract;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.ServiceContracts;
using ECCentral.BizEntity.Common;
using ECCentral.Job.Utility;

namespace IPP.OrderMgmt.JobV31.ServiceAdapter
{
    public class CommonServiceAdapter
    {

        public static bool SendEmail2MailDb(string mailFrom, string mailTo, string ccAddress, string bccAddress, string mailSubject, string mailBody, string companyCode)
        {
            MailInfo mailInfo = new MailInfo();
            mailInfo.FromName = mailFrom;
            mailInfo.ToName = mailTo;
            mailInfo.CCName = ccAddress;
            mailInfo.BCCName = bccAddress;
            mailInfo.Subject = mailSubject;
            mailInfo.Body = mailBody;

            if (string.IsNullOrEmpty(mailInfo.ToName))
            {
                return false;
            }
            else
            {
                mailInfo.ToName = Util.TrimNull(mailInfo.ToName);
            }
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Message/SendMail", mailInfo, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }
            return true;
        }
    }
}
