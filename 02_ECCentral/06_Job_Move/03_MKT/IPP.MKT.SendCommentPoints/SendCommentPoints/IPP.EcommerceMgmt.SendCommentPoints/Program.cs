using System;
using IPP.EcommerceMgmt.SendCommentPoints.BizProcess;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;
using IPP.EcommerceMgmt.SendCommentPoints.Entities;

namespace IPP.EcommerceMgmt.SendCommentPoints
{
    class Program : IJobAction
    {
        static void Main(string[] args)
        {
            try
            {
                SendCommentPointsBP.jobContext = new JobContext();
                SendCommentPointsBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
                new SendCommentPointsBP().Process();
                Log.WriteLog("最有用评论处理完成", "Log\\ServiceInfo.txt");
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

                Log.WriteLog(ex.ToString(), "Log\\ServiceInfo.txt");
            }
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            SendCommentPointsBP.jobContext = context;
            SendCommentPointsBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            new SendCommentPointsBP().Process();
        }

        #endregion
    }
}