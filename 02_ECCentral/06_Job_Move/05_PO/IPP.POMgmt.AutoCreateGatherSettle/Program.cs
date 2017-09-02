using System;
using System.Diagnostics;
using System.Text;
using System.Configuration;

using POASNMgmt.AutoCreateVendorSettle.Compoents.Configuration;
using Newegg.Oversea.Framework.JobConsole.Client;
using POASNMgmt.AutoCreateVendorSettle.Compoents.Logger;
using POASNMgmt.AutoCreateVendorSettle.Business;
using POASNMgmt.AutoCreateVendorSettle.Compoents;

namespace POASNMgmt.AutoCreateSettleGather
{
    public class Program : IJobAction
    {
        private static ILog logger = LogManager.GetLogger();

        private static JobContext CurrentContext;

        public static void Main(string[] args)
        {
            #region TestCode
            //var testJobContext = new JobContext();
            //testJobContext.Properties = new System.Collections.Generic.Dictionary<string, string>();
            ////当前日期
            //testJobContext.Properties.Add("CurrentDay", "2012-07-01");
            ////最大的原始单据截至日期
            //testJobContext.Properties.Add("MaxOrderEndData", "2012-07-01");
            ////计算供应商编号
            //testJobContext.Properties.Add("RunVendorSysno", "1296");
            //new Program().Run(testJobContext);
            #endregion

            new Program().Run(null);
            if (args == null || args.Length == 0)
            {
                Console.Read();
            }
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
            WriteLog("*  代收结算单自动创建Job ");
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
                business.CreateGatherSettle();
            }
            catch (Exception ex)
            {
                WriteLog(ex);

                if (business != null)
                {
                    business.SendMail(GlobalSettings.MailSubject, ex.Message);
                }
            }

            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }
    }
}
