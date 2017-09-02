using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using SendMailARAmtOfVIPCustomer.Biz.Entities;
using System.Configuration;
using Newegg.Oversea.Framework.ExceptionBase;
using SendMailARAmtOfVIPCustomer.Biz.Common;
using SendMailARAmtOfVIPCustomer.Biz.ServiceAdapter;
using SendMailARAmtOfVIPCustomer.Biz.DataAccess;
using System.Data;
using Newegg.Oversea.Framework.EmailService.ServiceInterfaces;
using SendMailARAmtOfVIPCustomer.Biz.Utilities;
using ECCentral.BizEntity.Common;
using System.IO;

namespace SendMailARAmtOfVIPCustomer.Biz.Processor
{
    public class SendMailARAmtOfVIPCustomerBP
    {
        /// <summary>
        /// JOB Start point
        /// </summary>
        /// <param name="jobContext"></param>
        public static void Start(JobContext jobContext)
        {
            //初始化数据
            List<ARAmtOfVIPCustomerEntity> queryResult = SendMailARAmtOfVIPCustomerDA.GetARAmtOfVIPCustomerData();

            //处理结果
            SendMail(queryResult);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="dataTable"></param>
        private static void SendMail(List<ARAmtOfVIPCustomerEntity> entities)
        {
            MailInfo mail = new MailInfo();

            string fileName = string.Format("ARAmtOfVIPCustomer_{0}.xls", DateTime.Now.ToString(JobConfig.DateFormat));
            //byte[] excelContent = GetExcelContent(entities);
            //MailAttachment mailAttachment = new MailAttachment(fileName, excelContent, MediaType.Other);
            string filePath = new ExcelSend().WriteToFile(entities, fileName);

            mail.FromName = JobConfig.MailSendAddress;
            mail.ToName = JobConfig.MailRecvAddress;
            mail.Subject = string.Format(JobConfig.MailSubject, DateTime.Now.ToString(JobConfig.DateFormat));
            mail.Body = JobConfig.MailBody;
            mail.IsHtmlType = false;

            mail.Attachments = new List<string>();
            mail.Attachments.Add(filePath);
            mail.IsAsync = false;
            mail.IsInternal = true;

            MailHelper.SendEmail(mail);
        }

        /// <summary>
        /// 获取Excel附件字节阵列
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private static byte[] GetExcelContent(List<ARAmtOfVIPCustomerEntity> entities)
        {
            ExcelSend excelSend = new ExcelSend();
            byte[] result = excelSend.Getbytes(entities);

            return result;
        }

    }
}
