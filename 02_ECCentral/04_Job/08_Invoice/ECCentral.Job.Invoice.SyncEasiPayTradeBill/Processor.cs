using ECCentral.Job.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCentral.Job.Invoice.SyncEasiPayTradeBill
{
    public class Processor : IJobAction
    {
        public void Run(JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            client.Timeout = 100000;
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];  //可以传入null ,表示所有
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());
            string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            RestServiceError error;
            bool bResutl = false;
            client.Query("/Job/SyncTradeBillTrade/" + date, out bResutl, out error);

            if (error != null)
            {
                string errorMessage = "";
                foreach (var errItem in error.Faults)
                {
                    errorMessage = errItem.ErrorDescription;
                }
                throw new Exception(error.StatusDescription + errorMessage);
            }
        }

        static void Main(string[] args)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            client.Timeout = 100000;
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];  //可以传入null ,表示所有
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());
            string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            RestServiceError error;
            bool bResutl = false;
            client.Query("/Job/SyncTradeBillTrade/" + date, out bResutl, out error);

            Console.WriteLine("对账开始");

            if (error != null)
            {
                string errorMessage = "";
                foreach (var errItem in error.Faults)
                {
                    errorMessage = errItem.ErrorDescription;
                    Console.WriteLine("error " + errorMessage);
                }
                throw new Exception(error.StatusDescription + errorMessage);
            }
            Console.WriteLine("对账结束");
            Console.ReadLine();
        }

    }
}
