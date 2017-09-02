using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;

namespace AutoSendMail.DAL
{
    internal static class AutoSendMailDA
    {
        public static int SendEmail(
            string mailAddress,
            string mailSubject,
            string mailBody,
            string companyCode,
            string storeCompanyCode,
            string languageCode
            )
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertToSendEmail");

            cmd.SetParameterValue("@MailAddress", mailAddress);
            cmd.SetParameterValue("@MailSubject", mailSubject);
            cmd.SetParameterValue("@MailBody", mailBody);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@StoreCompanyCode", storeCompanyCode);

            return cmd.ExecuteNonQuery();
        }

        public static QueryResult ExecuteResult(QueryCommand queryCmd)
        {
            var result = new QueryResult()
            {
                OutputParams = new Dictionary<string, object>()
            };
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig(queryCmd.CommandName);

            if (queryCmd.OutputParameters != null)
            {
                foreach (var param in queryCmd.OutputParameters)
                {
                    cmd.AddOutParameter(param.Name, (DbType)Enum.Parse(typeof(DbType), param.DbType), Convert.ToInt32(param.Size));
                }
            }

            var ds = cmd.ExecuteDataSet();

            if (queryCmd.OutputParameters != null)
            {
                foreach (var param in queryCmd.OutputParameters)
                {
                    var value = cmd.GetParameterValue(param.Name);

                    result.OutputParams.Add(param.Name.TrimStart('@'), value);
                }
            }

            result.ResultTable = cmd.ExecuteDataSet().Tables[0];

            return result;
        }

        public static int ExecuteNonQuery(string commandName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand(commandName);

            return cmd.ExecuteNonQuery();
        }
    }
}
