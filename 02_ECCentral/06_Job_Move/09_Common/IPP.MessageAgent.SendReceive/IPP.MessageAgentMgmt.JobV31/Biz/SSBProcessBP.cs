using System;
using System.Collections.Generic;
using System.Threading;

using IPP.MessageAgent.SendReceive.JobV31.Configuration;
using Newegg.Oversea.Framework.Core.Threading;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.MessageAgent.SendReceive.JobV31.Biz
{
    public class SSBProcessBP
    {
        public static void RunProcess(JobContext jobContext)
        {
            SSBProcessConfig processConfig = ConfigHelper.SSBProcessConfig;
            if (processConfig == null)
            {
                string message = "无效的SSBProcess配置";
                jobContext.Message = message;
                Console.WriteLine(message);
                return;
            }

            if (processConfig.SSBChannels.Count == 1)
            {
                SingleChannelProcess(jobContext, processConfig);
            }
            else
            {
                MultiChannelProcess(jobContext, processConfig);
            }
        }

        /// <summary>
        /// 处理单张SSB表
        /// </summary>
        /// <param name="jobContext"></param>
        /// <param name="processConfig"></param>
        private static void SingleChannelProcess(JobContext jobContext, SSBProcessConfig processConfig)
        {
            SSBProcessBase processer = SSBProcessFactory.Create(processConfig.SSBChannels[0]);
            processer.RunProcess();

            OutPutLog(processer.Logs, jobContext);
        }

        /// <summary>
        /// 监控多张SSB表,每个表开启一个线程来处理
        /// </summary>
        /// <param name="jobContext"></param>
        /// <param name="processConfig"></param>
        private static void MultiChannelProcess(JobContext jobContext, SSBProcessConfig processConfig)
        {
            List<string> logs = new List<string>();

            using (ThreadWaitHandle handler = new ThreadWaitHandle(processConfig.SSBChannels.Count))
            {
                for (int i = 0; i < processConfig.SSBChannels.Count; i++)
                {
                    SSBChannel channel = processConfig.SSBChannels[i];
                    ThreadPool.QueueUserWorkItem((object obj) =>
                    {
                        try
                        {
                            SSBProcessBase processer = SSBProcessFactory.Create(channel);
                            processer.RunProcess();

                            lock (logs)
                            {
                                logs.AddRange(processer.Logs);
                            }
                        }
                        finally
                        {
                            handler.ReleaseOne();
                        }
                    });
                }
            }

            OutPutLog(logs, jobContext);
        }

        public static void OutPutLog(List<string> logs, JobContext jobContext)
        {
            if (logs.Count > 0)
            {
                string message = string.Join(Environment.NewLine, logs.ToArray());
                jobContext.Message = message;

                Console.WriteLine(message);
            }
        }
    }
}
