using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using System.Threading;
using System.Net.Mail;
using System.Net;
using ECCentral.Service.Common.BizProcessor;
using System.Collections;
using System.IO;

namespace ECCentral.Service.Common.AppService
{
    /// <summary>
    /// 邮件服务类
    /// </summary>
    [VersionExport(typeof(IEmailSend))]
    public class EmailService : IEmailSend
    {
        #region IEmailSend Members

        public void SendMail(ECCentral.Service.Utility.MailMessage mailMessage, bool isAsync, bool isInternal)
        {
            if (isAsync)
            {
                //发送异步邮件
                if (isInternal)
                {
                    ObjectFactory<EmailProcessor>.Instance.SendAsyncMailInternal(mailMessage);
                }
                else
                {
                    ObjectFactory<EmailProcessor>.Instance.SendAsyncMail(mailMessage);
                }
            }
            else
            {
                //发送同步邮件
                TCPMail tcpMail = new TCPMail();
                tcpMail.Priority = (System.Net.Mail.MailPriority)Enum.Parse(typeof(System.Net.Mail.MailPriority), mailMessage.Priority.ToString());
                tcpMail.Html = mailMessage.IsHtmlType;

                bool isSucc = tcpMail.Send(mailMessage.ToName, mailMessage.CCName, mailMessage.BCCName, mailMessage.Subject, mailMessage.Body, mailMessage.Attachments, mailMessage.FromName, mailMessage.DisplaySenderName);
                if (!isSucc)
                {
                    throw new ApplicationException("发送邮件失败!");
                }
            }
        }

        #endregion
    }

    internal class TCPMail
    {
        #region Private
        /// <summary> 
        /// 发件人
        /// </summary> 
        private string mMailFrom = "";
        /// <summary> 
        /// 发件人显示名
        /// </summary> 
        private string mMailDisplyName = "";
        /// <summary> 
        /// 收件人列表
        /// </summary> 
        private string mMailTo;
        /// <summary> 
        /// 抄送人列表
        /// </summary> 
        private string mMailCc;
        /// <summary> 
        /// 暗抄送人列表
        /// </summary> 
        private string mMailBcc;
        /// <summary>
        /// 邮件主题
        /// </summary>
        private string mMailSubject = "";
        /// <summary>
        /// 邮件正文
        /// </summary>
        private string mMailBody = "";
        private System.Collections.ArrayList mMailAttachments;
        /// <summary> 
        /// 邮件服务器域名 
        /// </summary>    
        private string mSMTPServer = "";
        /// <summary> 
        /// 邮件服务器端口号 
        /// </summary>    
        private int mSMTPPort;
        /// <summary> 
        /// SMTP认证时使用的用户名 
        /// </summary> 
        private string mSMTPUsername = "";
        /// <summary> 
        /// SMTP认证时使用的密码 
        /// </summary> 
        private string mSMTPPassword = "";
        private bool mSMTPSSL;
        /// <summary> 
        /// 邮件发送优先级 
        /// </summary> 
        private MailPriority mPriority = MailPriority.Normal;
        /// <summary>
        /// 邮件发送格式
        /// </summary>
        private bool mIsBodyHtml = true;
        private bool mIsInternalMail = true;
        bool mailSent = false;

        #endregion

        #region Public
        /// <summary> 
        /// 设定语言代码，默认设定为GB2312，如不需要可设置为"" 
        /// </summary> 
        private System.Text.Encoding charset = System.Text.Encoding.GetEncoding("GB2312");
        public System.Text.Encoding Charset
        {
            set
            {
                charset = value;
            }
            get
            {
                return charset;
            }
        }
        /// <summary> 
        /// 发件人地址 
        /// </summary> 
        public string From
        {
            set { mMailFrom = value; }
            get { return mMailFrom; }
        }
        /// <summary> 
        /// 发件人姓名 
        /// </summary>
        public string FromName
        {
            set { mMailDisplyName = value; }
            get { return mMailDisplyName; }
        }
        /// <summary> 
        /// 是否Html邮件 
        /// </summary>   
        public bool Html
        {
            get { return mIsBodyHtml; }
            set { mIsBodyHtml = value; }
        }
        /// <summary> 
        /// 邮件主题 
        /// </summary>      
        public string Subject
        {
            set { mMailSubject = value; }
            get { return mMailSubject; }
        }
        /// <summary> 
        /// 邮件正文 
        /// </summary>       
        public string Body
        {
            set { mMailBody = value; }
            get { return mMailBody; }
        }
        /// <summary> 
        /// 邮件服务器端口号 
        /// </summary>    
        public int MailDomainPort
        {
            set
            {
                mSMTPPort = value;
            }
        }
        /// <summary> 
        /// SMTP认证时使用的用户名 
        /// </summary> 
        public string MailServerUserName
        {
            set { mSMTPUsername = value; }
            get { return mSMTPUsername; }
        }

