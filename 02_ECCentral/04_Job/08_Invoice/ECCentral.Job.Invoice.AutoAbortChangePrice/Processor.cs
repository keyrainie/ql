using ECCentral.Job.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCentral.Job.Invoice.AutoAbortChangePrice
{
    public class Processor : IJobAction
    {
        public void Run(JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];  //可以传入null ,表示所有
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());

            RestServiceError error;
            client.Create("/Invoice/BatchAbortedPriceChangeByJob", "", out error);

            if(error != null)
            throw new Exception(error.StatusDescription);
        }
    }
}
