using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.WebFramework.Mail
{
    public class MailHelper
    {
        public static string GetMailTemplateBody(string templateID, out string subject)
        {
            MailTemplate tem = MailConfig.GetMailTemplate(templateID);
            if (tem != null && !string.IsNullOrEmpty(tem.Body))
            {
                subject = tem.Subject;
                return tem.Body;
            }
            subject = string.Empty;
            return string.Empty;
        }
    }
}
