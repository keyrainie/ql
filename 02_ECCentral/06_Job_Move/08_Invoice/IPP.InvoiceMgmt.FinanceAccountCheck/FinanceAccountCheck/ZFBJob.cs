using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IPPOversea.Invoicemgmt.ZFBAccountCheck.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;
namespace IPPOversea.Invoicemgmt.ZFBAccountCheck
{

    public class ZFBJob : IJobAction
    {
        static ILog log = LogerManger.GetLoger();
        static JobContext CurrentContext = null;
        [STAThread]
        static void Main(string[] args)
        {

            Stopwatch sw = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog("************************************************");
            WriteLog("*");
            WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
            WriteLog("*  否则，可能造成数据导入异常");
            WriteLog("*");
            WriteLog("************************************************");
            Console.ResetColor();
            AutoResetEvent are = new AutoResetEvent(false);
            ZFBBP.ShowInfo = new ZFBBP.ShowMsg(WriteLog);
            sw.Start();
            try
            {
                ZFBBP.DoWork(are);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                ZFBBP.SendMail("支付宝对账异常", ex.Message);
                are.Set();
            }
            are.WaitOne();
            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }
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
            Stopwatch sw = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog("************************************************");
            WriteLog("*");
            WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
            WriteLog("*  否则，可能造成数据导入异常");
            WriteLog("*");
            WriteLog("************************************************");
            Console.ResetColor();
            AutoResetEvent are = new AutoResetEvent(false);
            ZFBBP.ShowInfo = new ZFBBP.ShowMsg(WriteLog);
            sw.Start();
            try
            {
                ZFBBP.DoWork(are);
            }
            catch (Exception ex)
            {
                Console.WriteLine("应用程序出错了！");
                WriteLog(ex.Message);
                ZFBBP.SendMail("支付宝对账异常", ex.Message);
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
