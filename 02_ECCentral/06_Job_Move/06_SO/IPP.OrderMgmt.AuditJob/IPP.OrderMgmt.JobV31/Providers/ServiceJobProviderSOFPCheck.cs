using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Providers
{
    public class JobV31ProviderSOFPCheck : IJobAction
    {
        public void Run(JobContext context)
        {
            //SOFPCheckBP.Check(context);
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["SORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            JobFPCheckReq req = new JobFPCheckReq();
            //特定用户的订单不检查，比如Intel订单，#代表不过滤
            req.Interorder = "#";
            req.CompanyCode = companyCode;
            req.IgnoreCustomIDList = new List<string>();
            req.OutStockList = new List<int>();
            var ar = client.Update("/SO/Job/FPCheck", req, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                foreach (var errorItem in error.Faults)
                {
                    context.Message += errorItem.ErrorDescription;
                }
            }
        }
    }
}
