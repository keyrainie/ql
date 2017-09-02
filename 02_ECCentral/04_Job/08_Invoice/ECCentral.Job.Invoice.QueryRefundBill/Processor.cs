using ECCentral.Job.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCentral.Job.Invoice.QueryRefundBill
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
            
            ECCentral.Job.Utility.RestServiceError error;
            List<int> sysNoList = new List<int>();
            client.Query<List<int>>("/Job/GetSysNoListByRefund", out sysNoList, out error);
            if (sysNoList != null && sysNoList.Count > 0)
            {
                foreach (int sysNo in sysNoList)
                {
                    object obj = new object();
                    client.Query("/Job/QueryRefund/" + sysNo.ToString(), out obj, out error);
                }
            }
        }

        //public class Program
        //{
        //    static void Main()
        //    {
        //        Processor p = new Processor();
        //        p.Run(null);
        //    }
        //}
    }
}
