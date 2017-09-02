using System;
using System.Transactions;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using System.Collections.Generic;
using Newegg.Oversea.Framework.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess
{
    public static class MailDA
    {
        public static string LogMailMessage(MailMessage message)
        {
            var dataCommand = DataCommandManager.GetDataCommand("CreateMailMessage");

            dataCommand.SetParameterValue("@ID", message.ID);
            dataCommand.SetParameterValue("@DomainName", message.DomainName);
            dataCommand.SetParameterValue("@SystemID", message.SystemID);
            dataCommand.SetParameterValue("@TemplateID", message.TemplateID);
            dataCommand.SetParameterValue("@IsSentByTemplate", ConvertToString(message.IsSentByTemplate));
            dataCommand.SetParameterValue("@FromName", message.From);
            dataCommand.SetParameterValue("@ToName", message.To);
            dataCommand.SetParameterValue("@CCName", message.CC);
            dataCommand.SetParameterValue("@BCCName", message.BCC);
            dataCommand.SetParameterValue("@ReplyName", message.ReplyName);
            dataCommand.SetParameterValue("@IsHtmlBody", message.BodyType == MailBodyType.Html ? "Y" : "N");
            dataCommand.SetParameterValue("@Priority", (Int32)message.Priority);
            dataCommand.SetParameterValue("@Subject", message.Subject);
            dataCommand.SetParameterValue("@MailBody", message.Body);
            dataCommand.SetParameterValue("@CountryCode", message.CountryCode);
            dataCommand.SetParameterValue("@CompanyCode", message.CompanyCode);
            dataCommand.SetParameterValue("@LanguageCode", message.LanguageCode);

            return dataCommand.ExecuteScalar().ToString();
        }

        public static void UpdateMailMessage(MailMessage message)
        {
            var dataCommand = DataCommandManager.GetDataCommand("UpdateMailMessage");

            dataCommand.SetParameterValue("@MessageID", message.MessageID);
            dataCommand.SetParameterValue("@FromName", message.From);
            dataCommand.SetParameterValue("@ToName", message.To);
            dataCommand.SetParameterValue("@CCName", message.CC);
            dataCommand.SetParameterValue("@BCCName", message.BCC);
            dataCommand.SetParameterValue("@Subject", message.Subject);
            dataCommand.SetParameterValue("@MailBody", message.Body);
            dataCommand.SetParameterValue("@IsSent", ConvertToString(message.IsSent));
            dataCommand.SetParameterValue("@IsSentByTemplate", ConvertToString(message.IsSentByTemplate));

            dataCommand.ExecuteNonQuery();
        }

        public static void LogBusinessNumber(string messageID, List<BusinessNumber> numbers)
        {
            var dataCommand = DataCommandManager.GetDataCommand("CreateBusinessNumber");

            foreach (var number in numbers)
            {
                dataCommand.SetParameterValue("@MessageID", messageID);
                dataCommand.SetParameterValue("@NumberType", (Int32)number.NumberType);
                dataCommand.SetParameterValue("@NumberValue", number.NumberValue);

                dataCommand.ExecuteNonQuery();
            }
        }

        public static void LogTemplateVariable(string messageID, List<MailTemplateVariable> variables)
        {
            var xml = SerializationUtility.XmlSerialize(variables);

            var dataCommand = DataCommandManager.GetDataCommand("CreateVariables");
            dataCommand.SetParameterValue("@MessageID", messageID);
            dataCommand.SetParameterValue("@Variables", xml);

            dataCommand.ExecuteNonQuery();
        }

        public static void LogPageSetting(string messageID, MailPageSetting setting)
        {
            var dataCommand = DataCommandManager.GetDataCommand("CreatePageSetting");

            dataCommand.SetParameterValue("@MessageID", messageID);
            dataCommand.SetParameterValue("@IsAllowEdit", ConvertToString(setting.IsAllowEdit));
            dataCommand.SetParameterValue("@IsAllowSend", ConvertToString(setting.IsAllowSend));
            dataCommand.SetParameterValue("@IsAllowChangeMailFrom", ConvertToString(setting.IsAllowChangeMailFrom));
            dataCommand.SetParameterValue("@IsAllowChangeMailTo", ConvertToString(setting.IsAllowChangeMailTo));
            dataCommand.SetParameterValue("@IsAllowChangeMailSubject", ConvertToString(setting.IsAllowChangeMailSubject));
            dataCommand.SetParameterValue("@IsAllowChangeMailBody", ConvertToString(setting.IsAllowChangeMailBody));
            dataCommand.SetParameterValue("@IsAllowCC", ConvertToString(setting.IsAllowCC));
            dataCommand.SetParameterValue("@IsAllowBCC", ConvertToString(setting.IsAllowBCC));
            dataCommand.SetParameterValue("@IsAllowAttachment", ConvertToString(setting.IsAllowAttachment));

            dataCommand.ExecuteNonQuery();
        }


        public static MailMessage GetMailMessage(string messageID)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetMailMessage");

            dataCommand.SetParameterValue("@MessageID", messageID);

            var result = dataCommand.ExecuteEntity<MailMessage>();

            return result;
        }

        public static bool GetMailStatus(string messageID)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetMailStatus");

            dataCommand.SetParameterValue("@MessageID", messageID);

            return (bool)dataCommand.ExecuteScalar();
        }

        public static MailPageSetting GetPageSetting(string messageID)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetMailPageSetting");

            dataCommand.SetParameterValue("@MessageID", messageID);

            var result = dataCommand.ExecuteEntity<MailPageSetting>();

            return result;
        }

        public static List<BusinessNumber> GetBusinessNumber(string messageID)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetBusinessNumber");

            dataCommand.SetParameterValue("@MessageID", messageID);

            var result = dataCommand.ExecuteEntityList<BusinessNumber>();

            return result;
        }

        public static List<MailTemplateVariable> GetTemplateVariable(string messageID)
        {
            var dataCommand = DataCommandManager.GetDataCommand("GetTemplateVariable");

            dataCommand.SetParameterValue("@MessageID", messageID);

            var result = dataCommand.ExecuteScalar().ToString();

            return SerializationUtility.XmlDeserialize<List<MailTemplateVariable>>(result);
        }


        private static string ConvertToString(bool b)
        {
            return b ? "Y" : "N";
        }
    }
}
