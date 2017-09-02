using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPP.OrderMgmt.SendSOPayToEPortJob.Providers;

namespace IPP.OrderMgmt.SendSOPayToEPortJobHost
{
    class Program
    {
        private const string BIZLOG = @"Log\FPCheck_Biz.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);
            context.Properties.Add("RejectionPercent", "30");
            (new IPP.OrderMgmt.SendSOPayToEPortJob.Providers.SendSOPayToEPortJob()).Run(context);

            Console.WriteLine(string.Format("检查结束 {0}", DateTime.Now));
            Console.Read();

        }
    }
}
