using System;
using System.Threading.Tasks;

using Newegg.Oversea.Framework.EmailService;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.EmailCenterService;
using Newegg.Oversea.Silverlight.ControlPanel.Service.Transformers;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess;
using System.Transactions;
using System.Collections.Generic;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Service.Resources;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.Utilities;
using System.Configuration;
using System.IO;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess
{
    public static class MailBiz
    {
        private static object s_syncObj = new object();

        public static string UploadPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["UploadFilePath"] != null)
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["UploadFilePath"]);
                }
                else
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadFile");
                }
            }
        }

        /// <summary>
        /// 发送业务邮件
        /// </summary>
        /// <param name="message"></param>
        public static void SendBusinessMail(MailMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("MailMessage");
            }

            if (string.IsNullOrWhiteSpace(message.TemplateID))
            {
                GeneralValidate(message);
            }
            else
            {
                TemplateValidate(message);
            }


            //如果没有使用SendMail的页面进行发送邮件，则需要先创建Mail的记录到DB中，
            //反之不需要创建直接把MessageID付给当前的id.
            string id;
            if (string.IsNullOrWhiteSpace(message.MessageID))
            {
                id = LogMail(message, null);
            }
            else
            {
                id = message.MessageID;
            }
            //END


            //如果存在附件或者SysmteID为空，则使用Oversea提供的邮件服务进行发送邮件
            //如果不存在，则使用London II的邮件服务发送邮件
            if (string.IsNullOrWhiteSpace(message.SystemID) || (message.Attachments != null && message.Attachments.Count > 0))
            {
                var b = MailProvider.Oversea.SendMail(message.ToMailContract(), false);
                if (b)
                {
                    message.IsSent = true;
                }
                message.IsSentByTemplate = false;
                UpdateMailMessage(message);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(message.TemplateID))
                {
                    message.IsSentByTemplate = false;
                    MailProvider.London.SendEmail(message.ToEmailMessage());
                }
                else
                {
                    message.IsSentByTemplate = true;
                    MailProvider.London.SendEmailByTemplate(message.ToTemplateEmailMessage(), message.MailTemplateVariables.ToKeyValue());
                }

                message.IsSent = true;
                UpdateMailMessage(message);
            }
        }

        /// <summary>
        /// 发送内部邮件
        /// </summary>
        /// <param name="message"></param>
        public static bool SendInternalMail(MailMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("MailMessage");
            }

            GeneralValidate(message);

            return MailProvider.Oversea.SendMail(message.ToMailContract(), false);
        }

        /// <summary>
        /// 记录MailMessage的信息到DB
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string LogMail(MailMessage message, MailPageSetting setting)
        {
            if (message == null)
            {
                throw new ArgumentNullException("MailMessage");
            }

            using (var scope = new TransactionScope())
            {
                if (!string.IsNullOrWhiteSpace(message.TemplateID))
                {
                    message.IsSentByTemplate = true;
                }
                else
                {
                    message.IsSentByTemplate = false;
                }

                //记录MailMessage的信息
                message.MessageID = MailDA.LogMailMessage(message);

                //记录BusinessNumber的信息
                if (message.BusinessNumberList != null && message.BusinessNumberList.Count > 0)
                {
                    MailDA.LogBusinessNumber(message.MessageID, message.BusinessNumberList);
                }
                //记录MailTemplateVariable的信息
                if (!string.IsNullOrWhiteSpace(message.TemplateID) && message.MailTemplateVariables != null)
                {
                    MailDA.LogTemplateVariable(message.MessageID, message.MailTemplateVariables);
                }

                //记录PageSetting的信息
                if (setting != null)
                {
                    MailDA.LogPageSetting(message.MessageID, setting);
                }

                scope.Complete();
            }
            return message.MessageID;
        }

        /// <summary>
        /// 更新MailMessage的状态
        /// </summary>
        /// <param name="messageID"></param>
        /// <param name="isSent"></param>
        public static void UpdateMailMessage(MailMessage message)
        {
            if (message != null)
            {
                MailDA.UpdateMailMessage(message);
            }
        }

        /// <summary>
        /// 根据MessageID获取Mail的相关信息和PageSetting的信息
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public static MailPageMessage GetMailMessage(string messageID)
        {
            MailPageMessage result = new MailPageMessage();
            MailMessage message = null;

            Parallel.Invoke(
                () =>
                {
                    message = MailDA.GetMailMessage(messageID);
                    if (message != null)
                    {
                        message.BusinessNumberList = MailDA.GetBusinessNumber(messageID);
                    }
                },
                () =>
                {
                    result.MailPageSetting = MailDA.GetPageSetting(messageID);
                });

            //如果使用模板方式发送邮件，则需要调用London II的服务获取模板内容
            if (message != null && !string.IsNullOrWhiteSpace(message.TemplateID))
            {
                var templateIDs = message.TemplateID.Split(',');
                var collection = new List<MailMessage>();

                //并行获取所有TemplateID的内容
                Parallel.ForEach(templateIDs, (templateID) =>
                {
                    var msg = SerializationUtility.XmlDeserialize<MailMessage>(SerializationUtility.XmlSerialize(message));

                    var variable = MailDA.GetTemplateVariable(messageID);
                    msg.MailTemplateVariables = variable;
                    msg.TemplateID = templateID;

                    var query = new GetEmailContentMessageV10();
                    query.SystemID = msg.SystemID;
                    query.TemplateID = templateID;
                    query.CompanyCode = msg.CompanyCode;
                    query.CountryCode = msg.CountryCode;
                    query.LanguageCode = msg.LanguageCode;

                    var content = MailProvider.London.GetEmailContentByTemplate(query, variable.ToKeyValue());

                    if (content != null)
                    {
                        msg.From = content.FromName;
                        msg.Subject = content.Subject;
                        msg.Body = content.Body;
                    }

                    lock (s_syncObj)
                    {
                        collection.Add(msg);
                    }
                });

                result.MailCollection = collection;

                if (collection.Count > 0)
                {
                    result.MailMessage = collection[0];
                }
            }
            else
            {
                result.MailMessage = message;
            }

            return result;
        }


        public static void BatchSend(List<MailMessage> msgs)
        {
            if (msgs == null)
            {
                throw new ArgumentNullException("MailMessage");
            }

            foreach (var message in msgs)
            {
                GetAttachments(message);

                SendBusinessMail(message);
            }
        }

        private static void GetAttachments(MailMessage message)
        {
            var uploadPath = Path.Combine(UploadPath, message.MessageID);
            if (Directory.Exists(uploadPath))
            {
                var files = Directory.GetFiles(uploadPath, "*.*", SearchOption.TopDirectoryOnly);

                if (files.Length > 0)
                {
                    if (message.Attachments == null)
                    {
                        message.Attachments = new List<MailAttachment>();
                    }

                    foreach (var file in files)
                    {
                        if (File.Exists(file))
                        {
                            var fileInfo = new FileInfo(file);
                            var att = new MailAttachment();
                            att.FileName = fileInfo.Name;
                            att.MediaType = MediaType.Other;

                            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                att.FileContent = GetFileContent(fs);
                            }

                            //删除临时文件
                            File.Delete(file);

                            message.Attachments.Add(att);
                        }
                    }
                }

                //删除临时目录
                Directory.Delete(uploadPath);
            }
        }

        private static byte[] GetFileContent(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static void TemplateValidate(MailMessage message)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(message.To))
            {
                sb.Append(InfoMessage.Validation_Required_To);
                sb.Append("\r\n");
            }
            if (string.IsNullOrWhiteSpace(message.CompanyCode))
            {
                sb.Append(InfoMessage.Validation_Required_CompanyCode);
                sb.Append("\r\n");
            }
            if (string.IsNullOrWhiteSpace(message.CountryCode))
            {
                sb.Append(InfoMessage.Validation_Required_CountryCode);
                sb.Append("\r\n");
            }
            if (string.IsNullOrWhiteSpace(message.DomainName))
            {
                sb.Append(InfoMessage.Validation_Required_DomainName);
                sb.Append("\r\n");
            }
            if (string.IsNullOrWhiteSpace(message.LanguageCode))
            {
                sb.Append(InfoMessage.Validation_Required_LanguageCode);
                sb.Append("\r\n");
            }

            if (sb.Length > 0)
            {
                throw new BusinessException(sb.ToString());
            }
        }

        private static void GeneralValidate(MailMessage message)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(message.From))
            {
                sb.Append(InfoMessage.Validation_Required_From);
                sb.Append("\r\n");
            }
            if (string.IsNullOrWhiteSpace(message.To))
            {
                sb.Append(InfoMessage.Validation_Required_To);
                sb.Append("\r\n");
            }
            if (string.IsNullOrWhiteSpace(message.Subject))
            {
                sb.Append(InfoMessage.Validation_Required_Subject);
            }

            if (sb.Length > 0)
            {
                throw new BusinessException(sb.ToString());
            }
        }
    }

    internal static class MailProvider
    {
        private static object m_syncObj = new object();
        private static MailManager m_oversea;
        private static EmailServiceCenterProxyClient m_london;

        internal static MailManager Oversea
        {
            get
            {
                if (m_oversea == null)
                {
                    lock (m_syncObj)
                    {
                        if (m_oversea == null)
                        {
                            m_oversea = new MailManager();
                        }
                    }
                }
                return m_oversea;
            }
        }

        internal static EmailServiceCenterProxyClient London
        {
            get
            {
                if (m_london == null)
                {
                    lock (m_syncObj)
                    {
                        if (m_london == null)
                        {
                            m_london = new EmailServiceCenterProxyClient();
                        }
                    }
                }
                return m_london;
            }
        }
    }
}
