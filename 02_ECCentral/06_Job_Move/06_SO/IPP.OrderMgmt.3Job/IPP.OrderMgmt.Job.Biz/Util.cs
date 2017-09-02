using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.Common;
using ECCentral.Job.Utility;
using IPP.OrderMgmt.Job.Biz.DataContract;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ServiceConsole.Client;

namespace IPP.OrderMgmt.Job.Biz
{
    /// <summary>
    /// In this class,include the common data process functions, database query functions,
    /// service call functions. There are all static functions.
    /// </summary>
    public class Util
    {
        public static string TrimNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToString().Trim();
            }
        }

        /// <summary>
        /// check the dataset first table whether has more row
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static bool HasMoreRow(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string ToSqlString(string paramStr)
        {
            return "'" + SafeFormat(paramStr) + "'";
        }

        private static string SafeFormat(string strInput)
        {
            if (string.IsNullOrEmpty(strInput) == true)
                return string.Empty;
            return strInput.Trim().Replace("'", "''");
        }

        public static decimal TrimDecimalNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return AppConst.DecimalNull;
            }
            else
            {
                return decimal.Parse(obj.ToString());
            }
        }

        public static DateTime TrimDateNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return AppConst.DateTimeNull;
            }
            else
            {
                return DateTime.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// 判断是否手机号码
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsCellNumber(string cell)
        {
            if (string.IsNullOrEmpty(cell) == true)
                return false;

            try
            {
                // 验证为数字，防止全角字符
                Convert.ToInt64(cell);

                return Regex.IsMatch(cell, @"^1\d{10}$");
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyCode">companycode</param>
        /// <param name="isSys">true, system email; false</param>
        /// <param name="mailInfor"></param>
        public static void SendEmail(string companyCode,MailInfo mailInfor)
        {
            if (string.IsNullOrEmpty(mailInfor.ToName)) return;
            else
                mailInfor.ToName = Util.TrimNull(mailInfor.ToName);
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Message/SendMail", mailInfor, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }
        }

        /// <summary>
        /// get the order check shipping information
        /// </summary>
        /// <param name="soSysNo">order sysnumber</param>
        /// <returns>shipping information dataset</returns>
        public static DataSet GetSOCheckShipping(int soSysNo, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("CheckShipping");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteDataSet();
        }

        /// <summary>
        /// send the short message to notice the customer
        /// </summary>
        /// <param name="body">short message body</param>
        public static void SendTheShortMessage(SMSInfo body, JobContext context)
        {
            if (string.IsNullOrEmpty(body.CellPhoneNum)) return;
            else
                body.CellPhoneNum = Util.TrimNull(body.CellPhoneNum);
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Message/SendSMS", body, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }
        }

        /// <summary>
        /// send the email notice custmer leave the comment for product.
        /// </summary>
        /// <param name="sysNo">order sysno</param>
        public static void SendTheAddPointEmail(int soSysNo, JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["SORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/SO/Job/SendCommentNotifyMail", soSysNo, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }
        }

        /// <summary>
        /// Abandon the so by sosysno.
        /// </summary>
        /// <param name="soMaster"></param>
        public static void AbandonSO(int sysNo,JobContext context)
        {
            SOAbandonReq req = new SOAbandonReq();
            req.SOSysNoList = new System.Collections.Generic.List<int> { sysNo };

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["SORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update("/SO/Abandon", req, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
                throw new Exception(errorMsg);
            }
        }


        /// <summary>
        /// write the log when the bussiness exception orrour
        /// </summary>
        /// <param name="content"></param>
        public static void WriteTheExcetpionLog(string content)
        {
            Logger.WriteLog(content, "ExceptionLog");
        }
    }
}
