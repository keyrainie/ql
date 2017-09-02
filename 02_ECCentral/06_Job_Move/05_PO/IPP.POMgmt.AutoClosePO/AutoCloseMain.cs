using System;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newegg.Oversea.Framework.JobConsole.Client;
using AutoClose.DAL;
namespace IPPOversea.POmgmt
{

    public class AutoCloseMain : IJobAction
    {
        static ILog log = LogerManger.GetLoger();
        static JobContext CurrentContext = null;


        [STAThread]
        static void Main(string[] args)
        {
            new AutoCloseMain().Run(null);
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

        #region IJobAction Members

        public void Run(JobContext context)
        {
            CurrentContext = context;
            Stopwatch sw = new Stopwatch();
            WriteLog("本轮开始...");

            sw.Start();
            try
            {
                AutoClose.Biz.AutoCLosePoBP.DoWork(CurrentContext);
                //MonitorBP.DoWork(are);
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("应用程序出现了异常！:{0}",ex.Message));
                WriteLog(ex.StackTrace);
                AutoCloseDA.SendEmail(0, Settings.ExceptionMeil, "PO关闭JOB邮件", ex.StackTrace);           
            }
            
            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }

        #endregion
    }
}
