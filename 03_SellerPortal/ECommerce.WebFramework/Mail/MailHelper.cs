using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.WebFramework.Mail
{
    public class MailHelper
    {
        public static MailTemplate GetMailTemplateByID(string templateID)
        {
            MailTemplate tem = MailConfig.GetMailTemplate(templateID);
            if (tem == null)
            {
                throw new ApplicationException("目标模板[" + templateID + "]未找到!");
            }
            return tem;
        }

        public static MailTemplate BuildMailTemplate(string templateID, KeyValueVariables keyValues, KeyTableVariables keyTables)
        {
            MailTemplate tem = MailConfig.BuildMailTemplate(templateID, keyValues, keyTables);
            return tem;
        }

        public static MailTemplate BuildMailTemplate(MailTemplate template, KeyValueVariables keyValues, KeyTableVariables keyTables)
        {
            MailTemplate tem = MailConfig.BuildMailTemplate(template, keyValues, keyTables);
            return tem;
        }
    }
}
