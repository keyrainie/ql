using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AutoSendMail.BP;
using AutoSendMail.Components;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace AutoSendMail
{

    public sealed class Program : IJobAction
    {
        #region Instance Fields

        private JobContext context;

        private static ILog logger = LoggerManager.GetLogger();

        #endregion

        #region Main

        [STAThread]
        private static void Main(string[] args)
        {
            new Program().Run(null);
        }

        #endregion

        #region IJobAction Members

        public void Run(JobContext context)
        {
            try
            {
                this.context = context;

                Console.ForegroundColor = ConsoleColor.Green;
                WriteLog("************************************************");
                WriteLog("*");
                WriteLog("*  在没有提示可以安全退出时，不要人为退出程序。");
                WriteLog("*  否则，可能造成数据导入异常");
                WriteLog("*");
                WriteLog("************************************************");
                Console.ResetColor();

                var config = GetConfiguration();
                var jobs = GetActionList();

                WriteLog(string.Format("共有 {0} 个任务", config.Sequence.Length));

                Stopwatch sw = new Stopwatch();

                foreach (var item in config.Sequence)
                {
                    var actionName = item.ActionName;

                    try
                    {
                        if (jobs != null && jobs.Count > 0 && !jobs.Contains(item.ActionName))
                        {
                            WriteLog(string.Format("任务 < {0} > 没有激活", actionName));

                            continue;
                        }

                        WriteLog(string.Format("任务 < {0} > 开始....", actionName));

                        sw.Start();

                        AutoSendMailBP.SendMail(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("任务 < {0} > 出错了！", actionName));

                        WriteLog(ex.Message);
                        WriteLog(ex.StackTrace);
                    }
                    finally
                    {
                        sw.Stop();
                        WriteLog(string.Format("任务 < {0} > 共耗时 {1} 秒", actionName, sw.Elapsed.TotalSeconds));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                WriteLog(ex.StackTrace);
            }
        }

        #endregion

        #region 读取配置文件

        private Root GetConfiguration()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Root));

            using (FileStream stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["MailConfigPath"])))
            {
                return (Root)xs.Deserialize(stream);
            }
        }

        #endregion

        #region 获取激活状态的报警列表

        /// <summary>
        /// 返回激活的报警列表,空集合表示没有限制
        /// </summary>
        /// <returns></returns>
        private List<string> GetActionList()
        {
            List<string> jobs = new List<string>();
            var configString = ConfigurationManager.AppSettings["ActionList"];

            if (string.IsNullOrEmpty(configString))
            {
                return jobs;
            }

            jobs = configString.Split(',').ToList();

            if (jobs.Contains("*"))
            {
                jobs.Clear();

                return jobs;
            }

            return jobs;
        }

        #endregion

        #region 记录日志

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message">message</param>
        private void WriteLog(string message)
        {
            logger.WriteLog(message);
            Console.WriteLine(message);

            if (context != null)
            {
                context.Message += Environment.NewLine + message;
            }
        }

        #endregion
    }
}
