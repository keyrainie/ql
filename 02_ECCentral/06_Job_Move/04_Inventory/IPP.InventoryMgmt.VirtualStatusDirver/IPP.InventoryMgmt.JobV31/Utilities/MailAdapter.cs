using ECCentral.BizEntity.Common;
using ECCentral.Job.Utility;
using IPP.OrderMgmt.JobV31;

namespace IPP.InventoryMgmt.JobV31
{
    public class MailAdapter
    {
        public static void Send(MailInfo mailInfor)
        {
            //设置收件人信息
            if (string.IsNullOrEmpty(mailInfor.ToName))
            {
                return;
            }
            else
            {
                mailInfor.ToName = Util.TrimNull(mailInfor.ToName);
            }

            //获取配置参数
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["CommonRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];

            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/Message/SendMail", mailInfor, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = string.Empty;
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }
        }
    }
}
