using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.InventoryMgmt.JobV31.Biz.InventoryAge;
using Newegg.Oversea.Framework.JobConsole.Client;



namespace IPP.InventoryMgmt.JobHost
{
    class Program
    {
        private const string BIZLOG = @"Log\InventroyAging_Biz.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);
            context.Properties.Add("RejectionPercent", "30");
            Console.WriteLine("Please select a item to run:");
            Console.WriteLine("1: InventoryAging");
           
            string x = Console.ReadLine();
            
            while(x!="x")
            {
                switch (x)
                { 
                    case "1":
                        InventoryAgeBP.SolveInventoryAge(context);
                        break;
                   
                }
                x = Console.ReadLine();
            }
            
            Console.WriteLine(string.Format("结束 {0}",DateTime.Now));
            Console.Read();

        }
    }
}
