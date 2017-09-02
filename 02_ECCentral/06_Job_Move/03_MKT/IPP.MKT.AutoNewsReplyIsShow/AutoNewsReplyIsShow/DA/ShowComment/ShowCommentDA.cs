using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Entity;
using IPP.CN.ECommerceMgmt.AutoCommentShow.BusinessEntities;
using System.Reflection;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.DA
{
    public class ShowCommentDA
    {
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static List<RemarkMode> GetRemarkModeList()
        {
            List<RemarkMode> entitylist = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetRemarkMode");
            entitylist = command.ExecuteEntityList<RemarkMode>();
            return entitylist;
        }

        public static List<NewsAndReply> GetNewsAndReply()
        {
            List<NewsAndReply> entitylist = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetNewsAndReply");
            entitylist = command.ExecuteEntityList<NewsAndReply>();
            return entitylist;
        }

        public static List<BBSTopicMaster> GetBBSMasterList(int C3Sysno)
        {
            List<BBSTopicMaster> entitylist = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetComment");
            command.SetParameterValue("@C3Sysno", C3Sysno);
            entitylist = command.ExecuteEntityList<BBSTopicMaster>();
            return entitylist;
        }

        public static List<BBSTopicReply> GetBBSTopicReply()
        {
            List<BBSTopicReply> entitylist = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetBBSTopicReply");        
            entitylist = command.ExecuteEntityList<BBSTopicReply>();
            return entitylist;
        }


        public static List<BBSTopicReply> GetAllShowBBSTopicReplyByTopicSysNo(int TopicSysNo)
        {
            List<BBSTopicReply> entitylist = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllShowBBSTopicReplyByTopicSysNo");
            command.SetParameterValue("@TopicSysNo", TopicSysNo);
            entitylist = command.ExecuteEntityList<BBSTopicReply>();
            return entitylist;
        }

        public static List<NewsAndReply> GetHoliday(DateTime time)
        {
            List<NewsAndReply> entitylist = null;

            DataCommand command = DataCommandManager.GetDataCommand("GetHoliday");
            command.SetParameterValue("@StartTime", time);
            command.SetParameterValue("@EndTime", time.AddHours(24));
            command.SetParameterValue("@CompanyCode", CompanyCode);
            try
            {
                entitylist = command.ExecuteEntityList<NewsAndReply>();
            }
            catch
            {
                return null;
            }
            return entitylist;
        }

        public static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + " IPP-ECommerceMgmt-AutoSetNewsReplyIsShowOnHoliday 运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);

            command.ExecuteNonQuery();
        }

        public static int CheckPointLogByCommentSysNo(BBSTopicMaster entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckPointLogByCommentSysNo");
            command.SetParameterValue("@CustomerSysNo", entity.CreateCustomerSysNo);
            command.SetParameterValue("@CommentSysNo", entity.CommentSysNo.ToString());
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteScalar<int>();
        }
        public static string GetCannotOnlineWords()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCannotOnlineWords");
            string dirtyWords = command.ExecuteScalar<string>();
            return dirtyWords;
        }

        public static string GetQuestionWords()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetQuestionWords");
            string result = dc.ExecuteScalar<string>();
            return result;
        }

        public static int GetCustomerRank(int CreateCustomerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerRank");
            command.SetParameterValue("@CreateCustomerSysNo", CreateCustomerSysNo);
            try
            {
                return command.ExecuteScalar<int>();
            }
            catch
            {
                return -999;
            }
        }

        public static void UpdateBBSTopicMaster(BBSTopicMaster entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBBSTopicMaster");

            command.SetParameterValue("@SysNo", entity.CommentSysNo);       
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@IsAddPoint", entity.IsAddPoint);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }

        public static void UpdateNewsAndReply(NewsAndReply entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNewsAndReply");
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }

        public static void UpdateBBSTopicReplyStatus(BBSTopicReply entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBBSTopicReply");
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.TopicReplySysNo);
            command.SetParameterValue("@IsFirstShow", entity.IsFirstShow);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }

        public static bool InsertReplyMail(string MailAddress, string CCMailAddress, string MailSubject, string MailBody)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertReplyMail");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", MailBody);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SetProductRemarkCountRemarkScore(BBSTopicMaster item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("SetProductRemarkCountRemarkScore");
            dc.SetParameterValue("@ReferenceSysNo", item.ReferenceSysNo);
            dc.SetParameterValue("@CompanyCode", CompanyCode);
            dc.ExecuteNonQuery();
        }

        public static void SendEmail2MailDbWhenCommentShow(string mailAddress, string mailSubject, string mailBody, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", mailAddress);
            command.SetParameterValue("@CCMailAddress", string.Empty);
            command.SetParameterValue("@MailSubject", mailSubject);
            command.SetParameterValue("@MailBody", mailBody);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);

            command.ExecuteNonQuery();
        }

        public static List<BBSTopicMaster> GetBBSMasterListBySysNo(BBSTopicMaster entity)
        {
            List<BBSTopicMaster> entitylist = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetBBSMasterListBySysNo");
            command.SetParameterValue("@CommentSysNo", entity.CommentSysNo);
            entitylist = command.ExecuteEntityList<BBSTopicMaster>();
            return entitylist;
        }
    }

    public static class ConvertHelper
    {
        public static string ToListString<T>(this List<T> list)
        {
            string result = string.Empty;
            foreach (var item in list)
            {
                if (item.GetType() == typeof(string))
                {
                    result += "'" + item + "',";
                }
                else
                {
                    result += item + ",";
                }
            }
            return result.TrimEnd(',');
        }

        public static string ToListString<T>(this List<T> list, string PropName)
        {
            string result = string.Empty;
            Type ty = typeof(T);
            PropertyInfo pty = ty.GetProperty(PropName);
            foreach (var item in list)
            {
                if (pty != null)
                {
                    object obj = pty.GetValue(item, null);
                    result += string.Format(obj.GetType() == typeof(Int32) ? "{0}," : "'{0}',", obj);
                }
            }
            return result.TrimEnd(',');
        }

    }
}
