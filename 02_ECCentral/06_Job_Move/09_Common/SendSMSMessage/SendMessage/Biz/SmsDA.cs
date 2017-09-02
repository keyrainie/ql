using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newegg.Oversea.Framework.DataAccess;
using SendMessage.Entity;

namespace SendMessage.Class
{
    public class SmsDA
    {

        /// <summary>
        /// 获取待发送短信列表
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public static List<SmsEntity> GetSMS2SendList(int priority, int topCount)
        {
            var command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSMS2SendList");

            command.AddInputParameter("@Priority", System.Data.DbType.Int32, priority);
            command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());

            return command.ExecuteEntityList<SmsEntity>();

        }

        /// <summary>
        /// 更新发送结果
        /// </summary>
        /// <param name="entity"></param>
        public static void UpdateResult(SmsEntity entity)
        {
            var command = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateResult");

            command.AddInputParameter("@SysNo", System.Data.DbType.Int32, entity.SysNo);

            command.ExecuteNonQuery();
        }


        public static void UpdateRetryCount(int sysno)
        {
            var command = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateRetryCount");

            command.AddInputParameter("@SysNo", System.Data.DbType.Int32, sysno);

            command.ExecuteNonQuery();

        }

        /// <summary>
        /// 记录运行日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(string msg)
        {
            try
            {
                string fileName = string.Format("{0}{1}.txt", Helper.LogFile, DateTime.Today.ToString("yyyyMMdd"));
                File.AppendAllText(fileName, string.Format("{0}=>{1}\r\n", DateTime.Now.ToString(), msg));
                
            }
            catch
            {

            }
        }

        /// <summary>
        /// 异常时发邮件
        /// </summary>
        /// <param name="mailto"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        public static void SendMail(string mailto, string subject, string content)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendMail");

            command.SetParameterValue("@MailAddress", mailto);
            command.SetParameterValue("@MailSubject", subject);
            command.SetParameterValue("@MailBody", content);
            command.ExecuteNonQuery();
        }
    }
}
