using ECCentral.BizEntity.SO;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.IM.EasiPayProductDeclare
{
    public class Processor : IJobAction
    {
        public void Run(JobContext context)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            client.Timeout = 100000;
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            companyCode = companyCode == null ? null : (companyCode.Trim() == String.Empty ? null : companyCode.Trim());

            List<WaitDeclareProduct> productList = new List<WaitDeclareProduct>();
            ECCentral.Job.Utility.RestServiceError error;
            #region 获取审核通过的商品，进行提交商检操作
            client.Query<List<WaitDeclareProduct>>("/SO/Job/GetEntryAuditSucess", out productList, out error);
            if (productList != null && productList.Count > 0)
            {
                for (int i = 0; i < productList.Count; i += 10)
                {
                    client.Update("/SO/Job/MarkInInspection", productList.Skip(i).Take(10).ToList(), out error);
                    if (error != null)
                    {
                        string errorMessage = "商品申请商检:";
                        foreach (var errItem in error.Faults)
                        {
                            errorMessage += errItem.ErrorDescription;
                        }
                        context.Message += errorMessage + "\r\n";
                    }
                }
            }
            else
            {
                context.Message += "商品申请商检数量：0" + "\r\n";
            }
            #endregion
            #region 获取商检中的商品，进行商检通过操作
            client.Query<List<WaitDeclareProduct>>("/SO/Job/GetInInspectionProduct", out productList, out error);
            if (productList != null && productList.Count > 0)
            {
                for (int i = 0; i < productList.Count; i += 10)
                {
                    client.Update("/SO/Job/MarkSuccessInspection", productList.Skip(i).Take(10).ToList(), out error);
                    if (error != null)
                    {
                        string errorMessage = "商品通过商检:";
                        foreach (var errItem in error.Faults)
                        {
                            errorMessage += errItem.ErrorDescription;
                        }
                        context.Message += errorMessage + "\r\n";
                    }
                }
            }
            else
            {
                context.Message += "商品通过商检数量：0" + "\r\n";
            }
            #endregion
            #region 获取商检通过的商品，进行报关申请
            client.Query<List<WaitDeclareProduct>>("/SO/Job/GetWaitDeclareProduct", out productList, out error);
            if (productList != null && productList.Count > 0)
            {
                var group = productList.GroupBy(t => t.MerchantSysNo);
                var enumerator = group.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var pList = enumerator.Current.ToList();
                    int totalDeclareCnt = pList.Count;
                    int successCnt = 0;
                    int failCnt = 0;
                    for (int i = 0; i < pList.Count; i += 10)
                    {
                        DeclareProductResult bResult;
                        var data = pList.Skip(i).Take(10).ToList();
                        client.Query("/SO/Job/DeclareProduct", data, out bResult, out error);
                        if (error != null)
                        {
                            string errorMessage = "商品申请报关:";
                            foreach (var errItem in error.Faults)
                            {
                                errorMessage += errItem.ErrorDescription;
                            }
                            context.Message += errorMessage + "\r\n";
                        }
                        if (bResult.Success)
                            successCnt += data.Count;
                        else
                            failCnt += data.Count;
                        context.Message += bResult.Message + "\r\n";
                    }
                    context.Message += string.Format("商家【{3}】商品总申报数量：{0}，成功数量：{1}，失败数量：{2}", totalDeclareCnt, successCnt, failCnt, enumerator.Current.FirstOrDefault().MerchantSysNo) + "\r\n";
                }
            }
            else
            {
                context.Message += "商品总申报数量：0" + "\r\n";
            }
            #endregion
        }
    }
}
