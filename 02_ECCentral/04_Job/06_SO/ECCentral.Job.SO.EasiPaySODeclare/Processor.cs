using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Job.SO.EasiPaySODeclare
{
    public class Processor : Newegg.Oversea.Framework.JobConsole.Client.IJobAction
    {
        public void Run(Newegg.Oversea.Framework.JobConsole.Client.JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            client.Timeout = 100000;
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());

            ECCentral.Job.Utility.RestServiceError error;

            List<WaitDeclareSO> waitDeclareSOList = new List<WaitDeclareSO>();
            client.Query<List<WaitDeclareSO>>("/SO/Job/GetWaitDeclareSO", out waitDeclareSOList, out error);
            if (waitDeclareSOList != null && waitDeclareSOList.Count > 0)
            {
                int totalDeclareCnt = waitDeclareSOList.Count;
                int successCnt = 0;
                int failCnt = 0;
                waitDeclareSOList = waitDeclareSOList.FindAll(m => !string.IsNullOrWhiteSpace(m.TrackingNumber));
                foreach (var item in waitDeclareSOList)
                {
                    bool bResult = false;
                    client.Query("/SO/Job/DeclareSO", item, out bResult, out error);

                    if (error != null)
                    {
                        string errorMessage = "";
                        foreach (var errItem in error.Faults)
                        {
                            errorMessage = errItem.ErrorDescription;
                        }
                        throw new Exception(error.StatusDescription + errorMessage);
                    }
                    if (bResult)
                        successCnt++;
                    else
                        failCnt++;
                }
                context.Message = string.Format("总申报数量：{0}，成功数量：{1}，失败数量：{2}", totalDeclareCnt, successCnt, failCnt);
            }
            else
            {
                context.Message = "总申报数量：0";
            }
        }
    }
}
