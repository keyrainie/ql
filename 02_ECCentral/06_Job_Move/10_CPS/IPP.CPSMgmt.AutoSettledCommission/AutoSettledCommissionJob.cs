using System;
using System.Diagnostics;
using System.Threading;
using IPPOversea.Invoicemgmt.AutoSettledCommission.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission
{

    public class AutoSettledCommission : IJobAction
    {
        #region member
        static ILog log = LogerManger.GetLoger();
        static JobContext CurrentContext = null;
        #endregion

        #region Job 入口

        #region 本地运行使用
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Start();
        }
        #endregion 本地运行使用

        #region IJobAction Members
        /// <summary>
        /// Job 运行方法
        /// </summary>
        /// <param name="context">上下文对象</param>
        public void Run(JobContext context)
        {
            CurrentContext = context;
            Start();
        }
        #endregion

        #endregion

        /// <summary>
        /// 记录日志方法
        /// </summary>
        /// <param name="info">需要记录的信息</param>
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
            
            sw.Start();
            try
            {
                /*添加业务逻辑:
                 * 1.CPS 佣金SO & RMA 数据同步
                 */

                //<--1.应付对账逻辑
                BizFacadeManager.AutoSettledCommission.CreateWorkFlow();
                BizFacadeManager.AutoSettledCommission.ProcessSyncCommissionSettlement();
                //-->
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
            finally
            {
                are.Set();
            }
                
            ////are.Set();
            are.WaitOne();
            sw.Stop();
            WriteLog("本轮结束!");
            WriteLog(string.Format("本次任务共耗时 {0} 秒", sw.Elapsed.TotalSeconds));
        }


    }
}
