using System;
using System.Collections.Generic;
using System.Text;
using AutoSetResponsibleUser.Common;
using AutoSetResponsibleUser.DataAccessLayer;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ServiceConsole.Client;

namespace AutoSetResponsibleUser.BusinessLayer
{
    internal sealed class AutoSetResponsiblerUserBP
    {
        #region 字段

        private AutoSetResponsibleUserDAL dal = new AutoSetResponsibleUserDAL();

        public Action<string> DisplayMessage;

        #endregion

        #region 发送邮件

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        public void SendMail(string mailSubject, string mailBody)
        {
            dal.SendEmail(GlobalSettings.MailAddress, mailSubject, mailBody, 0, GlobalSettings.CompanyCode);
        }

        #endregion

        #region 显示消息

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message">消息字符串</param>
        private void OnDisplayMessage(string message)
        {
            if (this.DisplayMessage != null)
            {
                DisplayMessage(message);
            }
        }

        #endregion

        #region 调用Invoice的服务新增单据跟踪信息

        /// <summary>
        /// 调用Invoice的服务新增单据跟踪信息
        /// </summary>
        public void CreateTrackingInfo()
        {
            var trackingList = dal.GetTrackingInfo();
            var request = trackingList.ConvertAll<TrackingInfo>(x =>
            {
                return new TrackingInfo
                    {
                        OrderSysNo = x.OrderSysNo,
                        OrderType = SOIncomeOrderType.SO,
                        IncomeAmt = x.IncomeAmt,
                        CompanyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"]
                    };
            });

            CallService(request);
        }

        #endregion

        #region 调用Invoice的服务

        /// <summary>
        /// 调用Invoice的服务新增单据跟踪信息
        /// </summary>
        /// <param name="trackingInfoList">跟踪信息列表</param>
        private void CallService(List<TrackingInfo> trackingInfoList)
        {
            if (trackingInfoList == null || trackingInfoList.Count == 0)
            {
                return;
            }
            
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["InvoiceMgmtRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            string relativeUrl = "/Job/BatchCreateTracking";
            string resultMsg = string.Empty;

            var ar = client.Update<string>(relativeUrl, trackingInfoList, out resultMsg, out error);

            if (!string.IsNullOrEmpty(resultMsg))
            {
                OnDisplayMessage(resultMsg);
            }

            var messageBuilder = new StringBuilder();
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                foreach (var errorItem in error.Faults)
                {
                    messageBuilder.AppendFormat(" 错误： {0} <br/>", errorItem.ErrorDescription);
                }
            }

            if (messageBuilder.Length > 0)
            {
                SendMail(GlobalSettings.MailSubject, messageBuilder.ToString());
                throw new Exception(messageBuilder.ToString());
            }

        }

        #endregion
    }
}
