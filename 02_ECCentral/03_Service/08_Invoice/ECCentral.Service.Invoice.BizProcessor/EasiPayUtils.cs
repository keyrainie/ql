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
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.Invoice.BizProcessor
{
    public class EasiPayUtils
    {
        /// <summary>
        /// 退款
        /// 保证流水号唯一退款前缀添加R
        /// </summary>
        /// <param name="entity">退款实体信息</param>
        /// <returns>退款结果</returns>
        public RefundResult Refund(RefundEntity entity)
        {
            string refundUrl = AppSettingManager.GetSetting("Invoice", "RefundUrl");
            string bgUrl = AppSettingManager.GetSetting("Invoice", "BGURL");

            VendorCustomsInfo customsInfo = ObjectFactory<ISOBizInteract>.Instance.LoadVendorCustomsInfo(entity.SOSysNo);
            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams["SRC_NCODE"] = customsInfo.CBTSRC_NCode;
            reqParams["BILL_NO"] = entity.SOSysNo.ToString();

            reqParams["REFUND_AMOUNT"] = entity.RefundAmt.ToString("F2");
            reqParams["CARGO_AMOUNT"] = entity.ProductAmount.GetValueOrDefault().ToString("F2");

            reqParams["TRANSPORT_AMOUNT"] = entity.ShippingFeeAmount.GetValueOrDefault().ToString("F2");
            reqParams["TAX_AMOUNT"] = entity.TaxFeeAmount.GetValueOrDefault().ToString("F2");
            reqParams["RDO_TIME"] = string.Empty;
            reqParams["BGURL"] = bgUrl;

            StringBuilder reqXml = new StringBuilder();
            reqXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><EasipayB2CRequest><ReqData>");
            foreach (KeyValuePair<string, string> kvp in reqParams)
            {
                reqXml.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", kvp.Key, kvp.Value);
            }
            reqXml.Append("</ReqData></EasipayB2CRequest>");
            var resultXml = HttpPostRequestReturnXml(refundUrl, BuildPostReqData(reqXml.ToString(), customsInfo));

            var result = new RefundResult();
            var code = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RTN_CODE").InnerText;//请求结果
            result.Message = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RTN_INFO").InnerText;//拒绝原因

            if (code == "000000") {
                result.Result = true;
                var soSysNo = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/BILL_NO").InnerText;//商户订单号
                result.ExternalKey = string.Format("R" + resultXml.SelectSingleNode("/EasipayB2CResponse/ResData/REFTRX_SERNO").InnerText);//退款流水
                var refundAmout = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/REFUND_AMOUNT").InnerText;//退款总金额
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
            string queryUrl = AppSettingManager.GetSetting("Invoice", "QueryUrl");

            int SOID = 0;
            int.TryParse(soSysNo, out SOID);
            VendorCustomsInfo customsInfo = ObjectFactory<ISOBizInteract>.Instance.LoadVendorCustomsInfo(SOID);
            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams["SRC_NCODE"] = customsInfo.CBTSRC_NCode;
            reqParams["TRX_TYPE"] = "00";
            reqParams["BILL_NO"] = soSysNo;

            StringBuilder reqXml = new StringBuilder();
            reqXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><EasipayB2CRequest><ReqData>");
            foreach (KeyValuePair<string, string> kvp in reqParams)
            {
                reqXml.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", kvp.Key, kvp.Value);
            }
            reqXml.Append("</ReqData></EasipayB2CRequest>");
            var resultXml = HttpPostRequestReturnXml(queryUrl, BuildPostReqData(reqXml.ToString(), customsInfo));

            var result = new TransactionQueryBill();

            var code = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RTN_CODE").InnerText;//请求结果
            result.Message = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RTN_INFO").InnerText;//拒绝原因

            if (code == "000000")
            {
                result.IsTrue = true;
                string trxState = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/TRX_STATE").InnerText;
                result.BillNo = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/BILL_NO").InnerText;
                result.PayAmount = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/PAY_AMOUNT").InnerText;
                result.RdoTime = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RDO_TIME").InnerText;
                switch (trxState)
                {
                    case "QS":
                        result.TrxState = "等待支付";
                        break;
                    case "S":
                        result.TrxState = "支付成功";
                        break;
                    case "F":
                        result.TrxState = "支付失败";
                        break;
                }
            }
            
            return result;
        }

        /// <summary>
        /// 同步对账单
        /// </summary>
        /// <param name="billType">
        /// 交易类型
        /// 1 交易对账
        /// 2 实扣税费对账
        /// 3 保证金对账
        /// 4 外币账户对账
        /// </param>
        /// <param name="date">日期，格式：yyyyMMdd</param>
        /// <returns></returns>
        public bool SyncTradeBill(string billType, string date)
        {
            //获取所有需要对账的关务对接相关信息
            var list = ObjectFactory<SOIncomeProcessor>.Instance.QueryVendorCustomsInfo(); ;
            //循环 逐个对账
            foreach (VendorCustomsInfo customsInfo in list)
            {
                Dictionary<string, string> reqParams = new Dictionary<string, string>();
                //一级商户代码
                reqParams["SRC_NCODE"] = customsInfo.CBTSRC_NCode;
                //对账日期
                reqParams["DATE"] = date;
                //账单类别
                reqParams["ACCOUNT_TYPE"] = billType;

                StringBuilder reqXml = new StringBuilder();
                reqXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><EasipayB2CRequest><ReqData>");
                foreach (KeyValuePair<string, string> kvp in reqParams)
                {
                    reqXml.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", kvp.Key, kvp.Value);
                }
                reqXml.Append("</ReqData></EasipayB2CRequest>");

                string resResult = HttpPostRequestReturnString(AppSettingManager.GetSetting("Invoice", "CheckUrl"), BuildPostReqData(reqXml.ToString(), customsInfo), true);
                string[] resResultArray = resResult.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in resResultArray)
                {
                    string[] itemArray = item.Split('|');
                    if (itemArray == null || itemArray.Length != 15)
                    {
                        ECCentral.Service.Utility.Logger.WriteLog(string.Format("同步对账单失败，本条记录：{0}", item), "同步对账单");
                        continue;
                    }
                    TransactionCheckBill bill = new TransactionCheckBill()
                    {
                        TransactionType = itemArray[0].Equals("P") ? CheckTransactionType.P : CheckTransactionType.R,
                        SoSysNo = int.Parse(itemArray[1]),
                        SerialNo = string.Format("{0}{1}", itemArray[0].Equals("P") ? "P" : "R", itemArray[2]),
                        SubOrderTime = itemArray[3],
                        ProcessingTime = itemArray[4],
                        TotalAmount = decimal.Parse(itemArray[5]),
                        ProductAmount = decimal.Parse(itemArray[6]),
                        ForexCurrency = itemArray[7],
                        ForexAmount = decimal.Parse(itemArray[8]),
                        Tariff = decimal.Parse(itemArray[10]),
                        FareAmount = decimal.Parse(itemArray[11]),
                        FareCurrency = itemArray[12],
                        SubtotalAmount = decimal.Parse(itemArray[13]),
                        MerchantName = itemArray[14]
                    };
                    if (!string.IsNullOrWhiteSpace(itemArray[9]))
                        bill.Exchange = decimal.Parse(itemArray[9]);
                    ObjectFactory<IInvoiceDA>.Instance.InsertTransactionCheckBill(bill);
                }
            }
            return true;
        }

        /// <summary>
        /// 构造请求的协议数据
        /// </summary>
        /// <param name="reqXmlValue"></param>
        /// <returns></returns>
        private string BuildPostReqData(string reqXmlValue, VendorCustomsInfo customsInfo)
        {
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("SENDER_CODE={0}", customsInfo.CBTMerchantCode);
            postData.AppendFormat("&TRX_CONTENT={0}", Base64Encode(reqXmlValue).Replace("+", "%2B"));
            postData.AppendFormat("&SIGNATURE={0}", SignData(reqXmlValue, customsInfo));
            return postData.ToString();
        }

        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="reqXmlValue">请求的业务xml值，原始xml值</param>
        /// <returns>签名数据</returns>
        private string SignData(string reqXmlValue, VendorCustomsInfo customsInfo)
        {
            string sourceSignValue = "{0}^{2}^{3}^{1}";
            string merchantCode = customsInfo.CBTMerchantCode;
            string certKey = customsInfo.EasiPaySecretKey;
            string prefixCertKey = certKey.Substring(0, 64);
            string suffixCertKey = certKey.Substring(64, 64);
            reqXmlValue = Base64Encode(reqXmlValue);
            sourceSignValue = string.Format(sourceSignValue, prefixCertKey, suffixCertKey, merchantCode, reqXmlValue);
            return Hash_MD5.GetMD5(sourceSignValue).ToUpper();
        }

        #region HTTP POST DATA

        private static XmlDocument HttpPostRequestReturnXml(string url, string reqContent)
        {
            string encoding = "UTF-8";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(HttpPostRequestReturnString(url, reqContent, encoding));
            return xmlDoc;
        }
        private static string HttpPostRequestReturnString(string url, string reqContent, bool bIsGzip = false)
        {
            return HttpPostRequestReturnString(url, reqContent, "UTF-8", bIsGzip);
        }
        private static string HttpPostRequestReturnString(string url, string reqContent, string encoding, bool bIsGzip = false)
        {
            HttpWebResponse response = CreatePostHttpResponse(url, reqContent, encoding, null, bIsGzip);
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
        private static HttpWebResponse CreatePostHttpResponse(string url, string reqContent, string encoding, CookieCollection cookies, bool bIsGzip = false)
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
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (bIsGzip)
                request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Method = "POST";
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

        #region Base64

        /// <summary>
        /// 进行base64编码
        /// </summary>
        /// <param name="data">被编码数据</param>
        /// <returns></returns>
        private static string Base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in Base64Encode, message:{0}", ex.Message));
            }
        }

        /// <summary>
        /// 进行base64解码
        /// </summary>
        /// <param name="data">被解码数据</param>
        /// <returns></returns>
        private static string Base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in Base64Decode, message:{0}", ex.Message));
            }
        }
        #endregion
    }
}