        /// <summary> 
        /// SMTP认证时使用的密码 
        /// </summary> 
        public string MailServerPassWord
        {
            set { mSMTPPassword = value; }
            get { return mSMTPPassword; }
        }
        /// <summary> 
        /// 邮件发送优先级
        /// </summary> 
        public MailPriority Priority
        {
            get { return mPriority; }
            set { mPriority = value; }
        }

        /// <summary>
        /// 是否是内部邮件
        /// </summary>
        public bool IsInternalMail
        {
            get { return mIsInternalMail; }
            set { mIsInternalMail = value; }
        }

        // Modify by tomato 2006-01-12 mail 乱码 Begin
        /// <summary> 
        /// 邮件发送错误信息
        /// </summary> 
        public string errmsg = "";
        // Modify by tomato 2006-01-12 mail 乱码 End
        #endregion

        private System.Net.Mail.MailMessage mailObject;

        public TCPMail()
        {
            mailObject = new System.Net.Mail.MailMessage();
            mMailAttachments = new ArrayList();

            Charset = System.Text.Encoding.GetEncoding(GetSMTPMailConfigValue("Email_MailCharset"));
            mMailFrom = GetSMTPMailConfigValue("Email_MailFrom");
            mMailDisplyName = GetSMTPMailConfigValue("Email_MailFromName");
            mSMTPServer = GetSMTPMailConfigValue("Email_MailServer");
            mSMTPUsername = GetSMTPMailConfigValue("Email_MailUserName");
            mSMTPPassword = GetSMTPMailConfigValue("Email_MailUserPassword");
            mSMTPPort = 25;
            mMailTo = null;
            mMailCc = "";
            mMailBcc = "";
            mMailSubject = null;
            mMailBody = null;
            mSMTPSSL = false;
        }

        #region Methods

        public bool Send(string address, string subject, string body)
        {
            mMailTo = address;
            mMailSubject = subject;
            mMailBody = body;
            return Send();
        }

        public bool Send(string Toaddress, string Ccaddress, string Bccaddress, string subject, string body)
        {
            mMailTo = Toaddress;
            mMailCc = Ccaddress;
            mMailBcc = Bccaddress;
            mMailSubject = subject;
            mMailBody = body;
            return Send();
        }

        public bool Send(string Toaddress, string Ccaddress, string Bccaddress, string subject, string body, string mailFrom, string mailSenderName)
        {
            if (!string.IsNullOrEmpty(mailFrom)) this.mMailFrom = mailFrom;
            if (!string.IsNullOrEmpty(mailSenderName)) this.mMailDisplyName = mailSenderName;
            return Send(Toaddress, Ccaddress, Bccaddress, subject, body);
        }

        public bool Send(string Toaddress, string Ccaddress, string Bccaddress, string subject, string body, List<string> attachments, string mailFrom, string mailSenderName)
        {
            if (null != attachments)
            {
                attachments.ForEach(delegate(string path)
                {
                    mMailAttachments.Add(path);
                });
            }
            return Send(Toaddress, Ccaddress, Bccaddress, subject, body, mailFrom, mailSenderName);
        }

