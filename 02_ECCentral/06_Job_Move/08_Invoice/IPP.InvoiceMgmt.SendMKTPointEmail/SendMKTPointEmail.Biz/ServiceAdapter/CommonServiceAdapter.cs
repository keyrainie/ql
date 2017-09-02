using System;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ServiceConsole.Client;
//using SendMKTPointEmail.Biz.Common;
//using SendMKTPointEmail.Biz.Entities;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using SendMKTPointEmail.Biz.Common;

namespace SendMKTPointEmail.Biz.ServiceAdapter
{
    public class CommonServiceAdapter
    {
        #region Message Contains Mail

        /// <summary>
        /// 发送内部邮件
        /// </summary>
        /// <param name="mailList">内部邮件实体列表</param>
        public static void SendMail(List<MailInfo> mailList)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;

            foreach (MailInfo mailInfo in mailList)
            {
                mailInfo.FromName = JobConfig.SendMailAddress;

                if (string.IsNullOrEmpty(mailInfo.ToName))
                {
                    throw new Exception("收件人为空");
                }
                else
                {
                    mailInfo.ToName = mailInfo.ToName.Trim();
                }

                mailInfo.IsHtmlType = false;
                mailInfo.IsAsync = true;
                mailInfo.IsInternal = true;
                    
                var ar = client.Create("/Message/SendMail", mailInfo, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string errorMsg = "";
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }

                    throw new Exception("发送邮件异常：" + errorMsg);
                }
            }

        }
    
        #endregion
    }
}
