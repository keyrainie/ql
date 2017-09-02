using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Job.SO.KnownFraudCustomer
{
    public class Processor : Newegg.Oversea.Framework.JobConsole.Client.IJobAction
    {
        public void Run(Newegg.Oversea.Framework.JobConsole.Client.JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl);
            client.AsyncUpdate("/SO/Job/InsertKnownFraudCustomer", null, () =>
            {

            });

        }
    }
}
