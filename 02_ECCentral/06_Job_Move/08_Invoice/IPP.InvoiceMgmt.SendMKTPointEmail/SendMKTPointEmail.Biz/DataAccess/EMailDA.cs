using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Newegg.Oversea.Framework.DataAccess;
using SendMKTPointEmail.Biz.Entities;
using SendMKTPointEmail.Biz.Common;

namespace SendMKTPointEmail.Biz.DataAccess
{
    public class EMailDA
    {
        /// <summary>
        /// 直接往邮件DB中插记录来发邮件
        /// </summary>
        /// <param name="MailAddress"></param>
        /// <param name="MailSubject"></param>
        /// <param name="MailBody"></param>
        public static void SendEmail(string MailAddress, string MailSubject, string MailBody)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");

            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", MailBody);
            command.SetParameterValue("@CompanyCode", JobConfig.CompanyCode);

            command.ExecuteNonQuery();
        }


    }
}
