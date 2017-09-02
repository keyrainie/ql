using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IPPOversea.Invoicemgmt.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;
namespace IPPOversea.Invoicemgmt
{
    public class InvoiceMonitor : IJobAction
    {
        static ILog log = LogerManger.GetLoger();
        static JobContext CurrentContext = null;

        [STAThread]
        static void Main(string[] args)
        {
            new InvoiceMonitor().Run(null); 
        }
        static void WriteLog(string info)
        {
            if (CurrentContext != null)
            {
                CurrentContext.Message += Environment.NewLine + info;
            }
            else
            {
                log.WriteLog(info);
                Console.WriteLine(info);
            }
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            CurrentContext = context;
            Stopwatch sw = new Stopwatch();
            WriteLog("本轮开始...");
            AutoResetEvent are = new AutoResetEvent(false);
            MonitorBP.ShowInfo = new MonitorBP.ShowMsg(WriteLog);
            sw.Start();
            try
            {
                MonitorBP.DoWork(are);
            }
            catch (Exception ex)
            {
                Console.WriteLine("应用程序出错了异常！");
                WriteLog(ex.Message);
                are.Set();
            }
            are.WaitOne();
            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }

        #endregion
    }
}
