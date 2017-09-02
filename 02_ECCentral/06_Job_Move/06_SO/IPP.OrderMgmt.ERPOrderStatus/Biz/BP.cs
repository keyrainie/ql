using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;
using ERPOrderStatus.DAL;
using System.Xml;

using System.Transactions;
using ECCentral.BizEntity.ExternalSYS;
using IPP.OrderMgmt.ERPOrderStatus;
using ECCentral.BizEntity.Invoice;


namespace ERPOrderStatus.Biz
{
    public static class BP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;     
        private static JobContext CurrentContext;       
        #endregion
        public static void DoWork(JobContext Context)
        {
            CurrentContext = Context;
            var data = DA.GetERPOrder();
            List<ERPShippingInfo> orderList = new List<ERPShippingInfo>();
            OnShowInfo(string.Format("获取到{0}个ERP订单", data.Count));
            foreach (var item in data)
            {
                ERPShippingInfo info = new ERPShippingInfo
                {
                    RefOrderNo = item.RefOrderNo,
                    RefOrderType = "销售订单"
                };
                orderList.Add(info);
            }
            OnShowInfo(string.Format("调用ERP接口获取送货单列表..."));
            orderList = CallSystemCreateService(orderList);
            OnShowInfo(string.Format("ERP接口返回列表数：{0}", orderList.Count));
            var result = orderList.Where(p => p.ShippingStatus == 1).Select(p => new ErpOrder { RefOrderNo = p.RefOrderNo }).ToList();
            OnShowInfo(string.Format("有{0}个订单需要回刷状态", result.Count()));
            foreach (var item in result)
            {
                DA.UpdateErpOrder(item);
            }
            OnShowInfo(string.Format("ERP订单回刷状态完成。", result.Count()));
            var huodao = DA.GetHuoDao(result);
            OnShowInfo(string.Format("有{0}个ERP订单为货到订单，需要生成收款单", huodao.Count()));
            CallCreateSOIncomeService(huodao);
            OnShowInfo(string.Format("收款单创建完成", huodao.Count()));
            OnShowInfo(string.Format("全部工作完成。", huodao.Count()));
        }

        private static List<ERPShippingInfo> CallSystemCreateService(List<ERPShippingInfo> orderList)
        {
            
            //获取配置参数
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            int? UserSysNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UserSysNo"]);

                
                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;
                var ar = client.Create<List<ERPShippingInfo>>("/ExternalSYSService/ERP/GetERPShippingInfoByRefOrder", orderList, out orderList, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string errorMsg = string.Empty;
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }
                    OnShowInfo(errorMsg);
                }
                return orderList;          
        }
        private static List<ErpOrder> CallCreateSOIncomeService(List<ErpOrder> orderList)
        {

            //获取配置参数
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["RestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            int? UserSysNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UserSysNo"]);
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            foreach (var item in orderList)
            {                
                SOIncomeInfo so = new SOIncomeInfo
                {
                    OrderType = SOIncomeOrderType.SO,
                    OrderSysNo= Convert.ToInt32(item.RefOrderNo),
                    OrderAmt=item.OrderAmt,
                    IncomeStyle = SOIncomeOrderStyle.Normal,
                    IncomeAmt = item.OrderAmt,
                    PrepayAmt=0,
                    Status = SOIncomeStatus.Origin,
                    PayAmount = item.OrderAmt,
                    CompanyCode="8601"

                };
                var ar = client.Create("/InvoiceService/SOIncome/CreatSOIncome", so, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string errorMsg = string.Empty;
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }
                    OnShowInfo(errorMsg);
                }
            }            
            return orderList;
        }

        private static void OnShowInfo(string info)
        {
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }             
        }      
       
    }
}
