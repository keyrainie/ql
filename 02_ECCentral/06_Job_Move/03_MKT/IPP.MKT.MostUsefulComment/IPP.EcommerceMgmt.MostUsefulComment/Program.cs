using System;
using System.Configuration;
using IPP.EcommerceMgmt.MostUsefulComment.BizProcess;

namespace IPP.EcommerceMgmt.MostUsefulComment
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new MostUsefulCommentBP().Process();
                //logger.WriteLog("最有用评论处理完成");

                string errorLog = ConfigurationManager.AppSettings["ErrorLog"];
                Log.WriteLog("最有用评论处理完成", errorLog);
            }
            catch (Exception ex)
            {
                //MailHelper.SendMail(ex.ToString(), "最有用评论Job出现异常,请联系管理员！");

                string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
                string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];

                ECCentral.BizEntity.Common.MailInfo mInfo = new ECCentral.BizEntity.Common.MailInfo();
                mInfo.Subject = "最有用评论Job出现异常,请联系管理员！";
                mInfo.Body = ex.ToString();
                mInfo.FromName = ConfigurationManager.AppSettings["EmailFrom"];
                mInfo.ToName = ConfigurationManager.AppSettings["EmailTo"];
                mInfo.CCName = ConfigurationManager.AppSettings["EmailCC"];
                mInfo.Priority = 1;

                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;
                var ar = client.Create("/Message/SendMail", mInfo, out error);
                string errorLog = ConfigurationManager.AppSettings["ErrorLog"];
                Log.WriteLog(ex.ToString(), errorLog);
            }
        }
    }
}