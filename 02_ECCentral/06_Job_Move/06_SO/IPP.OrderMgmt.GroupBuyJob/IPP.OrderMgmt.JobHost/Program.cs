using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.Providers;
using Newegg.Oversea.Framework.JobConsole.Client;


namespace IPP.OrderMgmt.JobHost
{
    class Program
    {
        private const string BIZLOG = @"Log\FPCheck_Biz.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            //context.Properties.Add("BizLog", BIZLOG);
            context.Properties.Add("RejectionPercent", "30");
            Console.WriteLine("Please select a item to run:");
            Console.WriteLine("1:Finished GroupBuy Process.");
            Console.WriteLine("2:Abandon GroupBuy SO which didn't pay in time.");           

            string x = Console.ReadLine();
            
            while(x!="x")
            {
                switch (x)
                { 
                    case "1":
                        new ServiceJobProviderGroupByProcessV2().Run(context);
                        break;
                    case "2":
                        new ServiceJobProviderAbandonSOV2().Run(context);
                        break;                   
                }
                x = Console.ReadLine();
            }
            
            Console.WriteLine(string.Format("处理结束 {0}",DateTime.Now));
            Console.Read();

        }
    }
}
