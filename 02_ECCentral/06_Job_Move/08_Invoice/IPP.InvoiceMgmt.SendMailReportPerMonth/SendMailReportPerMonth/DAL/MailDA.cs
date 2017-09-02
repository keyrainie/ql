using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPPOversea.InvoiceMgmt.PerMonthReport.Compoents;

namespace IPPOversea.InvoiceMgmt.PerMonthReport.DAL
{
    public static class MailDA
    {
        public static void SendEmail(string MailAddress, string MailSubject, string MailBody, int status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", MailBody);
            command.SetParameterValue("@Staues", status);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            command.ExecuteNonQuery();
        }
    }
}
