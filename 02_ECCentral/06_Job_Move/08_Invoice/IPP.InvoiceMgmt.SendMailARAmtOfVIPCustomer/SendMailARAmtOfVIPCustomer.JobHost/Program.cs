using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ECCentral.Job.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;
using SendMailARAmtOfVIPCustomer.Biz.Processor;

namespace SendMailARAmtOfVIPCustomer.JobHost
{
    class Program : IJobAction
    {

        private static ILog logger = LoggerManager.GetLogger();
        private static JobContext context = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            new Program().Run(context);
        }

        private static void WriteLog(string info)
        {
            //布署jobconsole后，不写文件和控制台日志
            if (context != null)
            {
                context.Message += Environment.NewLine + info;
            }
            else
            {
                logger.WriteLog(info);
                Console.WriteLine(info);
            }
        }

        private static void WriteLog(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            StringBuilder messageBuilder = new StringBuilder();
            Exception exception = ex;

            do
            {
                messageBuilder.AppendLine("Message:");
                messageBuilder.AppendLine(exception.Message);
                messageBuilder.AppendLine("Stack Trace:");
                messageBuilder.AppendLine(exception.StackTrace);
                messageBuilder.AppendLine("Source:");
                messageBuilder.AppendLine(exception.Source);

                exception = exception.InnerException;

                if (exception != null)
                {
                    messageBuilder.AppendLine("InnerException:");
                }
            }
            while (exception != null);

            WriteLog(messageBuilder.ToString());
        }

        public void Run(JobContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog("************************************************");
            WriteLog("*");
            WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
            WriteLog("*  否则，可能造成数据导入异常");
            WriteLog("*");
            WriteLog("************************************************");
            Console.ResetColor();

            try
            {
                SendMailARAmtOfVIPCustomerBP.Start(context);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }

            sw.Stop();
            WriteLog(string.Format("检查结束 {0}", DateTime.Now));
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }
    }
}
