using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;


namespace IPP.OrderMgmt.ShipLog.Providers
{
    public class JobV31ProviderSOAutoAudit : IJobAction
    {
        public void Run(JobContext context)
        {
            //SOAutoAuditBP.AuditSO(context);
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["SORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            int auditUserSysNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AuditUserSysNo"]);
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
          
            //特定用户的订单不检查，比如Intel订单,#代表不过滤
            
            var ar = client.Update("/SO/Job/AutoAuditSO", null, out error);
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
