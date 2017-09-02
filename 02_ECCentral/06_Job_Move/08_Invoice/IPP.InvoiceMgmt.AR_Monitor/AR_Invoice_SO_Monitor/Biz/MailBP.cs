using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.DAL;

namespace IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.Biz
{
    class MailBP
    {
        public static bool SendMail(string payTypeDesc,int SOSysNo,decimal so,decimal ar,decimal invoice)
        {
            string subject = Settings.MailSubject + "-" + SOSysNo.ToString();
            string mailBody = GetMailBody(payTypeDesc,SOSysNo, so, ar, invoice);
            string address = Settings.EmailAddress;
            return MailDA.SendEmail(SOSysNo, address, subject, mailBody);
        }
        private static string GetMailBody(string payTypeDesc,int SOSysNo, decimal so, decimal ar, decimal invoice)
        {
            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mailTemplate.txt"));
            string content = sr.ReadToEnd();
            sr.Close();
            content = content.Replace("@PayTypeDesc", payTypeDesc);
            content = content.Replace("@SysNo", SOSysNo.ToString());
            content = content.Replace("@SO", so.ToString());
            content = content.Replace("@AR", ar.ToString());
            content = content.Replace("@Invoice", invoice.ToString());
            return content;
        }
    }
}
