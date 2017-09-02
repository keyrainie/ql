using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Newegg.Oversea.Framework.DataAccess;


namespace CustomReport.DAL
{
    public class MailDA
    {

        public static void SendEmail(string MailAddress, string MailSubject, string MailBody,int status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", MailBody);
            command.SetParameterValue("@Staues", status);
            command.ExecuteNonQuery();
        }      
      
    }
}
