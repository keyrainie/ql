using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using InvoiceMgmt.JobV31.Biz;

namespace InvoiceMgmt.JobHost
{
    public class Program
    {
        private const string BIZLOG = @"Log\PresentedPoints.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);
            context.Properties.Add("RejectionPercent", "30");
            Console.WriteLine("\n\nVIP卡用户年购满10000元送500积分\n");
            Console.WriteLine("输入1开始赠送");
            string x = Console.ReadLine();
            while (x != "x")
            {
                switch (x)
                {
                    case "1":
                        Console.WriteLine("结束： "+VIPCustomerPresentedPointsBP.PresentedPoints(context));
                        break;
                }
                x = Console.ReadLine();
            }  
            Console.Read();                  
        }             
    }    
}
