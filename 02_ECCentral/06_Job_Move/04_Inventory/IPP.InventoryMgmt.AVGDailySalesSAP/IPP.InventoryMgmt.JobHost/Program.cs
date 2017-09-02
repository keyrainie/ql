using System;
using System.Collections.Generic;
using IPP.InventoryMgmt.JobV31.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.InventoryMgmt.JobHost
{
    public class Program
    {
        private const string BIZLOG = @"Log\AVGDailySalesSync.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);
            context.Properties.Add("RejectionPercent", "30");

            context.Properties.Add("ISInitialHavaSaledRecord","1");
            context.Properties.Add("ISInitialHavaNotSaledRecord","1");

            Console.WriteLine("Please select a item to run:");
            Console.WriteLine("1: AVGDailySalesSAP");           
            string x = Console.ReadLine();            
            while(x!="x")
            {
                switch (x)
                { 
                    case "1":
                        Console.WriteLine("同步日均销售表结束： " + AVGDailySalesToSAPBP.Process(context));                     
                        break;                 
                }
                x = Console.ReadLine();
            }                       
            Console.Read();
        }
    }
}
