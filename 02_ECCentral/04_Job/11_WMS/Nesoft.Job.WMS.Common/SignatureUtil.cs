using Nesoft.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Nesoft.Job.WMS.Common
{
    public class SignatureUtil
    {
        /// <summary>
        /// 跨境通接口 - 生成签名方法 (MD5方式)
        /// *************************************************************************************
        /// 
        /// 签名方法 :
        ///step1: 拼接参数字符串
        ///将除sign以外的请求参数（包括标准参数，除非有特别说明）按照参数名称的字典升序排列，然后按此顺序，将”参数名＝参数值”用”&”符号连接，结果形如：”参数名1＝参数值1&参数名2＝参数值2&…&参数名n＝参数值n”。
        ///注意事项：
        ///1)	参数值应为urlencode过后的字符串。
        ///2)	仅对接口定义中声明且请求参数列表中包含的参数（包括空值）进行签名。
        ///3)	参数值不作去除空格。
        ///step2: 计算参数字符串&appSecret的hash摘要
        ///digest=md5(参数字符串&appSecret)
        ///step3: 将二进制的摘要转换为16进制表示
        ///sign=toHex(digest)，注：签名比较无需区分大小写
        /// *************************************************************************************
        /// </summary>
        /// <param name="requestBody">post请求的Body（所有post参数的值需要做UrlEncode处理）</param>
        /// <param name="secretKey">ScretKey,建议做成配置项</param>
        /// <returns>返回生成的签名</returns>
        public static string Build(string requestBody, string secretKey)
        {
            NameValueCollection nv = new NameValueCollection();
            if (requestBody.Contains('&'))
            {
                foreach (var item in requestBody.Split('&'))
                {
                    nv.Add(item.Split('=')[0], item.Split('=')[1]);
                }
            }
            else
            {
                nv.Add(requestBody.Split('=')[0], requestBody.Split('=')[1]);
            }
            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();
            foreach (string key in nv.AllKeys)
            {
                string value = nv[key];
                value = HttpUtility.UrlDecode(nv[key], Encoding.UTF8);
                paramList.Add(new KeyValuePair<string, string>(key, value));
            }


            #region Sign
            string logContent = string.Empty;
            byte[] bytes = null;
            //Step 2:计算参数字符串&appSecret的hash摘要
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                string data = string.Concat(PopulateSignData(paramList), "&", secretKey);
                logContent = "Sign Before:" + data;
                Logger.WriteLog(logContent, "Sign");
                Debug.WriteLine(logContent);
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
            //Step 3:将二进制的摘要转换为16进制表示
            StringBuilder sb = new StringBuilder();
            foreach (byte _byte in bytes)
            {
                string hex = _byte.ToString("x");
                if (hex.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(hex);
            }
            logContent = "Sign After:" + sb.ToString().ToLower();
            Logger.WriteLog(logContent, "Sign");
            Debug.WriteLine(logContent);
            return sb.ToString().ToLower();
            #endregion

        }

        /// <summary>
        /// 私有方法，处理post参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static string PopulateSignData(List<KeyValuePair<string, string>> parameter)
        {
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>();
            foreach (var param in parameter)
            {
                if (param.Key.ToLower() != "sign"
                    && param.Key != "_"
                    && !string.IsNullOrEmpty(param.Key)
                    && !string.IsNullOrEmpty(param.Value)
                    )
                {
                    if (param.Key.ToLower() == "callback" && param.Value.ToLower() == "jsonpcallback")
                    {
                        continue;
                    }
                    sortedParams.Add(param.Key, param.Value);
                }
            }

            StringBuilder sb = new StringBuilder();

            IEnumerator<KeyValuePair<string, string>> enumerator = sortedParams.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = enumerator.Current.Key;
                string value = enumerator.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    sb.Append(key).Append("=").Append(UrlEncode(value)).Append("&");
                }
            }
            return string.IsNullOrEmpty(sb.ToString()) ? sb.ToString() : sb.ToString().TrimEnd('&');
        }

        private static Regex REG_URL_ENCODING = new Regex(@"%[a-f0-9]{2}");
        private static string UrlEncode(string str)
        {
            var encodeStr = HttpUtility.UrlEncode(str, Encoding.UTF8).Replace("+", "%20").Replace("*", "%2A").Replace("(", "%28").Replace(")", "%29");
            return REG_URL_ENCODING.Replace(encodeStr, m => m.Value.ToUpperInvariant());
        }
    }
}
