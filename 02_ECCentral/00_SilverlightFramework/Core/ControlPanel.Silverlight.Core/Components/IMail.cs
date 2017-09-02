using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface IMail : IComponent
    {
        /// <summary>
        /// 发送内部邮件（支持4位短名）
        /// </summary>
        /// <param name="message"></param>
        void SendInternalMail(InternalMailMessage message, Action<MailResult> callback);


        /// <summary>
        /// 发送对外的业务邮件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void SendBusinessMail(MailMessage message, Action<MailResult> callback);


        /// <summary>
        /// 发送基于模板的对外业务邮件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void SendBusinessMailByTemplate(MailTemplateMessage message, Action<MailResult> callback);


        /// <summary>
        /// 打开发送邮件页面
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        void OpenMailPage(MailMessage message, MailPageSetting setting);

        /// <summary>
        /// 打开发送邮件页面, 提供发送完成之后回调。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        /// <param name="callback"></param>
        void OpenMailPage(MailMessage message, MailPageSetting setting, Action<MailResult> callback);

        /// <summary>
        /// 打开发送多邮件的页面，提供发送完成之后回调
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="setting"></param>
        /// <param name="callback"></param>
        void OpenMultiMailPage(List<MailMessage> messages, MailPageSetting setting, Action<MailResult> callback);

        /// <summary>
        /// 打开发送邮件页面
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        void OpenMailPageByTemplate(MailTemplateMessage message, MailPageSetting setting);

        /// <summary>
        /// 打开发送邮件页面, 提供发送完成之后回调。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        /// <param name="callback"></param>
        void OpenMailPageByTemplate(MailTemplateMessage message, MailPageSetting setting, Action<MailResult> callback);
    }

    public class MailPageSetting
    {
        /// <summary>
        /// 是否允许用户编辑邮件内容（默认为True,优先级高于IsAllowChangeMailFrom，IsAllowChangeMailBody，IsAllowChangeMailTo，IsAllowChangeMailSubject)
        /// </summary>
        public bool IsAllowEdit { get; set; }

        /// <summary>
        /// 是否允许发送：TRUE=允许点击Send按钮发送；FALSE=不允许，隐藏Send按钮(默认为True)
        /// </summary>
        public bool IsAllowSend { get; set; }

        /// <summary>
        /// 是否允许用户在页面上修改Mail Subject字段(默认为True)
        /// </summary>
        public bool IsAllowChangeMailSubject { get; set; }

        /// <summary>
        /// 是否允许用户在页面上修改Mail Body字段(默认为True)
        /// </summary>
        public bool IsAllowChangeMailBody { get; set; }

        /// <summary>
        /// 是否允许用户在页面上修改Mail From字段(默认为True)
        /// </summary>
        public bool IsAllowChangeMailFrom { get; set; }

        /// <summary>
        /// 是否允许用户在页面上修改Mail To字段(默认为True)
        /// </summary>
        public bool IsAllowChangeMailTo { get; set; }

        /// <summary>
        /// 是否支持CC(默认为False)
        /// </summary>
        public bool IsAllowCC { get; set; }

        /// <summary>
        /// 是否支持BCC(默认为False)
        /// </summary>
        public bool IsAllowBCC { get; set; }



        /// <summary>
        /// 是否允许用户在页面上上传附件(默认为False)
        /// </summary>
        public bool IsAllowAttachment { get; set; }


        public MailPageSetting()
        {
            this.IsAllowEdit = true;
            this.IsAllowSend = true;
            this.IsAllowChangeMailFrom = true;
            this.IsAllowChangeMailTo = true;
            this.IsAllowChangeMailSubject = true;
            this.IsAllowChangeMailBody = true;

            this.IsAllowCC = false;
            this.IsAllowBCC = false;
            this.IsAllowAttachment = false;
        }
    }

    public class MailTemplateMessage
    {
        public string SystemID { get; set; }

        public string CountryCode { get; set; }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }

        public string DomainName { get; set; }

        public string TemplateID { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string ReplyName { get; set; }

        public List<MailTemplateVariable> MailTemplateVariables { get; set; }

        public List<BusinessNumber> BusinessNumberList { get; set; }
    }

    public class MailMessage
    {
        public string ID { get; set; }

        public string SystemID { get; set; }

        public string CountryCode { get; set; }

        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }

        public string DomainName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string CC{ get; set; }

        public string BCC { get; set; }

        public string ReplyName { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public MailPriority Priority { get; set; }

        public MailBodyType BodyType { get; set; }

        public List<BusinessNumber> BusinessNumberList { get; set; }

        public List<MailAttachment> Attachments { get; set; }

        public MailMessage()
        {
            this.Priority = MailPriority.Normal;
        }
    }

    public class MailAttachment
    {
        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

        public MediaType MediaType { get; set; }
    }

    public class BusinessNumber
    {
        public NumberType NumberType { get; set; }

        public string NumberValue { get; set; }
    }

    public class MailTemplateVariable
    { 
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public enum NumberType
    {
        SONumber,
        CustomerNumber,
        InvoiceNumber,
        RMANumber,
        ItemNumber,
        PONumber,
        VendorNumber,
        TrackingNumber,
        ClaimNumber
    }

    public enum MailPriority
    {
        Normal,
        Low,
        High
    }

    public enum MailBodyType
    {
        Html,
        Text
    }

    public enum MediaType
    {
        GIF,
        JPEG,
        TIFF,
        PDF,
        RTF,
        SOAP,
        ZIP,
        Other
    }

    public class InternalMailMessage
    {
        public string From { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public MailBodyType BodyType { get; set; }

        public MailPriority Priority { get; set; }

        public List<MailAttachment> Attachments { get; set; }
    }

    public class MailResult
    {
        public string ID { get; set; }

        public string Error { get; set; }

        public bool IsSuccess { get; set; }
    }
}
