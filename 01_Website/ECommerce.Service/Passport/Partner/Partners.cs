using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;
using ECommerce.Entity.Passport;

namespace ECommerce.Facade.Passport.Partner
{
    public abstract class Partners
    {
        /// <summary>
        /// 设置请求参数
        /// </summary>
        /// <param name="context">第三方登录上下文</param>
        public abstract void SetRequestParam(PartnerContext context);

        /// <summary>
        /// 第三方登录回调验签
        /// </summary>
        /// <param name="context">第三方登录回调上下文</param>
        /// <returns></returns>
        public abstract bool BackVerifySign(PartnerBackContext context);

        /// <summary>
        /// 获取第三方登录回调用户信息
        /// </summary>
        /// <param name="context">第三方登录回调上下文</param>
        public abstract void GetResponseUserInfo(PartnerBackContext context);

        #region 获取第三方登录实例

        public static Partners GetInstance(PartnerContext context)
        {
            if (context != null && !string.IsNullOrWhiteSpace(context.PartnerIdentify))
            {
                context.PassportInfo = GetPassportInfo(context.PartnerIdentify);
                if (context.PassportInfo != null && context.PassportInfo.Parnter != null)
                {
                    return GetInstance(context.PassportInfo.Parnter.PartnerProcessor);
                }
            }

            return null;
        }

        public static Partners GetInstance(PartnerBackContext context)
        {
            if (context != null && !string.IsNullOrWhiteSpace(context.PartnerIdentify))
            {
                context.PassportInfo = GetPassportInfo(context.PartnerIdentify);
                if (context.PassportInfo != null && context.PassportInfo.Parnter != null)
                {
                    return GetInstance(context.PassportInfo.Parnter.PartnerProcessor);
                }
            }

            return null;
        }

        private static Partners GetInstance(string partnerProcessor)
        {
            if (!string.IsNullOrEmpty(partnerProcessor))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.CreateInstance(partnerProcessor) as Partners;
            }

            return null;
        }

        #endregion

        #region 获取一个第三方登录上送请求数据

        /// <summary>
        /// 根据支付上下文获取一个支付上送请求数据
        /// </summary>
        /// <param name="context">支付上下文</param>
        /// <returns></returns>
        public string GetRequestContent(PartnerContext context)
        {
            if (context != null && !string.IsNullOrWhiteSpace(context.PartnerIdentify))
            {
                if (context.RequestParam == null)
                {
                    context.RequestParam = new NameValueCollection();
                }
                SetRequestParam(context);
                return BuildRequestContent(context);
            }

            return string.Empty;
        }

        /// <summary>
        /// 构造上送请求数据
        /// </summary>
        /// <param name="context">支付上下文</param>
        /// <returns></returns>
        protected virtual string BuildRequestContent(PartnerContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (context.RequestParam != null && context.RequestParam.Count > 0)
            {
                foreach (string name in context.RequestParam)
                {
                    builder.AppendFormat("{0}={1}&", name, context.RequestParam[name]);
                }
                return string.Format("{0}?{1}", context.PassportInfo.Parnter.LoginUrl, builder.ToString().TrimEnd('&'));
            }
            return string.Empty;
        }

        #endregion

        #region 从配置文件获取配置信息

        /// <summary>
        /// 根据第三方标识获取第三方配置信息
        /// </summary>
        /// <param name="partnerIdentify">第三方标识</param>
        /// <returns></returns>
        private static PassportSetting GetPassportInfo(string partnerIdentify)
        {
            PassportSetting info = new PassportSetting();
            info.Base = new PassportBase();
            XDocument doc = XDocument.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/PassportPartner.config"));
            if (null != doc)
            {
                XElement root = doc.Root;
                XElement baseElement = root.Element("Base");
                info.Base.BaseUrl = GetElementValue(baseElement, "BaseUrl");

                XElement parnters = root.Element("Parnters");
                foreach (XElement item in parnters.Elements("Parnter"))
                {
                    if (!string.IsNullOrWhiteSpace(partnerIdentify))
                    {
                        if (item.Attribute("Identify").Value.Trim().ToLower().Equals(partnerIdentify.Trim().ToLower()))
                        {
                            info.Parnter = GetPassportParnter(item);
                            break;
                        }
                    }
                }
            }

            if (info.Parnter == null)
            {
                info.Parnter = new PassportParnter();
            }

            return info;
        }

