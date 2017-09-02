using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using ECommerce.Entity.Payment;

namespace ECommerce.Facade.Payment.Charge
{
    public class ChargeTenPay : Charges
    {
        public override void SetRequestForm(ChargeContext context)
        {
            context.SOInfo.SellerSysNo = 1;
            context.RequestForm["sign_type"] = "MD5";
            context.RequestForm["service_version"] = "1.0";
            context.RequestForm["input_charset"] = context.PaymentInfoMerchant.Encoding;//编码
            context.RequestForm["sign_key_index"] = "1";
            context.RequestForm["bank_type"] = "DEFAULT";
            context.RequestForm["body"] = "KJT_SO_" + context.SOInfo.SoSysNo.ToString();//商品标题
            //context.RequestForm["attach"] = "";
            context.RequestForm["return_url"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentCallbackUrl);//针对该交易支付成功之后的前台通知URL
            context.RequestForm["notify_url"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);//针对该交易支付成功之后的通知接收URL
            //context.RequestForm["buyer_id"] = "";
            context.RequestForm["partner"] = context.PaymentInfoMerchant.MerchantNO;//境外商户在财付通的用户ID
            context.RequestForm["out_trade_no"] = context.SOInfo.SoSysNo.ToString();//境外商户交易号，订单号
            context.RequestForm["total_fee"] = ((int)(100 * context.SOInfo.RealPayAmt)).ToString();
            context.RequestForm["fee_type"] = context.PaymentInfoMerchant.CurCode;//币种
            context.RequestForm["spbill_create_ip"] = GetCientIP();//IP
            //风控字段
            context.RequestForm["risk_info"] = HttpUtility.UrlEncode("line_type=on&goods_type=phy&deliver=log&&seller_tag=mer");

            //context.RequestForm["time_start"] = "";
            //context.RequestForm["time_expire"] = "";
            //context.RequestForm["transport_fee"] = "";
            //context.RequestForm["product_fee"] = "";
            //context.RequestForm["goods_tag"] = "";
            context.RequestForm["sign"] = SignData(context).ToUpper();//签名
        }

        public override bool VerifySign(CallbackContext context)
        {
            if (context != null && context.ResponseForm != null && context.ResponseForm.Count > 0)
            {
                context.SOInfo.SellerSysNo = 1;
                string sign = string.Empty;
                if (context.ResponseForm.AllKeys.Contains("sign"))
                {
                    sign = context.ResponseForm["sign"];
                }
                string sourceMD5Value = "";
                string[] keys = context.ResponseForm.AllKeys;
                Array.Sort(keys);
                foreach (var item in keys)
                {
                    if (item == "sign")
                        continue;
                    sourceMD5Value += string.Format("{0}={1}&", item, context.ResponseForm[item]);
                }
                sourceMD5Value += string.Format("key={0}", context.PaymentInfoMerchant.MerchantCertKey);

                if (sign.ToLower() == GetMD5(sourceMD5Value, context.PaymentInfoMerchant.Encoding).ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public override bool GetPayResult(CallbackContext context)
        {
            return context.ResponseForm["trade_state"].Equals("0");
        }

        public override int GetSOSysNo(CallbackContext context)
        {
            return int.Parse(context.ResponseForm["out_trade_no"]);
        }

        public override decimal GetPayAmount(CallbackContext context)
        {
            return decimal.Parse(context.ResponseForm["total_fee"]) / 100m;
        }

        public override string GetSerialNumber(CallbackContext context)
        {
            return context.ResponseForm["transaction_id"];
        }

        public override string GetPayProcessTime(CallbackContext context)
        {
            return context.ResponseForm["time_end"];
        }

        private string SignData(ChargeContext context)
        {
            string md5SourceValue = "";
            string[] allKeys = context.RequestForm.AllKeys;
            Array.Sort(allKeys);
            foreach (var item in allKeys)
            {
                if (item == "sign")
                    continue;
                string value = context.RequestForm[item];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    md5SourceValue += string.Format("{0}={1}&", item, value);
                }
            }
            md5SourceValue += string.Format("key={0}", context.PaymentInfoMerchant.MerchantCertKey);

            string signStr = GetMD5(md5SourceValue, context.PaymentInfoMerchant.Encoding).ToUpper();

            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
            {
                string sourceData = BuildStringFromNameValueCollection(context.RequestForm);
                ECommerce.Utility.Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceData, md5SourceValue, signStr), "TenPay", "PaySignData");
            }

            return signStr;
        }

        #region HTTP DATA

        private static string HttpRequest(string url, string method, string reqContent)
        {
            string encoding = "UTF-8";
            HttpWebResponse response = CreateHttpResponse(url, method, reqContent, encoding, null);
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding)))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>  
        /// 创建HTTP请求
        /// </summary>  
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        private static HttpWebResponse CreateHttpResponse(string url, string method, string reqContent, string encoding, CookieCollection cookies)
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

        #region IP
        private static string GetCientIP()
        {
            string ipAddress = string.Empty;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                }
            }
            else
            {
                ipAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return ipAddress;
        }
        #endregion
    }
}
