using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice.EBank;
using ECCentral.Service.Utility;
using System.Xml;
using ICSharpCode.SharpZipLib.BZip2;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.BizEntity.Invoice;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ECCentral.Service.Invoice.IDataAccess;
using System.Security.Cryptography;

namespace ECCentral.Service.Invoice.BizProcessor
{
    public class TenPayUtils
    {
        /// <summary>
        /// 退款
        /// 保证流水号唯一退款前缀添加R
        /// </summary>
        /// <param name="entity">退款实体信息</param>
        /// <returns>退款结果</returns>
        public RefundResult Refund(RefundEntity entity)
        {
            string refundUrl = AppSettingManager.GetSetting("Invoice", "TenPayRefundUrl");

            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams["input_charset"] = "UTF-8";
            reqParams["sign_key_index"] = "1";
            reqParams["service_version"] = "1.1";
            reqParams["partner"] = AppSettingManager.GetSetting("Invoice", "TenPayParnter");
            reqParams["out_trade_no"] = entity.SOSysNo.ToString();
            reqParams["out_refund_no"] = entity.RefundSysNo.ToString();
            reqParams["total_fee"] = (entity.SOAmt * 100).ToString();
            reqParams["refund_fee"] = (entity.RefundAmt * 100).ToString();
            reqParams["op_user_id"] = AppSettingManager.GetSetting("Invoice", "TenPayParnter");
            reqParams["op_user_passwd"] = GetMD5(AppSettingManager.GetSetting("Invoice", "TenPayParnterPwd"));
            reqParams["sign_type"] = "MD5";
            //reqParams["notify_url"] = GetMD5(AppSettingManager.GetSetting("Invoice", "TenPayRefundNotifyUrl"));

            string reqData = "";
            string[] allKeys = reqParams.Keys.ToArray();
            Array.Sort(allKeys);
            foreach (var item in allKeys)
            {
                reqData += string.Format("{0}={1}&", item, reqParams[item]);
            }
            string sign = GetMD5(reqData + string.Format("key={0}", AppSettingManager.GetSetting("Invoice", "TenPaySecretKey")));
            reqData += string.Format("sign={0}", sign);

            var resultXml = HttpRequestReturnXml(string.Format("{0}?{1}", refundUrl, reqData), "GET", "");

            var result = new RefundResult();
            var code = resultXml.SelectSingleNode("root/retcode").InnerText;//请求结果

            if (code == "0")
            {
                result.Result = true;
                var soSysNo = entity.SOSysNo.ToString();//商户订单号
                result.ExternalKey = resultXml.SelectSingleNode("root/transaction_id").InnerText;//退款流水，支付宝国际无
                var refundAmout = entity.RefundAmt.ToString("F2");//退款总金额                
            }
            else
            {
                result.Message = resultXml.SelectSingleNode("root/retmsg").InnerText;//拒绝原因
            }

            //记录日志
            string resultNote = string.Format("用户[{0}]对订单号：{1} 调用了退款接口.调用结果;{2} 调用返回信息：{3}，{4} PostUrl:{5} ", ServiceContext.Current.UserSysNo, entity.SOSysNo, code, result.Message, resultXml.ToXmlString(), refundUrl);
            ExternalDomainBroker.CreateOperationLog(resultNote, BizLogType.RMA_Refund_Refund, entity.RefundSysNo, entity.CompanyCode);
            return result;
        }

        /// <summary>
        /// 订单网关查询
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public TransactionQueryBill QueryBill(string soSysNo)
        {
            string queryUrl = AppSettingManager.GetSetting("Invoice", "TenPayQueryUrl");

            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams["input_charset"] = "UTF-8";
            reqParams["service_version"] = "1.0";
            reqParams["sign_key_index"] = "1";
            reqParams["partner"] = AppSettingManager.GetSetting("Invoice", "TenPayParnter");
            reqParams["out_trade_no"] = soSysNo.ToString();
            reqParams["sign_type"] = "MD5";

            string reqData = "";
            string[] allKeys = reqParams.Keys.ToArray();
            Array.Sort(allKeys);
            foreach (var item in allKeys)
            {
                reqData += string.Format("{0}={1}&", item, reqParams[item]);
            }
            string sign = GetMD5(reqData + string.Format("key={0}", AppSettingManager.GetSetting("Invoice", "TenPaySecretKey")));
            reqData += string.Format("sign={0}", sign);

            var resultXml = HttpRequestReturnXml(string.Format("{0}?{1}", queryUrl, reqData), "GET", "");

            var result = new TransactionQueryBill();
            var code = resultXml.SelectSingleNode("root/retcode").InnerText;//请求结果
            if (code == "0")
            {
                result.IsTrue = true;
                string trxState = resultXml.SelectSingleNode("root/trade_state").InnerText;
                result.BillNo = resultXml.SelectSingleNode("root/out_trade_no").InnerText;
                result.PayAmount = resultXml.SelectSingleNode("root/total_fee").InnerText;
                result.RdoTime = resultXml.SelectSingleNode("root/time_end").InnerText;
                switch (trxState)
                {
                    case "0":
                        result.TrxState = "支付成功";
                        break;
                    default:
                        result.TrxState = "等待买家付款";
                        break;
                }
            }
            else
            {
                result.Message = "查询失败";// resultXml.SelectSingleNode("root/retmsg").InnerText;//失败原因
            }
            return result;
        }

