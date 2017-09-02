using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPPOversea.Invoicemgmt.SyncDataToFinancePay.Biz;
namespace IPPOversea.Invoicemgmt.SyncDataToFinancePay
{

    public class SyncDataToFinancePayJob : IJobAction
    {
        static ILog log = LogerManger.GetLoger();

        static JobContext CurrentContext = null;

        static void Main(string[] args)
        {
            Start();
        }
        
        /// <summary>
        /// 记录日志方法
        /// </summary>
        /// <param name="info">需要记录的信息</param>
        static void WriteLog(string info)
        {
            log.WriteLog(info);
            Console.WriteLine(info);
            if (CurrentContext!=null)
            {
                CurrentContext.Message += info + Environment.NewLine;
            }            
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            CurrentContext = context;
            Start();
        }

        private static void Start()
        {
            Stopwatch sw = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog("************************************************");
            WriteLog("*");
            WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
            WriteLog("*");
            WriteLog("************************************************");
            Console.ResetColor();
            AutoResetEvent are = new AutoResetEvent(false);
            SyncDataBL.ShowInfo = new SyncDataBL.ShowMsg(WriteLog);
            sw.Start();
            try
            {
                SyncDataBL.DoWork(are);
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
               
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
