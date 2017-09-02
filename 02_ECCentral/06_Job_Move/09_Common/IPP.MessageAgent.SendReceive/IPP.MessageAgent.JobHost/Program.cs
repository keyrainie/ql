using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.MessageAgent.SendReceive.JobV31.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;


namespace IPP.OrderMgmt.JobHost
{
    class Program
    {
        private const string BIZLOG = @"Log\Message_Log.txt";
        static void Main(string[] args)
        {
            Console.WriteLine(string.Format("检查开始 {0}", DateTime.Now));

            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", BIZLOG);

            try
            {
                SSBProcessBP.RunProcess(context);
                Console.WriteLine(string.Format("检查结束 {0}", DateTime.Now));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("异常： {0}", ex.Message));
            }
            if (args == null || args.Length == 0)
            {
                Console.Read();
            }
        }
    }
}
