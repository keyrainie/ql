using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using IPP.ContentMgmt.SendQuestionList.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;

namespace IPP.ContentMgmt.SendQuestionList.DA
{
    public class QuestionListDA
    {
        #region definition
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static string LanguageCode = ConfigurationManager.AppSettings["LanguageCode"];
        public static string GiftC3SysNo = ConfigurationManager.AppSettings["GiftC3SysNo"];
        #endregion

        public static List<QuestionList> GetQuestionList()
        {
            List<QuestionList> QuestionList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetQuestionList");
            command.SetParameterValue("@ReplyTimeFrom",Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00")));
            command.SetParameterValue("@ReplyTimeTo", Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59")));
            command.SetParameterValue("@Type", Convert.ToChar("N"));
            command.SetParameterValue("@CompanyCode", CompanyCode);
            QuestionList = command.ExecuteEntityList<QuestionList>();
            return QuestionList;
        }

        public static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + " IPP-ContentMgmt-SendQuestionListJOB 运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);

            command.ExecuteNonQuery();
        }

        public static void SendMailQuestionList(MailBodyV31 mailBody)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", mailBody.Body.MailTo);
            command.SetParameterValue("@CCMailAddress", mailBody.Body.CCMailAddress);
            command.SetParameterValue("@MailSubject", mailBody.Body.Subjuect);
            command.SetParameterValue("@MailBody", mailBody.Body.MailBody);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);

            command.ExecuteNonQuery();
        }
    }
}
