using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;
using ECCentral.Job.Utility;


namespace ECCentral.Job.MKT.BatchUpdateKeywordsFromQueue
{
    public class Processor : Newegg.Oversea.Framework.JobConsole.Client.IJobAction
    {
        public void Run(Newegg.Oversea.Framework.JobConsole.Client.JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            RestClient client = new RestClient(baseUrl);
            RestServiceError error;
            string companyCode = "8601";
            var ar = client.Update("/MKTService/Job/BatchUpdateKeywordsFromQueue", companyCode, out error);
        }
    }
}