        /// <summary>
        /// 根据节点获取节点配置信息
        /// </summary>
        /// <param name="item">节点</param>
        /// <returns></returns>
        private static PassportParnter GetPassportParnter(XElement item)
        {
            PassportParnter paymentMode = new PassportParnter();

            paymentMode.PartnerIdentify = item.Attribute("Identify").Value;
            paymentMode.Name = item.Attribute("Name").Value;
            paymentMode.RequestType = GetElementValue(item, "RequestType");
            paymentMode.PartnerProcessor = GetElementValue(item, "PartnerProcessor");
            paymentMode.LoginUrl = GetElementValue(item, "LoginUrl");
            paymentMode.AccessTokenUrl = GetElementValue(item, "AccessTokenUrl");
            paymentMode.OpenIDUrl = GetElementValue(item, "OpenIDUrl");
            paymentMode.LoginBackUrl = GetElementValue(item, "LoginBackUrl");
            paymentMode.GetUserInfoUrl = GetElementValue(item, "GetUserInfoUrl");
            paymentMode.AppID = GetElementValue(item, "AppID");
            paymentMode.AppSecret = GetElementValue(item, "AppSecret");
            paymentMode.Encoding = GetElementValue(item, "Encoding");
            paymentMode.Debug = GetElementValue(item, "Debug");
            paymentMode.CustomProperty1 = GetElementValue(item, "CustomProperty1");
            paymentMode.CustomProperty2 = GetElementValue(item, "CustomProperty2");
            paymentMode.CustomProperty3 = GetElementValue(item, "CustomProperty3");
            paymentMode.CustomProperty4 = GetElementValue(item, "CustomProperty4");
            paymentMode.CustomProperty5 = GetElementValue(item, "CustomProperty5");

            return paymentMode;
        }

        /// <summary>
        /// 获取子节点值
        /// </summary>
        /// <param name="parentElement">根节点</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        private static string GetElementValue(XElement parentElement, string key)
        {
            if (parentElement != null && !string.IsNullOrEmpty(key))
            {
                XElement element = parentElement.Element(key.Trim());
                if (element != null)
                    return element.Value;
            }

            return string.Empty;
        }

        #endregion

        #region 辅助工具方法

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="dataStr">被编码值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetMD5(string dataStr, string encoding)
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

        /// <summary>
        /// 构造一个网址
        /// </summary>
        /// <param name="baseUrl">基地址</param>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string BuildActionUrl(string baseUrl, string url)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(baseUrl))
            {
                return string.Format("{0}/{1}", baseUrl.TrimEnd('/'), url.TrimStart('/'));
            }

            return string.Empty;
        }

        /// <summary>
        /// 从集合中构造一个字符串
        /// </summary>
        /// <param name="collection">集合</param>
        /// <returns></returns>
        public static string BuildStringFromNameValueCollection(NameValueCollection collection)
        {
            string result = "";

            foreach (string str in collection.AllKeys)
            {
                result += string.Format("{0}={1}&", str, collection[str]);
            }
            result = result.TrimEnd(new char[] { '&' });
            //result.TrimEnd('&');

            return result;
        }

        /// <summary>
        /// 获取当前时间，格式：yyyyMMddHHmmss
        /// </summary>
        /// <returns></returns>
        public string GetNowTime2Timestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// URL编码，将转义符号编码结果大写
        /// </summary>
        /// <param name="sourceValue">原始值</param>
        /// <returns></returns>
        public static string SpecialUrlEncode(string sourceValue)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < sourceValue.Length; i++)
            {
                string t = sourceValue[i].ToString();
                string k = HttpUtility.UrlEncode(t);
                if (t == k)
                {
                    stringBuilder.Append(t);
                }
                else
                {
                    stringBuilder.Append(k.ToUpper());
                }
            }
            return stringBuilder.ToString();
        }

        #region Request
        public static string HttpPostRequest(string url, string reqContent, string contentType, string encoding)
        {
            byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(reqContent);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            request.ContentLength = bytes.Length;
            request.ContentType = contentType;
            request.Method = "POST";
            request.KeepAlive = true;

            using (Stream rs = request.GetRequestStream())
            {
                rs.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding)))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public static string HttpGetRequest(string url, string encoding)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "*/*";
            request.Method = "GET";
            request.KeepAlive = true;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding)))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        #endregion

        #endregion
    }
}
