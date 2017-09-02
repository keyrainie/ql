using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCEntral.Job.SO.AbandonGroupBuySO
{
    public class Processor : Newegg.Oversea.Framework.JobConsole.Client.IJobAction
    {
        public void Run(Newegg.Oversea.Framework.JobConsole.Client.JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"]; //可以传入null ,表示所有
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update("/SO/Job/AbandonNotPayGroupBuySO", companyCode, out error);
        }
    }
}
