using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IPPOversea.Invoicemgmt.DAL;


namespace IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.DAL
{
    public class MailDA
    {

        public static bool SendEmail(int SOSysNo,string MailAddress,string MailSubject,string MailBody)
        {          
            SqlCommand cmd = CommandManager.GetCommand("InsertToSendEmail");
            cmd.Parameters.Add("@MailAddress", MailAddress);
            cmd.Parameters.Add("@MailSubject", MailSubject);
            cmd.Parameters.Add("@MailBody", MailBody);
            return DBHelper.ExecuteSql(cmd); ;
        }      
      
    }
}