        /// <summary>
        /// 查询退款
        /// </summary>
        /// <param name="sysNo">退款单号</param>
        public void QueryRefund(int sysNo)
        {
            string queryUrl = AppSettingManager.GetSetting("Invoice", "TenPayQueryRefundUrl");

            var soincomInfo = ObjectFactory<ISOIncomeDA>.Instance.LoadBySysNo(sysNo);
            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams["input_charset"] = "UTF-8";
            reqParams["service_version"] = "1.0";
            reqParams["sign_key_index"] = "1";
            reqParams["partner"] = AppSettingManager.GetSetting("Invoice", "TenPayParnter");
            reqParams["out_trade_no"] = soincomInfo.OrderSysNo.ToString();
            reqParams["sign_type"] = "MD5";

            string reqData = "";
            string[] allKeys = reqParams.Keys.ToArray();
            Array.Sort(allKeys);
            foreach (var item in allKeys)
            {
                reqData += string.Format("{0}={1}&", item, reqParams[item]);
            }
            string sign = GetMD5(reqData + string.Format("key={0}", AppSettingManager.GetSetting("Invoice", "TenPaySecretKey")));
            reqData += string.Format("sign={0}", sign);

            var resultXml = HttpRequestReturnXml(string.Format("{0}?{1}", queryUrl, reqData), "GET", "");

            var result = new TransactionQueryBill();
            var code = resultXml.SelectSingleNode("root/retcode").InnerText;//请求结果
            if (code == "0")
            {
                SOIncomeInfo updateEntity = new SOIncomeInfo()
                {
                    SysNo = sysNo,
                    ExternalKey = resultXml.SelectSingleNode("root/refund_id_0").InnerText
                };
                string status = resultXml.SelectSingleNode("root/refund_state_0").InnerText;
                if (status == "4" || status == "10")
                {
                    updateEntity.Status = SOIncomeStatus.Processed;
                    ObjectFactory<ISOIncomeDA>.Instance.UpdateStatus(updateEntity);
                }
                else if (status == "3" || status == "5" || status == "6")
                {
                    updateEntity.Status = SOIncomeStatus.ProcessingFailed;
                    ObjectFactory<ISOIncomeDA>.Instance.UpdateStatus(updateEntity);
                }
            }
        }

        #region HTTP POST DATA

        private static XmlDocument HttpRequestReturnXml(string url, string method, string reqContent)
        {
            string encoding = "UTF-8";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(HttpRequestReturnString(url, method, reqContent, encoding));
            return xmlDoc;
        }
        private static string HttpRequestReturnString(string url, string method, string reqContent, bool bIsGzip = false)
        {
            return HttpRequestReturnString(url, method, reqContent, "UTF-8", bIsGzip);
        }
        private static string HttpRequestReturnString(string url, string method, string reqContent, string encoding, bool bIsGzip = false)
        {
            HttpWebResponse response = CreatePostHttpResponse(url, method, reqContent, encoding, null, bIsGzip);
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding)))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        private static HttpWebResponse CreatePostHttpResponse(string url, string method, string reqContent, string encoding, CookieCollection cookies, bool bIsGzip = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (string.IsNullOrWhiteSpace(encoding))
                encoding = "UTF-8";
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //如果是发送HTTPS请求  
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3; 
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ClientCertificates.Add(new X509Certificate2("c:\\key\\tenpay\\1900000109.pfx", AppSettingManager.GetSetting("Invoice", "TenPayParnter")));
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (bIsGzip)
                request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Timeout = 600000;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            if (!string.IsNullOrWhiteSpace(reqContent))
            {
                byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(reqContent);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //总是接受  
            return true;
        }

        #endregion

        #region MD5
        private static string GetMD5(string dataStr, string encoding = "UTF-8")
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(encoding).GetBytes(dataStr));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            string result = sb.ToString();

            return result;
        }
        #endregion
    }
}
