using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor.Job
{
    [VersionExport(typeof(SOCPSSendProcessor))]
    public class SOCPSSendProcessor
    {
        #region Member
        
        private string logPath = string.Empty;
        private static WebProxy webProxy;
        private string m_targetUrl;
        private string m_spCode;
        private decimal m_fanli;
        private string m_companyCode;

        #endregion

        public SOCPSSendProcessor()
        {
            webProxy = GetProxy();
        }

        public void Run(string targetUrl, string spCode, decimal fanli, string companyCode)
        {
            m_targetUrl = targetUrl;
            m_spCode = spCode;
            m_fanli = fanli;
            m_companyCode = companyCode;

            List<SOInfo> entityList = ObjectFactory<ISODA>.Instance.GetCPSList(companyCode);

            foreach (var entity in entityList)
            {
                entity.Items = ObjectFactory<ISODA>.Instance.GetSOItemsBySOSysNo(entity.SysNo.Value);
                SendOrder(entity);
            }
        }

        public void SendOrder(SOInfo entity)
        {
            int? returnStatus = null;
            string returnMsg = null;
            string targetUrl = GetTargetUrl(entity);

            WebClient client = new WebClient();
            client.Proxy = webProxy;

#warning 暂时由service请求,以后需要移植到DataFeed
            string data = client.DownloadString(targetUrl);
            //测试返回数据
            //string data = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><A><Received>1</Received><ReturnMsg>Test</ReturnMsg></A>";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);
                var receiveds = doc.GetElementsByTagName("Received");
                if (receiveds.Count == 1)
                {
                    returnStatus = int.Parse(receiveds[0].InnerText);
                }
                var returnMsgs = doc.GetElementsByTagName("ReturnMsg");
                if (returnMsgs.Count == 1)
                {
                    returnMsg = returnMsgs[0].InnerText;
                }
                ObjectFactory<ISODA>.Instance.InsertCPSLog(entity.SysNo.Value, targetUrl, returnMsg, returnStatus.Value);
            }
            catch (Exception ex)
            {
                //保证其他程序继续运行
                ExceptionHelper.HandleException(ex);
            }
        }

        private WebProxy GetProxy()
        {
            string address = AppSettingManager.GetSetting(SOConst.DomainName, "SOJob_CPSSend_Proxy");
            string username = AppSettingManager.GetSetting(SOConst.DomainName, "SOJob_CPSSend_Proxy");
            string password = AppSettingManager.GetSetting(SOConst.DomainName, "SOJob_CPSSend_Proxy");

            WebProxy webProxy = new WebProxy(address);
            webProxy.Credentials = new System.Net.NetworkCredential(username, password);
            return webProxy;
        }

        public string GetTargetUrl(SOInfo entity)
        {
            decimal validMoney = GetValidMoney(entity);
            decimal totalMoney = GetTotalMoney(entity);
            StringBuilder builder = new StringBuilder();
            builder.Append(m_targetUrl);
            builder.AppendFormat("?spcode={0}",m_spCode);
            builder.AppendFormat("&unionParams={0}",entity.UnionParams);
            builder.AppendFormat("&orderCode={0}",entity.SysNo);
            builder.AppendFormat("&fanli={0}", validMoney * m_fanli);
            builder.AppendFormat("&orderTime={0}", entity.BaseInfo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendFormat("&orderMoney={0}", validMoney);
            builder.AppendFormat("&orderTotalMoney={0}", totalMoney);
            builder.AppendFormat("&productCount={0}", entity.Items.Sum(p => p.Quantity.Value));
            builder.AppendFormat("&productId={0}", string.Join(",", entity.Items.Select(p => p.ProductSysNo.ToString())));
            builder.AppendFormat("&productNum={0}", string.Join(",", entity.Items.Select(p => p.Quantity.ToString())));
            builder.AppendFormat("&productPrice={0}", string.Join(",", entity.Items.Select(p => p.OriginalPrice.ToString())));
            builder.AppendFormat("&productFlMoney={0}", string.Join(",", entity.Items.Select(p => (p.OriginalPrice.Value * m_fanli).ToString())));
            return builder.ToString();
        }

        private decimal GetTotalMoney(SOInfo entity)
        {
            return entity.BaseInfo.SOAmount.Value 
                + entity.BaseInfo.ShipPrice.Value
                + entity.BaseInfo.PayPrice.Value 
                + entity.BaseInfo.PremiumAmount.Value;
        }

        private decimal GetValidMoney(SOInfo entity)
        {
            return entity.BaseInfo.ShipPrice.Value
                    + entity.BaseInfo.PromotionAmount.Value
                    + entity.BaseInfo.CashPay
                    + entity.BaseInfo.PremiumAmount.Value
                    + entity.BaseInfo.PayPrice.Value;
        }
    }
}
