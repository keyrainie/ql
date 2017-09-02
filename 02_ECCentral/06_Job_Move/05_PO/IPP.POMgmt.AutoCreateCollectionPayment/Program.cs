using System;
using System.Diagnostics;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using POASNMgmt.AutoCreateCollectionPayment.Business;
using POASNMgmt.AutoCreateCollectionPayment.Compoents;
using POASNMgmt.AutoCreateCollectionPayment.Compoents.Logger;

namespace POASNMgmt.AutoCreateCollectionPayment
{
    public sealed class Program : IJobAction
    {
        private static ILog logger = LogManager.GetLogger();

        private static JobContext CurrentContext = null;

        private static void Main()
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            new Program().Run(null);
        }

        private static void WriteLog(string info)
        {
            //布署jobconsole后，不写文件和控制台日志
            if (CurrentContext != null)
            {
                CurrentContext.Message += Environment.NewLine + info;
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
            CurrentContext = context;
            Stopwatch sw = Stopwatch.StartNew();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog("************************************************");
            WriteLog("*");
            WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
            WriteLog("*  否则，可能造成数据导入异常");
            WriteLog("*");
            WriteLog("************************************************");
            Console.ResetColor();

            VendorSettleBP business = null;

            try
            {
                business = new VendorSettleBP(context);
                business.DisplayMessage += WriteLog;

                business.CreateVendorSettle();
            }
            catch (Exception ex)
            {
                WriteLog(ex);

                if (business != null)
                {
                    business.SendMail(GlobalSettings.MailAddress, ex.Message);
                }
            }

            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }
    }
}
