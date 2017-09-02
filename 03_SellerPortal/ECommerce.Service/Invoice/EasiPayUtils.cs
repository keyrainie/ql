using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using ECommerce.Entity.Invoice;
using ECommerce.Service.Common;
using ECommerce.Utility;

namespace ECommerce.Service.Invoice
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

            Dictionary<string, string> reqParams = new Dictionary<string, string>();
            reqParams["SRC_NCODE"] = AppSettingManager.GetSetting("Invoice", "SRC_NCODE");
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
            var resultXml = HttpPostRequestReturnXml(refundUrl, BuildPostReqData(reqXml.ToString()));

            var result = new RefundResult();
            var code = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RTN_CODE").InnerText;//请求结果
            result.Message = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/RTN_INFO").InnerText;//拒绝原因

            if (code == "000000")
            {
                result.Result = true;
                var soSysNo = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/BILL_NO").InnerText;//商户订单号
                result.ExternalKey = string.Format("R" + resultXml.SelectSingleNode("/EasipayB2CResponse/ResData/REFTRX_SERNO").InnerText);//退款流水
                var refundAmout = resultXml.SelectSingleNode("EasipayB2CResponse/ResData/REFUND_AMOUNT").InnerText;//退款总金额
            }

            //记录日志
            string resultNote = string.Format("用户[{0}]对订单号：{1} 调用了退款接口.调用结果;{2} 调用返回信息：{3}，{4} PostUrl:{5} ", entity.UserSysNo, entity.SOSysNo, code, result.Message, resultXml.ToXmlString(), refundUrl);
            ECommerce.Utility.Logger.WriteLog(resultNote, "EasiPayUtils.Refund");

            return result;
        }

        /// <summary>
        /// 构造请求的协议数据
        /// </summary>
        /// <param name="reqXmlValue"></param>
        /// <returns></returns>
        private string BuildPostReqData(string reqXmlValue)
        {
            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("SENDER_CODE={0}", AppSettingManager.GetSetting("Invoice", "SENDER_CODE"));
            postData.AppendFormat("&TRX_CONTENT={0}", Base64Encode(reqXmlValue).Replace("+", "%2B"));
            postData.AppendFormat("&SIGNATURE={0}", SignData(reqXmlValue));
            return postData.ToString();
        }

        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="reqXmlValue">请求的业务xml值，原始xml值</param>
        /// <returns>签名数据</returns>
        private string SignData(string reqXmlValue)
        {
            string sourceSignValue = "{0}^{2}^{3}^{1}";
            string merchantCode = AppSettingManager.GetSetting("Invoice", "SENDER_CODE");
            string certKey = AppSettingManager.GetSetting("Invoice", "SecretKey");
            string prefixCertKey = certKey.Substring(0, 64);
            string suffixCertKey = certKey.Substring(64, 64);
            reqXmlValue = Base64Encode(reqXmlValue);
            sourceSignValue = string.Format(sourceSignValue, prefixCertKey, suffixCertKey, merchantCode, reqXmlValue);
            return GetMD5(sourceSignValue).ToUpper();
        }

        private static string GetMD5(string s)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(s));
            StringBuilder builder = new StringBuilder(0x20);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString();
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
