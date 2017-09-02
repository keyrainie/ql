using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IPPOversea.Invoicemgmt.DAL;
using Newegg.Oversea.Framework.DataAccess;

namespace IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.DAL
{
    public class MailDA
    {

        public static bool SendEmail(string mailAddress, string mailSubject, string mailBody)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertToSendEmail");

            cmd.SetParameterValue("@MailAddress", mailAddress);
            cmd.SetParameterValue("@MailSubject", mailSubject);
            cmd.SetParameterValue("@MailBody", mailBody);
            cmd.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return cmd.ExecuteNonQuery() > 0;
        }

    }
}
