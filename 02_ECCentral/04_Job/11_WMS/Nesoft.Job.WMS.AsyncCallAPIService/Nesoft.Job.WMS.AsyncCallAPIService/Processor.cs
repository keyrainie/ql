using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Nesoft.Job.WMS.AsyncCallAPIService.Core;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace Nesoft.Job.WMS.AsyncCallAPIService
{
    public class Processor : IJobAction
    {
        #region IJobAction Members

        public void Run(JobContext context)
        {
            var activeCallLogList = CallLogService.GetActiveCallLogList();

            if (null != activeCallLogList && activeCallLogList.Count > 0)
            {
                int processedCount = 0;
                foreach (var logInfo in activeCallLogList)
                {
                    var serviceConfig = CallServicesConfigManager.ServicesConfigList.SingleOrDefault(x => x.ServiceName.ToLower() == logInfo.MethodName.ToLower());
                    if (logInfo.RetryCount >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxRetryCount"]))
                    {
                        continue;
                    }
                    //ThreadStart thread = new ThreadStart(() =>
                    //{
                    CallServiceManager serviceMgr = new CallServiceManager(logInfo, serviceConfig);
                    serviceMgr.CallServiceFinished += serviceMgr_CallServiceFinished;
                    serviceMgr.Do();
                    //});
                    //Thread thr = new Thread(thread);
                    //thr.Start();
                    processedCount++;
                }
                Console.WriteLine("成功处理了{0}个请求.", processedCount);
            }
        }

        #endregion

        static void serviceMgr_CallServiceFinished(object sender, CallServiceCompletedEventArgs e)
        {
            CallLogService.UpdateCallLogResult(
                e.LogInfo.SysNo
                , e.IsSuccess
                , e.ResponseMessage
                , e.CallTime
                , e.CostMillionSeconds
                , e.IsSuccess ? CallLogStatus.Deactive : CallLogStatus.Active
                , e.LogInfo.RetryCount + 1);
        }
    }
}
