using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCEntral.Job.SO.AuditBalanceSO
{
    public class Processor : Newegg.Oversea.Framework.JobConsole.Client.IJobAction
    {
        public void Run(Newegg.Oversea.Framework.JobConsole.Client.JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update("/SO/Job/AuditBuyMobileSettlementSO", null, out error);
        }
    }
}
