using System;
using System.Collections.Generic;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31;

namespace IPP.InventoryMgmt.JobHost
{
    class Program
    {
        static void Main(string[] args)
        {

            JobAutoRun job = new JobAutoRun();
            Console.WriteLine("PM项目控制管理Job开始运行\n\n");    
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", @"Log\log.txt");
            

            Console.WriteLine("Please choose model:(Init/Update)");

            string x = Console.ReadLine();

            switch (x)
            {
                case "Init":
                    context.Properties.Add("Model", x);
                    break;
                case "Update":
                    context.Properties.Add("Model", x);
                    break;
                
                
            }

            job.Run(context);
 

            Console.WriteLine("。。。。。。。。Runing。。。。。。。。");
            Console.ReadLine().Trim();
        }
    }
}
