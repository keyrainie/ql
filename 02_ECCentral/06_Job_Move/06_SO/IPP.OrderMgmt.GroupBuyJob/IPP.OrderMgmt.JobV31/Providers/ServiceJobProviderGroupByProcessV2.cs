using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Providers
{
    public class ServiceJobProviderGroupByProcessV2 : IJobAction
    {

        #region IJobAction Members

        public void Run(JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["SORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];  //可以传入null ,表示所有
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());

            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update("/SO/Job/ProcessFinishedAndInvalidGroupBuySO", companyCode, out error);
        }

        #endregion
    }
}
