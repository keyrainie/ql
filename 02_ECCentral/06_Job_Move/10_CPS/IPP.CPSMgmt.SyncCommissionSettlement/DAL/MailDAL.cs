using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Newegg.Oversea.Framework.DataAccess;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.DAL
{
    public class MailDAL
    {
        /// <summary>
        /// 邮件 数据访问
        /// </summary>
        /// <param name="MailAddress">邮件地址</param>
        /// <param name="MailSubject">邮件标题</param>
        /// <param name="MailBody">邮件内容</param>
        public static void SendEmail(string MailAddress,string MailSubject,string MailBody)
        {          
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", MailBody);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            command.ExecuteNonQuery();
        }      
      
    }
}
