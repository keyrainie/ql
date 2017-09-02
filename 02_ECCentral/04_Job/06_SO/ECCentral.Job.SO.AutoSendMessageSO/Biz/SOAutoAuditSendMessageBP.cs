using System;
using System.Collections.Generic;
using System.Configuration;
using ECCentral.Job.SO.AutoSendMessageSO.Dac.AutoAudit;
using ECCentral.Job.SO.AutoSendMessageSO.Utilities;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace ECCentral.Job.SO.AutoSendMessageSO.Biz.AutoAudit
{
    public class SOAutoAuditSendMessageBP
    {
        /// <summary>
        /// 自动审单时审单人编号
        /// </summary>
        private static int AuditUserSysNo = 0;
        /// <summary>
        /// 自动审核一次最多提取的单数
        /// </summary>
        private static int TopCount = 2000;
        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;

        //以下五项为邮件发送JOB中调用WCF服务时所使用到的信息
        private static string SendMailUserDisplayName;
        private static string SendMailUserLoginName;
        private static string SendMailCompanyCode;
        private static string SendMailStoreCompanyCode;
        private static string SendMailStoreSourceDirectoryKey;

        /// <summary>
        /// 从配置文件获取为邮件发送JOB中调用WCF服务时所使用到的信息
        /// </summary>
        /// <returns></returns>
        private static void GetAutoAuditSendMessageLoginInfo()
        {
            SendMailUserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
            SendMailUserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
            SendMailCompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
            SendMailStoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
            SendMailStoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
        }

        /// <summary>
        /// 获取一次对审核通过的订单发邮件的条数
        /// </summary>
        /// <returns></returns>
        private static int GetAutoAuditSendMessageTopCount()
        {
            string tmpAuditUserSysNo = ConfigurationManager.AppSettings["AutoAuditSendMessageTopCount"];
            int topCount = int.Parse(tmpAuditUserSysNo);


            return (topCount > 0) ? topCount : 2000;
        }
        /// <summary>
        /// 获取自动审单时的用户编号
        /// </summary>
        /// <returns></returns>
        private static int GetAutoAuditUserSysNo()
        {
            string tmpAuditUserSysNo = ConfigurationManager.AppSettings["AuditUserSysNo"];
            return int.Parse(tmpAuditUserSysNo);
        }
        public static void SendMessage(JobContext jobContext)
        {
            BizLogFile = jobContext.Properties["BizLog"];
            //从配置文件中获取自动审核一次最多提取的单数
            TopCount = GetAutoAuditSendMessageTopCount();
            //从配置文件获取为邮件发送JOB中调用WCF服务时所使用到的信息
            GetAutoAuditSendMessageLoginInfo();
            //自动审单时审单人编号
            AuditUserSysNo = GetAutoAuditUserSysNo();
            List<int> sendMailSysNoList = SOAutoAuditSendMessageDA.GetSOList4Audit2SendMessage(TopCount, SendMailCompanyCode);
            if (sendMailSysNoList.Count > 0)
            {
                jobContext.Message += string.Format("共获取到{0}条附合条件的记录\r\n", sendMailSysNoList.Count);
                WriteLog(string.Format("共获取到{0}条附合条件的记录\r\n", sendMailSysNoList.Count));

                int successCount = 0;

                foreach (int x in sendMailSysNoList)
                {
                    try
                    {
                        SendMessageSO(x);
                        successCount++;
                        jobContext.Message += string.Format("已为审单通过成功发送邮件，单号：{0}\r\n", x);
                        WriteLog(string.Format("已为审单通过成功发送邮件，单号：{0}", x));
                    }
                    catch (Exception ex)
                    {
                        jobContext.Message += string.Format("发送邮件异常,单号：{0},异常：{1}\r\n", x, ex.Message);
                        WriteLog(string.Format("发送邮件异常,单号：{0},异常：{1}", x, ex.Message));
                    }

                }

                jobContext.Message += string.Format("执行结束，成功{0}条,失败{1}条\r\n",
                    successCount, sendMailSysNoList.Count - successCount);
                WriteLog(string.Format("执行结束，成功{0}条,失败{1}条\r\n",
                    successCount, sendMailSysNoList.Count - successCount));
            }
        }
        public static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }
        public static void SendMessageSO(int soSysNo)
        {
            string baseUrl = ConfigurationManager.AppSettings["RestFulBaseUrl"];
            ECCentral.Job.Utility.RestClient client = new Utility.RestClient(baseUrl);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update(string.Format("SOService/SO/Job/AutoSendMessageSO/{0}", soSysNo), null, out error);
        }
    }
}
