using System;
using System.Threading;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.IO;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Collections.Generic;
using ProductSaleInfoInterface;

namespace ProductSaleInfoMain
{

    public class JobMain : IJobAction
    {
        static ILog log = LogerManger.GetLoger();
        static JobContext CurrentContext = null;
        [STAThread]
        static void Main(string[] args)
        {
            Start();
        }

        private static void Start()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<IProductSaleInfo> handlers = CustomeReportFactory.GetHandlers();          
            WriteLog("本轮开始...");
            try
            {
                foreach (var item in handlers)
                {
                    item.ShowInfo = WriteLog;
                    item.SendProductSaleInfoReport();
                }
            }
            catch (Exception ex)
            {               
                WriteLog(ex.Message);              
            }           
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
        public void Run(JobContext context)
        {
            CurrentContext = context;
            Start();
        }       
    }
}
