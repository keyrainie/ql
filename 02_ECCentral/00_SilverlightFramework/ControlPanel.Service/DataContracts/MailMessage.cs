using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    public class MailPageMessage
    {
        public MailPageSetting MailPageSetting { get; set; }

        public List<MailMessage> MailCollection { get; set; }

        public MailMessage MailMessage { get; set; }
    }


    public class SendResult
    {
        public bool IsSuccess { get; set; }

        public string Description { get; set; }
    }

    public class MailMessage
    {
        public string ID { get; set; }

        [DataMapping("MessageID", System.Data.DbType.String)]
        public string MessageID { get; set; }

        [DataMapping("SystemID", System.Data.DbType.AnsiStringFixedLength)]
        public string SystemID { get; set; }

        [DataMapping("DomainName", System.Data.DbType.AnsiString)]
        public string DomainName { get; set; }

        [DataMapping("CountryCode", System.Data.DbType.AnsiStringFixedLength)]
        public string CountryCode { get; set; }

        [DataMapping("CompanyCode", System.Data.DbType.AnsiStringFixedLength)]
        public string CompanyCode { get; set; }

        [DataMapping("LanguageCode", System.Data.DbType.AnsiStringFixedLength)]
        public string LanguageCode { get; set; }

        [DataMapping("TemplateID", System.Data.DbType.AnsiString)]
        public string TemplateID { get; set; }

        [DataMapping("FromName", System.Data.DbType.AnsiString)]
        public string From { get; set; }

        [DataMapping("ToName", System.Data.DbType.AnsiString)]
        public string To { get; set; }

        [DataMapping("CCName", System.Data.DbType.AnsiString)]
        public string CC { get; set; }

        [DataMapping("BCCName", System.Data.DbType.AnsiString)]
        public string BCC { get; set; }

        [DataMapping("ReplyName", System.Data.DbType.AnsiString)]
        public string ReplyName { get; set; }

        [DataMapping("Subject", System.Data.DbType.String)]
        public string Subject { get; set; }

        [DataMapping("MailBody", System.Data.DbType.String)]
        public string Body { get; set; }

        [DataMapping("Priority", System.Data.DbType.Int32)]
        public MailPriority Priority { get; set; }

        [DataMapping("BodyType", System.Data.DbType.Int32)]
        public MailBodyType BodyType { get; set; }

        [DataMapping("IsSent", System.Data.DbType.Boolean)]
        public bool IsSent { get; set; }

        [DataMapping("SendDate", System.Data.DbType.DateTime)]
        public bool SendDate { get; set; }

        [DataMapping("InDate", System.Data.DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("IsSentByTemplate", System.Data.DbType.Boolean)]
        public bool IsSentByTemplate { get; set; }

        public string TemplateText
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(this.Subject))
                {
                    return this.TemplateID;
                }
                else
                {
                    return string.Format("[{0}]{1}", this.TemplateID, this.Subject);
                }
            }
        }

        public List<MailAttachment> Attachments { get; set; }

        public List<MailTemplateVariable> MailTemplateVariables { get; set; }

        public List<BusinessNumber> BusinessNumberList { get; set; }
    }

    #region Enums

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

    #endregion

    #region Related Class

    public class MailTemplateVariable
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class BusinessNumber
    {
        [DataMapping("NumberType", System.Data.DbType.Int32)]
        public NumberType NumberType { get; set; }

        [DataMapping("NumberValue", System.Data.DbType.AnsiString)]
        public string NumberValue { get; set; }
    }

    public class MailAttachment
    {
        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

        public MediaType MediaType { get; set; }
    }

    public class MailPageSetting
    {
        [DataMapping("IsAllowEdit", System.Data.DbType.Boolean)]
        public bool IsAllowEdit { get; set; }

        [DataMapping("IsAllowSend", System.Data.DbType.Boolean)]
        public bool IsAllowSend { get; set; }

        [DataMapping("IsAllowChangeMailSubject", System.Data.DbType.Boolean)]
        public bool IsAllowChangeMailSubject { get; set; }

        [DataMapping("IsAllowChangeMailBody", System.Data.DbType.Boolean)]
        public bool IsAllowChangeMailBody { get; set; }

        [DataMapping("IsAllowChangeMailFrom", System.Data.DbType.Boolean)]
        public bool IsAllowChangeMailFrom { get; set; }

        [DataMapping("IsAllowChangeMailTo", System.Data.DbType.Boolean)]
        public bool IsAllowChangeMailTo { get; set; }

        [DataMapping("IsAllowCC", System.Data.DbType.Boolean)]
        public bool IsAllowCC { get; set; }

        [DataMapping("IsAllowBCC", System.Data.DbType.Boolean)]
        public bool IsAllowBCC { get; set; }

        [DataMapping("IsAllowAttachment", System.Data.DbType.Boolean)]
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

    #endregion
}
