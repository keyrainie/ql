using System;
using IPP.EcommerceMgmt.SendCustomerGuidePoints.Entities;
using IPP.EcommerceMgmt.SendCustomerGuidePoints.BizProcess;
using System.Configuration;

namespace IPP.EcommerceMgmt.SendCustomerGuidePoints
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new SendCustomerGuidePointsBP().Process();
                Log.WriteLog("用户指南发积分处理完成", "Log\\ServiceInfo.txt");
            }
            catch (Exception ex)
            {
                //MailHelper.SendMail(ex.ToString(), "用户指南发积分Job出现异常,请联系管理员！");

                string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
                string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];

                ECCentral.BizEntity.Common.MailInfo mInfo = new ECCentral.BizEntity.Common.MailInfo();
                mInfo.Subject = "用户指南发积分Job出现异常,请联系管理员！";
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
    }
}