using System;
using System.Collections.Generic;
using IPP.OrderMgmt.JobV31.Biz.SendMail;
using Newegg.Oversea.Framework.JobConsole.Client;


namespace IPP.OrderMgmt.JobHost
{
    public class Program
    {
        private const string BIZLOG = @"Log\FPCheck_Biz.txt";
        static void Main(string[] args)
        {
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);
            context.Properties.Add("RejectionPercent", "30");
            Console.WriteLine("\n超过十天邮件提醒\n\n");          
            Console.WriteLine(SOSendAlarmMailBP.Check(context));
            Console.Read();                  
        }             
    }    
}
