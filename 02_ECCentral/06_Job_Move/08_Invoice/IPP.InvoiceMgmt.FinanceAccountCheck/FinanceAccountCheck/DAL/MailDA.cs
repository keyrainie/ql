using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IPPOversea.Invoicemgmt.DAL;


namespace IPPOversea.Invoicemgmt.ZFBAccountCheck.DAL
{
    public class MailDA
    {

        public static bool SendEmail(string MailAddress,string MailSubject,string MailBody)
        {          
            SqlCommand cmd = CommandManager.GetCommand("InsertToSendEmail");
            cmd.Parameters.AddWithValue("@MailAddress", MailAddress);
            cmd.Parameters.AddWithValue("@MailSubject", MailSubject);
            cmd.Parameters.AddWithValue("@MailBody", MailBody);
            cmd.Parameters.AddWithValue("@CompanyCode", Settings.CompanyCode);
            return DBHelper.ExecuteSql(cmd);
        }      
      
    }
}
