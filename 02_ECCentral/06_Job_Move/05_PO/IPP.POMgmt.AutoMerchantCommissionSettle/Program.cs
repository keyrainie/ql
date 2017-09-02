using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MerchantCommissionSettle.Components.Logger;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Diagnostics;
using MerchantCommissionSettle.Business;
using MerchantCommissionSettle.Components;

namespace MerchantCommissionSettle
{
    public class Program : IJobAction
    {
        #region Fields

        private static ILog logger = LogManager.GetLogger();

        private static JobContext CurrentContext = null;

        #endregion

        #region Main

        private static void Main(string[] args)
        {
            //Console.WriteLine("Press input runVendorSysNo...");
            //int runVendorSysNo = int.TryParse(Console.ReadLine(), out runVendorSysNo) ? runVendorSysNo : 0;

            //while (runVendorSysNo == 0)
            //{
            //    Console.WriteLine("Press input runVendorSysNo...");
            //    runVendorSysNo = int.TryParse(Console.ReadLine(), out runVendorSysNo) ? runVendorSysNo : 0;
            //}

            //#region TestCode

            //var testJobContext = new JobContext();
            //testJobContext.Properties = new System.Collections.Generic.Dictionary<string, string>();
            ////计算供应商编号
            //testJobContext.Properties.Add("RunVendorSysno", runVendorSysNo.ToString());
            //new Program().Run(testJobContext);

            //#endregion

            ////new Program().Run(null);

            //Console.WriteLine("Press any key to exit...");
            //if (args == null || args.Length == 0)
            //{
            //    Console.ReadKey();
            //}
            new Program().Run(null);
        }

        #endregion

        #region WriteLog

        private static void WriteLog(string info)
        {
            if (CurrentContext != null)
            {
                CurrentContext.Message += Environment.NewLine + info;
            }
            logger.WriteLog(info);
            Console.WriteLine(info);
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

        #endregion

        #region Run

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

            CommissionBP business = null;

            try
            {
                business = new CommissionBP(context);
                business.DisplayMessage += WriteLog;

                business.SettleCommission();
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

        #endregion
    }
}
