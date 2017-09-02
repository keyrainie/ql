/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  King.B.Wu
 *  Date:    2009-12-08
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/
using System;
using System.Configuration;
using System.ServiceModel;

using IPP.Oversea.CN.ContentMgmt.GiftCard.Entities;

using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.Contract;
using ECCentral.BizEntity.Common;
using ECCentral.Job.Utility;

namespace IPP.Oversea.CN.ContentMgmt.GiftCard.ServiceAdapter
{
    public class EmailComparisonCNServiceFacade
    {
        public static void SendProductEmail(MailEntity mailEntity)
        {
            try
            {
                MailInfo mailInfor = new MailInfo();
                mailInfor.FromName = mailEntity.From;
                mailInfor.ToName = mailEntity.To;
                mailInfor.CCName = mailEntity.CC;
                mailInfor.BCCName = mailEntity.BCC;
                mailInfor.Body = mailEntity.Body;
                mailInfor.Subject = mailEntity.Subject;
                mailInfor.IsHtmlType = mailEntity.IsBodyHtml;
                mailInfor.IsAsync = true;
                mailInfor.IsInternal = true;

                if (string.IsNullOrEmpty(mailInfor.ToName)) return;
                else
                    mailInfor.ToName = mailInfor.ToName.Trim();
                string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
                string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;
                var ar = client.Create("/Message/SendMail", mailInfor, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string errorMsg = "";
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }
                    Logger.WriteLog(errorMsg, "JobConsole");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("邮件发送失败：" + ex.Message, "JobConsole");
            }
        }
    }
}