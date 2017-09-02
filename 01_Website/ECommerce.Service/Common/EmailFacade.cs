using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.DataAccess.Common;
using ECommerce.Entity;

namespace ECommerce.Facade
{
    public class EmailFacade
    {
        public static bool SendEmail(string subject, string content, string to, string from)
        {
            AsyncEmail email = new AsyncEmail();
            email.MailAddress = to;
            email.MailFrom = from;
            email.Status = (int)EmailStatus.NotSend;
            email.MailBody = content;
            email.MailSubject = subject;
            return EmailDA.SendEmail(email);
        }
    }
}
