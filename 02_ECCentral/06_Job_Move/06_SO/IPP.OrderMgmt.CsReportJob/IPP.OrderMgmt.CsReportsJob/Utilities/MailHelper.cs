using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Contract;
using IPP.ThirdPart.JobV31;
using ECCentral.BizEntity.Common;
using ECCentral.Job.Utility;

namespace IPP.OrderMgmt.CsReportsJob.Utilities
{
    public class MailHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyCode">companycode</param>
        /// <param name="isSys">true, system email; false</param>
        /// <param name="mailInfor"></param>
        public static void SendEmail(MailInfo mailInfor)
        {
            if (string.IsNullOrEmpty(mailInfor.ToName)) return;
            else
                mailInfor.ToName = Util.TrimNull(mailInfor.ToName);
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Message/SendMail", mailInfor, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }
        }
    }
}
