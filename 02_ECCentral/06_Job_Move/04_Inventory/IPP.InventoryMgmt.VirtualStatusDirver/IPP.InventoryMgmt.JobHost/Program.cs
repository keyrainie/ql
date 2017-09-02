using System;
using System.Collections.Generic;

using IPP.InventoryMgmt.JobV31.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;


namespace IPP.InventoryMgmt.JobHost
{
    class Program
    {
        private const string BIZLOG = @"Log\VirtualAction_Biz.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);
            Console.WriteLine("Please select a item to run:");
            Console.WriteLine("1: VirtualRequestLaunch");
            Console.WriteLine("2: VirtualRequsetExpire");
            Console.WriteLine("3: VirtualRequsetClose");
           
            string x = Console.ReadLine();
            
            while(x!="x")
            {
                switch (x)
                { 
                    case "1":
                        VirtualRequestRunBP runBP = new VirtualRequestRunBP();
                        runBP.Process(context);
                        break; 
                    case "2":
                        VirtualRequestExpireBP expireBP = new VirtualRequestExpireBP();
                        expireBP.Process(context);
                        break;
                    case "3":
                        VirtualRequestCloseBP closeBP = new VirtualRequestCloseBP();
                        closeBP.Process(context);
                        break;
                }
                x = Console.ReadLine();
            }
            
            Console.WriteLine(string.Format("结束 {0}",DateTime.Now));
            Console.Read();

        }
    }
}
