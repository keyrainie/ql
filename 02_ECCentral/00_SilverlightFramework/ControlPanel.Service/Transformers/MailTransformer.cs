using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.EmailService.ServiceInterfaces;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.EmailCenterService;


namespace Newegg.Oversea.Silverlight.ControlPanel.Service.Transformers
{
    public static class MailTransformer
    {
        public static MailContract ToMailContract(this MailMessage message)
        {
            var mail = new MailContract();

            mail.From = message.From != null ? message.From.Trim().TrimEnd(';') : string.Empty;
            mail.To = message.To != null ? message.To.Trim().TrimEnd(';') : string.Empty;
            mail.CC = message.CC != null ? message.CC.Trim().TrimEnd(';') : string.Empty;
            mail.BCC = message.BCC != null ? message.BCC.Trim().TrimEnd(';') : string.Empty;
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = message.BodyType == MailBodyType.Html;
            mail.Attachments = message.Attachments.ToContract();

            if (message.Priority == Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts.MailPriority.High)
            {
                mail.Priority = Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MailPriority.High;
            }
            else if (message.Priority == Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts.MailPriority.Low)
            {
                mail.Priority = Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MailPriority.Low;
            }
            else
            {
                mail.Priority = Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MailPriority.Normal;
            }

            return mail;
        }

        public static Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MailAttachment ToContract(this Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts.MailAttachment attachment)
        {
            return new Framework.EmailService.ServiceInterfaces.MailAttachment
            {
                FileContent = attachment.FileContent,
                FileName = attachment.FileName,
                MediaType = ((Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MediaType)((int)attachment.MediaType))
            };
        }

        public static List<Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MailAttachment> ToContract(this List<Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts.MailAttachment> attachments)
        {
            if (attachments == null)
                return null;

            var contracts = new List<Newegg.Oversea.Framework.EmailService.ServiceInterfaces.MailAttachment>();

            foreach (var item in attachments)
            {
                contracts.Add(item.ToContract());
            }

            return contracts;
        }


        public static EmailMessageV10 ToEmailMessage(this MailMessage message)
        {
            var mail = new EmailMessageV10();

            mail.FromName = message.From;
            mail.ToName = message.To;
            mail.CCName = message.CC;
            mail.BCCName = message.BCC;
            mail.ReplyName = message.ReplyName;
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.ReplyName = message.ReplyName;


            if (message.BodyType == MailBodyType.Html)
            {
                mail.HtmlType = EmailHtmlType.Html;
            }
            else if (message.BodyType == MailBodyType.Text)
            {
                mail.HtmlType = EmailHtmlType.Text;
            }

            if (message.Priority == DataContracts.MailPriority.Low)
            {
                mail.Priority = EmailPriority.Low;
            }
            else if (message.Priority == DataContracts.MailPriority.High)
            {
                mail.Priority = EmailPriority.High;
            }
            else
            {
                mail.Priority = EmailPriority.Normal;
            }

            mail.CompanyCode = message.CompanyCode;
            mail.CountryCode = message.CountryCode;
            mail.LanguageCode = message.LanguageCode;
            mail.SystemID = message.SystemID;

            if (message.BusinessNumberList != null)
            {
                var customerNumber = message.BusinessNumberList.Find(item => item.NumberType == NumberType.CustomerNumber);
                var number = 0;
                if (customerNumber != null && int.TryParse(customerNumber.NumberValue, out number))
                {
                    mail.CustomerNumber = number;
                }
            }
            return mail;
        }

        public static TemplateEmailMessageV10 ToTemplateEmailMessage(this MailMessage message)
        {
            var template = new TemplateEmailMessageV10();

            template.ToName = message.To;
            template.CCName = message.CC;
            template.BCCName = message.BCC;
            template.CompanyCode = message.CompanyCode;
            template.CountryCode = message.CountryCode;
            template.LanguageCode = message.LanguageCode;
            template.SystemID = message.SystemID;
            template.TemplateID = message.TemplateID;

            if (message.BusinessNumberList != null)
            {
                var customerNumber = message.BusinessNumberList.Find(item => item.NumberType == NumberType.CustomerNumber);
                var number = 0;
                if (customerNumber != null && int.TryParse(customerNumber.NumberValue, out number))
                {
                    template.CustomerNumber = number;
                }
            }

            return template;
        }

        public static List<KeyValue> ToKeyValue(this List<MailTemplateVariable> variables)
        {
            if (variables == null)
            {
                return null;
            }

            var keyValue = new List<KeyValue>();

            foreach (var variable in variables)
            {
                keyValue.Add(new KeyValue { Key = variable.Key, Value = variable.Value });
            }

            return keyValue;
        }
    }
}
