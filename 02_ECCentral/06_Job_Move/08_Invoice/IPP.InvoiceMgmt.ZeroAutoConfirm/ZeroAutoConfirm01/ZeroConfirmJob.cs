using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Diagnostics;
using System.Threading;
using ZeroAutoConfirm.Biz;
using ZeroAutoConfirm.Compoents;

namespace ZeroAutoConfirm
{
    class ZeroConfirmJob : IJobAction
    {
        static ILog log = LogerManger.GetLoger();
        static JobContext CurrentContext = null;

        [STAThread]
        static void Main(string[] args)
        {
            Start();
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
            ZeroConfirmBP.ShowInfo = new ZeroConfirmBP.ShowMsg(WriteLog);
            sw.Start();
            try
            {
                ZeroConfirmBP.DoWork(are);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                ZeroConfirmBP.SendMail("支付宝账单自动确认异常", ex.Message ,0);
                are.Set();
            }
            are.WaitOne();
            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            CurrentContext = context;
            Start();
        }

        #endregion
    }
}
