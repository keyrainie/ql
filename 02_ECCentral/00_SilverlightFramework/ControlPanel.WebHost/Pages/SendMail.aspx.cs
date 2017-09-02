using System;
using System.Threading;

using Newegg.Oversea.Silverlight.ControlPanel.Service.BizProcess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.WebHost.Utilities;
using System.Text;
using Newegg.Oversea.Framework.EmailService.Configuration;
using Newegg.Oversea.Silverlight.ControlPanel.Service;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Newegg.Oversea.Silverlight.ControlPanel.WebHost
{
    public partial class SendMail : System.Web.UI.Page
    {
        private readonly string m_SessionKey_MailMessage = "SessionKey_MailMessage_{0}";

        #region Properties

        protected string LanguageCode
        {
            get;
            set;
        }

        protected string MessageID
        {
            get
            {
                var id = Request.QueryString["MessageID"];
                if (id != null)
                    return id;
                return string.Empty;
            }
        }

        protected MailConfiguration MailConfiguration
        {
            get { return MailConfiguration.Default; }
        }

        protected List<MailMessage> MailCollection { get; private set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Initialize();
            }
            else
            {
                var key = string.Format(m_SessionKey_MailMessage, MessageID);
                var message = Session[key] as MailPageMessage;

                if (message == null)
                {
                    message = MailBiz.GetMailMessage(MessageID);
                    Session[key] = message;
                }

                HandlePermissions(message);
            }
        }

        protected override void InitializeCulture()
        {
            var languageCode = Request.QueryString["LanguageCode"];

            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = "en-US";
            }

            this.LanguageCode = languageCode;

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(languageCode);

            base.InitializeCulture();
        }

        protected void BtnSend_Click(object sender, EventArgs e)
        {
            var key = string.Format(m_SessionKey_MailMessage, MessageID);
            var message = Session[key] as MailPageMessage;
            if (message != null)
            {
                //如果是基于模板的方式的邮件，根据原始的MailMessage和现在UI上的输入进行比较，
                //如果有改变，则不再使用模板发送, 反之则使用模板方式发送；
                if (!string.IsNullOrWhiteSpace(message.MailMessage.TemplateID))
                {
                    var b = AreSendByTemplate(message.MailPageSetting);
                    if (!b)
                    {
                        message.MailMessage.TemplateID = null;
                    }
                    else
                    {
                        message.MailMessage.TemplateID = this.ddlTemplates.SelectedValue;
                    }
                }

                message.MailMessage.From = txtFrom.Text.Trim().TrimEnd(';');
                message.MailMessage.To = txtTo.Text.Trim().TrimEnd(';');
                message.MailMessage.Subject = txtSubject.Text.Trim();

                if (message.MailPageSetting.IsAllowChangeMailBody)
                {
                    message.MailMessage.Body = txtBody.Text.Trim();
                }

                if (message.MailPageSetting.IsAllowCC)
                {
                    message.MailMessage.CC = txtCC.Text.Trim().TrimEnd(';');
                }
                else
                {
                    message.MailMessage.CC = null;
                }
                if (message.MailPageSetting.IsAllowBCC)
                {
                    message.MailMessage.BCC = txtBCC.Text.Trim().TrimEnd(';');
                }
                else
                {
                    message.MailMessage.BCC = null;
                }

                if (message.MailPageSetting.IsAllowAttachment)
                {
                    message.MailMessage.Attachments = AttachmentHelper.GetFileAttachment(Request.Files);
                }

                try
                {
                    MailBiz.SendBusinessMail(message.MailMessage);
                    ScriptExecute("jAlert('" + this.GetLocalResourceObject("Info_SendSuccessfully") + "', '" + this.GetLocalResourceObject("Info_Alert_Title") + "', function() { closeWindow(); });");

                    this.BtnSend.Enabled = false;
                }
                catch (Exception ex)
                {
                    ScriptExecute("jAlert('" + ex.Message + "','" + this.GetLocalResourceObject("Info_Alert_Title") + "');");
                }
            }

        }

        protected void ddlTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = string.Format(m_SessionKey_MailMessage, MessageID);
            var message = (Session[key] as MailPageMessage).MailCollection[this.ddlTemplates.SelectedIndex];

            this.txtFrom.Text = message.From;
            this.txtSubject.Text = message.Subject;
            this.txtBody.Text = message.Body;
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            var messageID = MessageID;

            if (!string.IsNullOrWhiteSpace(messageID))
            {
                MailPageMessage message = null;

                message = MailBiz.GetMailMessage(messageID);


                if (message.MailPageSetting == null)
                {
                    message.MailPageSetting = new MailPageSetting();
                }
                var key = string.Format(m_SessionKey_MailMessage, messageID);

                //保存原始的MailMessage到Session中。
                Session[key] = message;

                if (message != null)
                {
                    HandlePermissions(message);

                    if (message.MailMessage != null)
                    {
                        var mail = message.MailMessage;
                        txtFrom.Text = mail.From;
                        txtTo.Text = mail.To;
                        txtCC.Text = mail.CC;
                        txtBCC.Text = mail.BCC;
                        txtSubject.Text = mail.Subject;
                        txtBody.Text = mail.Body;

                        if (!string.IsNullOrWhiteSpace(mail.TemplateID))
                        {
                            this.MailCollection = message.MailCollection;

                            ddlTemplates.DataSource = message.MailCollection;
                            ddlTemplates.SelectedValue = mail.TemplateID;
                            ddlTemplates.DataBind();
                        }

                        var title = string.Format(this.GetLocalResourceObject("LbTitle").ToString(), mail.Subject);

                        this.Title = title;
                    }
                }
            }
            else
            {
                ScriptExecute("jAlert('" + this.GetGlobalResourceObject("InfoMessage", "Info_InvalidParameter") + "','" + this.GetLocalResourceObject("Info_Alert_Title") + "', function() { closeWindow(); });");
            }
        }

        private bool AreSendByTemplate(MailPageSetting setting)
        {
            if (!setting.IsAllowEdit)
            {
                return true;
            }

            if (setting.IsAllowChangeMailFrom)
            {
                return false;
            }
            if (setting.IsAllowChangeMailSubject)
            {
                return false;
            }
            if (setting.IsAllowChangeMailBody)
            {
                return false;
            }

            return true;
        }

        private void HandlePermissions(MailPageMessage message)
        {
            var setting = message.MailPageSetting;
            var mail = message.MailMessage;
            var readOnly = true;
            var script = new StringBuilder();
            var height = 370;

            if (!string.IsNullOrWhiteSpace(message.MailMessage.TemplateID))
            {
                this.TemplateSelector(true);
                height -= 26;
            }
            else
            {
                this.TemplateSelector(false);
            }

            if (message.MailCollection != null && message.MailCollection.Count == 1)
            {
                this.TemplateSelector(false);
            }
           

            if (!setting.IsAllowSend)
            {
                BtnSend.Enabled = false;
            }

            if (!setting.IsAllowEdit)
            {
                txtFrom.Enabled = false;
                txtTo.Enabled = false;
                txtCC.Enabled = false;
                txtBCC.Enabled = false;
                txtSubject.Enabled = false;
                txtBody.ReadOnly = true;
                readOnly = true;
            }
            else
            {
                if (!setting.IsAllowChangeMailFrom)
                {
                    txtFrom.Enabled = false;
                }
                if (!setting.IsAllowChangeMailTo)
                {
                    txtTo.Enabled = false;
                }
                if (!setting.IsAllowChangeMailSubject)
                {
                    txtSubject.Enabled = false;
                }
                if (!setting.IsAllowChangeMailBody)
                {
                    txtBody.ReadOnly = true;
                    readOnly = true;
                }
                else
                {
                    readOnly = false;
                }
            }

            if (!setting.IsAllowAttachment || !setting.IsAllowEdit)
            {
                height += 45;
                script.Append("$('#attachmentContainer').remove();");
            }
            if (!setting.IsAllowCC)
            {
                txtCC.Text = string.Empty;
                height += 26;
                script.Append("$('#row_CC').remove();");
            }
            if (!setting.IsAllowBCC)
            {
                txtBCC.Text = string.Empty;
                height += 26;
                script.Append("$('#row_BCC').remove();");
            }

            if (mail.BodyType == MailBodyType.Html)
            {
                if (readOnly)
                {
                    script.Append("init_editor(true, true, " + height + ");");
                }
                else
                {
                    script.Append("init_editor(true, false, " + height + ");");
                }
            }
            else
            {
                script.Append("init_editor(false, false, " + height + ");");
            }

            if (script.Length > 0)
            {
                ScriptExecute(script.ToString());
            }

        }

        private void ScriptExecute(string script)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "script_" + new Random().Next(), "<script>" + script + "</script>");
        }

        private void TemplateSelector(bool visibility)
        {
            this.ddlTemplates.Visible = visibility;
            this.LbTemplate.Visible = visibility;
        }

        #endregion

        
    }
}