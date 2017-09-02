using ECCentral.BizEntity;
using ECCentral.Service.EventMessage.SZPointAlliance;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ECCentral.Service.EventConsumer
{
    public class SZPointAllianceConsumer : IConsumer<SZPointAllianceRequestMessage>
    {
        public SZPointAllianceResponseMessage SZResponse { get; set; }

        public void HandleEvent(SZPointAllianceRequestMessage eventMessage)
        {
            SZPointAllianceResponseMessage res = InitRequest(eventMessage);
            if (res.Result == 2)
            {
                if (string.IsNullOrEmpty(res.Message))
                {
                    res.Message = "调用神州运通接口出现错误，但并未返回错误信息";
                }
                throw new BizException(res.Message);
            }
            this.SZResponse = res;
        }

        private SZPointAllianceResponseMessage InitRequest(SZPointAllianceRequestMessage message)
        {
            string requestUrl = string.Empty;
            string requestType = string.Empty;
            if (message.RefundType == PointAllianceRefundType.Point)
            {
                requestType = "退积分";
                throw new InvalidOperationException("退积分尚未实现");
            }
            else
            {
                requestType = "退预付卡";
                requestUrl = AppSettingHelper.SZPointAlliance_RefundPrepaidCard;
            }

            string param = BuildParameter(message);

            WriteLog(string.Format("调用方:{0}|请求URL:{1}?{2}", "神州运通", requestUrl, param), string.Format("神州运通--{0}--请求日志", requestType));

            HttpWebRequest webRequest = CreateRequestProxy(requestUrl);
            string mes = string.Empty;

            byte[] paramBuffer = Encoding.UTF8.GetBytes(param);

            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(paramBuffer, 0, paramBuffer.Length);
            }

            try
            {
                WebResponse response = webRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        mes = reader.ReadToEnd();
                    }
                }
                if (mes != null)
                {
                    //在xml文件的开头有隐藏的非法字符,需要过滤
                    mes = System.Text.RegularExpressions.Regex.Replace(mes.Trim(), "^[^<]", string.Empty);
                }
                SZPointAllianceResponseMessage res = SerializationUtility.XmlDeserialize<SZPointAllianceResponseMessage>(mes);
                return res;
            }
            finally
            {
                WriteLog(string.Format("调用方:{0}|请求URL:{1}?{2}|返回信息：{3}", "神州运通", requestUrl, param, mes), string.Format("神州运通--{0}--返回日志", requestType));
            }
        }

        private static HttpWebRequest CreateRequestProxy(string requestUrl)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);
            req.Method = "POST";
            req.KeepAlive = true;
            req.Timeout = 300000;
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            req.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            bool isUseProxy = AppSettingHelper.SZPointAlliance_IsUseProxy;
            if (isUseProxy)
            {
                WebProxy proxy = new WebProxy();
                proxy.Address = new Uri(AppSettingHelper.SZPointAlliance_ProxyUrl);

                string username = AppSettingHelper.SZPointAlliance_ProxyUserID;
                string password = AppSettingHelper.SZPointAlliance_ProxyPassword;

                proxy.Credentials = new System.Net.NetworkCredential(username, password);
                req.Proxy = proxy;
            }
            return req;
        }

        private static string BuildParameter(SZPointAllianceRequestMessage request)
        {
            string parameter = string.Empty;
            switch (request.RefundType)
            {
                case PointAllianceRefundType.PrepaidCard:
                    parameter = BuildParameterForPrepaidCard(request);
                    break;
                case PointAllianceRefundType.Point:
                default:
                    throw new InvalidOperationException("退积分尚未实现");
            }
            return parameter;
        }

        /// <summary>
        /// 组建退预付款的请求参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string BuildParameterForPrepaidCard(SZPointAllianceRequestMessage request)
        {
            StringBuilder sb = new StringBuilder();
            int merchantId = int.Parse(AppSettingHelper.SZPointAlliance_MerchantID);
            string m_key = AppSettingHelper.SZPointAlliance_Key;
            string channel_type = AppSettingHelper.SZPointAlliance_ChannelType;

            string key = string.Format("{0}{1}{2}{3}{4}{5}",
                merchantId,
                request.SOSysNo,
                request.TNumber,
                request.RefundAmount.ToString("#######0.00"),
                request.RefundKey,
                m_key);

            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = bytes[i].ToString("X");
                if (hex.Length == 1)
                {
                    result.Append("0");
                }
                result.Append(hex);
            }
            string md5String = result.ToString();

            sb.AppendFormat("merchant_id={0}", merchantId);
            sb.AppendFormat("&order_id={0}", request.SOSysNo);
            sb.AppendFormat("&t_number={0}", request.TNumber);
            sb.AppendFormat("&refund_amount={0}", request.RefundAmount.ToString("#######0.00"));
            sb.AppendFormat("&key={0}", md5String);
            sb.AppendFormat("&refund_key={0}", System.Web.HttpUtility.UrlEncode(request.RefundKey));
            if (!string.IsNullOrEmpty(request.RefundDescription))
            {
                sb.AppendFormat("&refund_desc={0}", System.Web.HttpUtility.UrlEncode(request.RefundDescription));
            }

            return sb.ToString();
        }

        private void WriteLog(string content, string autherInfo)
        {
            LogEntry log = new LogEntry
            {
                Category = autherInfo,
                Content = content,
                GlobalName = "ECCentral",
                LocalName = "ThirdPart",
                ReferenceKey = autherInfo
            };
            List<ExtendedPropertyData> list = new List<ExtendedPropertyData>();
            list.Add(new ExtendedPropertyData() { PropertyName = "GlobalName", PropertyValue = "ECCentral" });
            list.Add(new ExtendedPropertyData() { PropertyName = "LocalName", PropertyValue = "ThirdPart" });
            Logger.WriteLog(log.Content, log.Category, log.ReferenceKey, list);
        }

        private static void CheckArguments(SZPointAllianceRequestMessage requestMsg)
        {
            if (requestMsg == null)
            {
                throw new ArgumentNullException("request");
            }
            if (requestMsg.SOSysNo <= 0)
            {
                throw new BizException(string.Format("无效订单号:{0}", requestMsg.SOSysNo));
            }
            if (requestMsg.RefundAmount == 0M)
            {
                throw new BizException(string.Format("订单号:{0},退款(积分或预付卡)的金额不能为0", requestMsg.SOSysNo));
            }
            if (string.IsNullOrEmpty(requestMsg.TNumber))
            {
                throw new BizException(string.Format("订单号:{0},无效的交易号{1}", requestMsg.SOSysNo, requestMsg.TNumber));
            }
            else if (requestMsg.TNumber.Trim().Length == 0)
            {
                throw new BizException(string.Format("订单号:{0},无效的交易号{1}", requestMsg.SOSysNo, requestMsg.TNumber));
            }
        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
