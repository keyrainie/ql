using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;
using ECCentral.Job.Utility;

namespace ECCentral.Job.RMA.RejectAuto
{
    /// <summary>
    /// 该Job处理的是SOType=10的联通结算单为中蛋特有，Service暂时未做。
    /// </summary>
    public class SOReject : IJobAction
    {

        public void Run(JobContext context)
        {
            string url = ConfigurationManager.AppSettings["ServiceBaseUrl"];
            string msg;
            RestServiceError error;
            RestClient client = new RestClient(url);
            bool hasCallSucceed = client.Create<string>("RMAService/Job/SOReject", null, out msg, out error);
            if (hasCallSucceed)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.WriteLine(error.StatusCode);
                foreach (var r in error.Faults)
                {
                    Console.WriteLine(r.ErrorDescription);
                }
            }

        }
    }
}
