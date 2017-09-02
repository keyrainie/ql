using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using IPPOversea.InvoiceMgmt.PerMonthReport.Biz;
using IPPOversea.InvoiceMgmt.PerMonthReport.Compoents;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPPOversea.InvoiceMgmt.PerMonthReport
{
    public class ReportJob : IJobAction
    {
        static ILog log = LogerManger.GetLoger();

        static JobContext CurrentContext = null;

        [STAThread]
        static void Main(string[] args)
        {
            #region TestCode
            //var testJobContext = new JobContext();
            //testJobContext.Properties = new System.Collections.Generic.Dictionary<string, string>();
            ////当前日期
            //testJobContext.Properties.Add("CurrentDay", "2012-05-01");
            //Start(testJobContext);
            #endregion
                        
            Start(null);
        }

        static void WriteLog(string info)
        {
            log.WriteLog(info);
            Console.WriteLine(info);
            if (CurrentContext != null)
            {
                CurrentContext.Message += info + Environment.NewLine;
            }
        }

        public void Run(JobContext context)
        {
            CurrentContext = context;
            Start(context);
        }

        private static void Start(JobContext context)
        {
            Stopwatch sw = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog("************************************************");
            WriteLog("*");
            WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
            WriteLog("*");
            WriteLog("************************************************");
            Console.ResetColor();
            ReportBP.ShowInfo = new ReportBP.ShowMsg(WriteLog);
            sw.Start();
            try
            {
                ReportBP.DoWork(context);
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                ReportBP.SendMail(Settings.EmailAddressError, Settings.EmailSubjectError, ex.ToString(), 0);
            }
            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }
    }
}
