using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using System.Collections.Generic;
using System.Transactions;
using System;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [RestService]
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    public class MailRestService
    {
        private static readonly string Sessionkey_IsCloseMailPage = "Sessionkey_IsCloseMailPage_{0}";

        [WebInvoke(UriTemplate = "SendBusinessEmail", Method = "POST")]
        public void SendBusinessEmail(MailMessage message)
        {
            MailBiz.SendBusinessMail(message);
        }

        [WebInvoke(UriTemplate = "SendInternalMail", Method = "POST")]
        public bool SendInternalMail(MailMessage message)
        {
            return MailBiz.SendInternalMail(message);
        }

        [WebInvoke(UriTemplate = "LogMail", Method = "POST")]
        public string LogMail(MailPageMessage message)
        {
            var result = MailBiz.LogMail(message.MailMessage, message.MailPageSetting);

            if (!string.IsNullOrWhiteSpace(result))
            {
                var key = string.Format(Sessionkey_IsCloseMailPage, result);
                HttpContext.Current.Session[key] = false;
            }

            return result;
        }

        [WebInvoke(UriTemplate = "CloseMailPage", Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void CloseMailPage(string messageID)
        {
            if (!string.IsNullOrWhiteSpace(messageID))
            {
                var mail = MailBiz.GetMailMessage(messageID);

                if (mail != null && mail.MailMessage != null && !mail.MailMessage.IsSent)
                {
                    var key = string.Format(Sessionkey_IsCloseMailPage, messageID);
                    HttpContext.Current.Session[key] = true;
                }
            }
        }

        [WebGet(UriTemplate = "GetMailStatus/{messageID}")]
        public bool? GetMailStatus(string messageID)
        {
            if (!string.IsNullOrWhiteSpace(messageID))
            {
                var status = MailDA.GetMailStatus(messageID);
                if (status)
                    return status;
                else
                {
                    var key = string.Format(Sessionkey_IsCloseMailPage, messageID);
                    if (HttpContext.Current.Session[key] != null && (bool)HttpContext.Current.Session[key])
                    {
                        return false;
                    }
                }
            }
            return null;
        }

        [WebInvoke(UriTemplate = "GetBatchMailStatus", Method = "POST")]
        public Dictionary<string, bool?> GetBatchMailStatus(List<string> messageIDs)
        {
            if (messageIDs == null)
            {
                return null;
            }

            var key = string.Format(Sessionkey_IsCloseMailPage, messageIDs[0]);
            var dic = new Dictionary<string, bool?>();

            if (HttpContext.Current.Session[key] != null && (bool)HttpContext.Current.Session[key])
            {
                foreach (var id in messageIDs)
                {
                    dic[id] = false;
                }
            }
            else
            {
                foreach (var id in messageIDs)
                {
                    var val = MailDA.GetMailStatus(id);
                    if (val)
                    {
                        dic[id] = true;
                    }
                    else
                    {
                        dic[id] = null;
                    }
                }
            }
            return dic;
        }

        [WebGet(UriTemplate = "GetMailMessage/{messageID}", ResponseFormat = WebMessageFormat.Json)]
        public List<MailPageMessage> GetMailMessage(string messageID)
        {
            if (string.IsNullOrEmpty(messageID))
            {
                return null;
            }
            var result = new List<MailPageMessage>();
            var messages = messageID.Split(',');

            foreach (var id in messages)
            {
                result.Add(MailBiz.GetMailMessage(id));
            }

            return result;
        }

        [WebInvoke(UriTemplate = "Send", Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        public SendResult Send(List<MailMessage> msgs)
        {
            try
            {
                MailBiz.BatchSend(msgs);
                return new SendResult { IsSuccess = true };
            }
            catch (Exception e)
            {
                return new SendResult { Description = e.Message, IsSuccess = false };
            }
        }
    }
}