        /// <summary>
        /// 读取邮件模版生成邮件主体
        /// </summary>
        /// <param name="strFileName">模版文件的物理地址</param>
        private void ReadModel(string strFileName)
        {
            // 清空邮件主体
            mMailBody = "";

            // 判断文件是否存在
            if (File.Exists(strFileName))
            {
                try
                {
                    // 读取文件
                    using (StreamReader objStreamReader = new StreamReader(strFileName, Encoding.GetEncoding("GB2312")))
                    {
                        string tempLine = "";
                        StringBuilder objStringBuilder = new System.Text.StringBuilder();
                        // 将文件流的数据存储到StringBuilder里面
                        while ((tempLine = objStreamReader.ReadLine()) != null)
                        {
                            objStringBuilder.Append(tempLine + "\r\n");
                        }
                        // 存储到邮件主体中
                        mMailBody = objStringBuilder.ToString();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHelper.HandleException(ex, "读取邮件模板失败", new object[] { strFileName });
                    // 异常的时候清空邮件主体
                    mMailBody = "";
                }
            }
        }

        /// <summary>s
        /// 同步发送邮件
        /// </summary>
        /// <returns></returns>
        private Boolean Send()
        {
            return SendMail(null);
        }

        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="userState">异步任务的唯一标识符</param>
        /// <returns></returns>
        private void SendAsync(object userState)
        {
            SendMail(userState);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="isAsync">是否异步发送邮件</param>
        /// <param name="userState">异步任务的唯一标识符，当 isAsync 为 True 时必须设置该属性， 当 isAsync 为 False 时可设置为 null</param>
        /// <returns></returns>
        private bool SendMail(object userState)
        {
            try
            {
                #region 设置属性值

                string[] mailTos = mMailTo.Split(';');
                string[] mailCcs = (string.IsNullOrEmpty(mMailCc) ? null : mMailCc.Split(';'));
                string[] mailBccs = (string.IsNullOrEmpty(mMailBcc) ? null : mMailBcc.Split(';'));
                System.Collections.ArrayList attachments = mMailAttachments;

                // build the email message
                System.Net.Mail.MailMessage Email = new System.Net.Mail.MailMessage();
                MailAddress MailFrom =
                  new MailAddress(mMailFrom, mMailDisplyName, Encoding.GetEncoding("gb2312"));
                Email.From = MailFrom;

                if (mailTos != null)
                {
                    foreach (string mailto in mailTos)
                    {
                        if (!string.IsNullOrEmpty(mailto))
                        {
                            Email.To.Add(mailto);
                        }
                    }
                }

                if (mailCcs != null)
                {
                    foreach (string cc in mailCcs)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            Email.CC.Add(cc);
                        }
                    }
                }

                if (mailBccs != null)
                {
                    foreach (string bcc in mailBccs)
                    {
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            Email.Bcc.Add(bcc);
                        }
                    }
                }

                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Count; i++)
                    {
                        string file = attachments[i].ToString();
                        if (!string.IsNullOrEmpty(file))
                        {
                            Attachment att = new Attachment(file);
                            Email.Attachments.Add(att);
                        }
                    }
                }

                Email.Subject = mMailSubject;
                Email.Body = mMailBody;
                Email.Priority = mPriority;
                Email.IsBodyHtml = mIsBodyHtml;
                //Email.BodyEncoding = charset;
                //Email.SubjectEncoding = charset;

                Email.BodyEncoding = Encoding.GetEncoding("GB2312");
                Email.SubjectEncoding = Encoding.GetEncoding("GB2312");

                // Smtp Client
                SmtpClient SmtpMail =
                 new SmtpClient(mSMTPServer, mSMTPPort);
                SmtpMail.Credentials =
                 new NetworkCredential(mSMTPUsername, mSMTPPassword);
                SmtpMail.EnableSsl = mSMTPSSL;

                //SmtpMail.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                #endregion

                SmtpMail.Send(Email);
                mailSent = true;
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex, "插入邮件任务进数据库失败",
                    new List<ExtendedPropertyData>() 
                    {
                        new ExtendedPropertyData("Mail To", mMailTo),
                        new ExtendedPropertyData("Mail Cc", mMailCc),
                        new ExtendedPropertyData("Mail BCc", mMailBcc),
                        new ExtendedPropertyData("Mail From", mMailFrom),
                        new ExtendedPropertyData("Mail From DisplyName", mMailDisplyName),
                        new ExtendedPropertyData("Mail Subject", mMailSubject)
                    });
                mailSent = false;
            }

            return mailSent;
        }

        private void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
                mailSent = false;
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
                mailSent = false;
            }
            else
            {
                Console.WriteLine("Message sent.");
                mailSent = false;
            }

            mailSent = true;
        }

        #endregion

        /// <summary> 
        /// 添加邮件附件 
        /// </summary> 
        /// <param name="path">附件绝对路径</param> 
        private void AddAttachment(string path)
        {
            mMailAttachments.Add(path);
        }

        /// <summary>
        /// 根据key,从缓存中获取邮件配置内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetSMTPMailConfigValue(string key)
        {
            return AppSettingManager.GetSetting("Common", key);
        }
    }
}
