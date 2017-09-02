using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.Job.SO.AutoSendMessageSO.Biz.AutoAudit;


namespace ECCentral.Job.SO.AutoSendMessageSO.JobHos
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
            Console.WriteLine("Please select a item to run:");
            Console.WriteLine("1: Order FP Check.");
            Console.WriteLine("2: Order Auto Audit.");
            Console.WriteLine("3: Order Auto Send Message.");
            Console.WriteLine("4: OutStock Job.");
            Console.WriteLine("5: Retrive Dubious User");

            string x = Console.ReadLine();

            while (x != "x")
            {
                switch (x)
                {
                    case "1":
                     //   SOFPCheckBP.Check(context);
                        break;
                    case "2":
                        //SOAutoAuditBP.AuditSO(context);
                        break;
                    case "3":
                        SOAutoAuditSendMessageBP.SendMessage(context);
                        break;
                    case "4":
                        //OutStockBP.Run(context);
                        break;
                    case "5":
                        //SOFetchDubiousUserBP.FetchUser(context);
                        break;
                }
                x = Console.ReadLine();
            }

            Console.WriteLine(string.Format("检查结束 {0}", DateTime.Now));
            Console.Read();

        }
    }
}
