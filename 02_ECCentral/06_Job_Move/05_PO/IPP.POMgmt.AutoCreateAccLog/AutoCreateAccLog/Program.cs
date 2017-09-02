using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace AutoCreateAccLog
{
    public class Program : IJobAction
    {
        static void Main(string[] args)
        {
            string key = string.Empty;
            Console.WriteLine("请选择：1、启动JOB，0、退出");
            while ((key = Console.ReadLine()) != "0")
            {
                Program p = new Program();
                p.Run(new JobContext
                {
                    Properties = new Dictionary<string, string>(),
                    JobRunningPath = AppDomain.CurrentDomain.BaseDirectory,
                    Message = string.Empty
                });
                Console.WriteLine("请选择：1、启动JOB，0、退出");
            }            
        }

        private JobContext Context;
        private StringBuilder MailBody = null;
        #region IJobAction Members

        public void Run(JobContext context)
        {
            this.Context = context;
            Context.Message = string.Empty;
            WriteLog("\r\n\r\n\r\nJob启动");
            MailBody = new StringBuilder(3000);

            int pageSize = 0;
            if (context.Properties.ContainsKey("PageSize"))
            {
                int.TryParse((context.Properties["PageSize"] ?? string.Empty).Trim(), out pageSize);
            }
            if (pageSize == 0)
            {
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageSize);
            }
            if (pageSize == 0)
            {
                pageSize = 1000;
            }
            WriteLog("正在读取数据……");
            List<ConsignToAccLogEntity> list = CommandDA.Query(pageSize);
            WriteLog("本次共读取数据{0}条", list.Count);
            if (list.Count > 0)
            {
                int sysno = 0;
                string log = string.Empty;
                WriteLog("开始写入代销转财务记录……");
                list.ForEach((item) =>
                {
                    log = string.Empty;
                    SetDefaultPropertiesValue(item);
                    try
                    {
                        log += string.Format("OrderSysNo:{0},ProductSysNo:{1},VendorSysNo:{2}", item.OrderSysNo, item.ProductSysNo, item.VendorSysNo);
                        sysno = CommandDA.InsertConsignToAccLog(item);
                        log += string.Format(",ConsignToAccLogSysNo:{0}", sysno);
                    }
                    catch (Exception ex)
                    {
                        log += string.Format(" 写入失败，{0}，详情请查看日志", ex.Message);
                        MailBody.AppendFormat("{0}<br />", log);
                        log += "\r\n" + ex.StackTrace;
                    }
                    WriteLog(log);
                });
                WriteLog("写入完成。");
            }

            SendMail(MailBody.ToString());

            WriteLog("Job退出");

#if DEBUG
            WriteFile(context.Message);
#endif
        }

        #endregion

        private void SetDefaultPropertiesValue(ConsignToAccLogEntity entity)
        {
            if (entity == null)
            {
                return;
            }
            entity.CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
            entity.StoreCompanyCode = ConfigurationManager.AppSettings["StoreCompanyCode"];
            entity.LanguageCode = ConfigurationManager.AppSettings["LanguageCode"];
            entity.CreateTime = DateTime.Now;
            entity.CurrencySysNo = 1;
            entity.Note = "Create By Job AutoCreateAccLog";
            entity.Status = 0;
        }

        private void WriteLog(string log)
        {
            WriteLog(log, string.Empty);
        }

        private void WriteLog(string format, params object[] args)
        {
            if (args == null
                || args.Length == 0)
            {
                Context.Message += string.Format("{0}   {1}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), format);
            }
            else
            {
                Context.Message += string.Format("{0}   {1}\r\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), string.Format(format, args));
            }
#if DEBUG
            Console.WriteLine(format, args);
#endif
        }

        private void WriteFile(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, @"log.log");
            using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8, 1024))
            {
                sw.Write(message);
                sw.Close();
            }
        }

        private void SendMail(string mailBody)
        {
            string mailNotify = string.Empty;

            if (Context.Properties.ContainsKey("MailNotify"))
            {
                mailNotify = Context.Properties["MailNotify"];
            }
            else
            {
                mailNotify = ConfigurationManager.AppSettings["MailNotify"];
            }

            if (mailNotify == "1")
            {
                WriteLog("正在发送通知邮件……");
                if (string.IsNullOrEmpty(mailBody))
                {
                    mailBody = "没有新数据产生";
                }
                string to = ConfigurationManager.AppSettings["MailAddress"];
                string from = ConfigurationManager.AppSettings["MailFrom"];
                string subject = ConfigurationManager.AppSettings["MailSubject"];
                string cc = ConfigurationManager.AppSettings["CCAddress"];
                try
                {
                    CommandDA.SendMail(subject, to, from, cc, mailBody);

                    WriteLog("通知邮件已发送");
                }
                catch (Exception ex)
                {
                    WriteLog("{0}\r\n{1}", ex.Message, ex.StackTrace);
                }
            }
            else
            {
                return;
            }
        }
    }
}
