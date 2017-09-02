using Nesoft.Job.WMS.Common;
using Nesoft.Job.WMS.Common.Entity;
using Nesoft.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.SendSOToDataExchangeCenter
{
    public class Processor : IJobAction
    {
        private JobContext m_JobContext;
        public void Run(JobContext context)
        {
            m_JobContext = context;
            var list = SODA.QuerySO();
            int successCount = 0;
            foreach (var item in list)
            {
                if (SendSO(item))
                {
                    successCount++;
                }
            }
            string message = string.Format("共需要处理的数据有：{0}个，成功{1}个，失败{2}个", list.Count, successCount, list.Count - successCount);
            WriteLog(message, "MainMessage");
        }
        /// <summary>
        /// 发送订单信息到数据交换中心 
        /// </summary>
        /// <param name="soInfo"></param>
        private bool SendSO(SOInfo soInfo)
        {
            var result = RestfulClient.PostJson<string>(ConfigurationManager.AppSettings["SORestfulUrl"], "customerOrderOutStore", soInfo, (message) =>
            {
                WriteLog(message, "SendRequest");
            });
            var success = result.Code == RestfulCode.Success;//200成功
            if (success)
            {
                SODA.UpdateSOStatus(soInfo.SysNo);
                //记录成功日志
                WriteLog("订单【" + soInfo.SOID + "】发送至数据交换中心成功", "Success");
            }
            else
            {
                //记录失败日志
                WriteLog("订单【" + soInfo.SOID + "】发送至数据交换中心失败【" + SerializationUtility.JsonSerialize(result) + "】", "Failed");
            }
            return success;
        }

        public void WriteLog(string content, string category)
        {
            if (m_JobContext != null)
            {
                m_JobContext.Message += content + "\r\n";
            }
            Logger.WriteLog(content, category);
        }
    }
}
