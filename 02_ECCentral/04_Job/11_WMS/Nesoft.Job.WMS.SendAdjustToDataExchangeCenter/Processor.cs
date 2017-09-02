using Nesoft.Job.WMS.Common;
using Nesoft.Job.WMS.Common.Entity;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Nesoft.Utility;

namespace Nesoft.Job.WMS.SendAdjustToDataExchangeCenter
{
    public class Processor : IJobAction
    {
        private JobContext m_JobContext;
        public void Run(JobContext context)
        {
            m_JobContext = context;
            var list = AdjustDA.QueryAdjust();
            int successCount = 0;
            foreach (var item in list)
            {
                if (SendAdjust(item))
                {
                    successCount++;
                }
            }
            string message = string.Format("共需要处理的数据有：{0}个，成功{1}个，失败{2}个", list.Count, successCount, list.Count - successCount);
            WriteLog(message, "MainMessage");
        }
        /// <summary>
        /// 发送损益单到数据交换中心 
        /// </summary>
        /// <param name="adjustInfo"></param>
        private bool SendAdjust(AdjustInfo adjustInfo)
        {
            var result = RestfulClient.PostJson<string>(ConfigurationManager.AppSettings["AdjustRestfulUrl"], "causticExcessive", adjustInfo, (message) =>
            {
                WriteLog(message, "SendRequest");
            });
            var success = result.Code == RestfulCode.Success;//200成功
            if (success)
            {
                AdjustDA.UpdateAdjustStatus(adjustInfo.SysNo);
                //记录成功日志
                WriteLog("损益单【" + adjustInfo.AdjustID + "】[" + adjustInfo.ItemCode + "]发送至数据交换中心成功", "Success");
            }
            else
            {
                //记录失败日志
                WriteLog("损益单【" + adjustInfo.AdjustID + "】[" + adjustInfo.ItemCode + "]发送至数据交换中心失败【" + SerializationUtility.JsonSerialize(result) + "】", "Failed");
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
