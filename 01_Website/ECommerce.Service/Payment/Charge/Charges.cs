using System;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.Specialized;

using ECommerce.Entity.Payment;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace ECommerce.Facade.Payment.Charge
{
    public abstract class Charges
    {
        public abstract void SetRequestForm(ChargeContext context);

        public abstract bool VerifySign(CallbackContext context);

        public abstract bool GetPayResult(CallbackContext context);

        public abstract int GetSOSysNo(CallbackContext context);

        public abstract decimal GetPayAmount(CallbackContext context);

        public virtual string GetSerialNumber(CallbackContext context)
        {
            return "";
        }

        public virtual string GetPayProcessTime(CallbackContext context)
        {
            return "";
        }
        /// <summary>
        /// 如果不是平台商家支付信息，则需要根据订单商家系统编号来重新加载支付相关信息
        /// </summary>
        /// <param name="context">支付上下文</param>
        public virtual void UpdateChargePayment(ChargeContext context)
        {
            //如果不是平台商家支付信息，则需要根据商家系统编号来重新加载支付相关信息
        }
        /// <summary>
        /// 如果不是平台商家支付信息，则需要根据订单商家系统编号来重新加载支付相关信息
        /// </summary>
        /// <param name="context">回调上下文</param>
        public virtual void UpdateCallbackPayment(CallbackContext context)
        {
            //如果不是平台商家支付信息，则需要根据商家系统编号来重新加载支付相关信息
        }

        #region 获取支付实例

        public static Charges GetInstance(ChargeContext context)
        {
            if (context != null && context.PaymentModeId > 0)
            {
                context.PaymentInfo = GetPaymentInfo(context.PaymentModeId);
                if (context.PaymentInfo != null && context.PaymentInfo.PaymentMode != null)
                {
                    return GetInstance(context.PaymentInfo.PaymentMode.ChargeProcessor);
                }
            }

            return null;
        }

        public static Charges GetInstance(CallbackContext context)
        {
            if (context != null && context.PaymentModeId > 0)
            {
                context.PaymentInfo = GetPaymentInfo(context.PaymentModeId);
                if (context.PaymentInfo != null && context.PaymentInfo.PaymentMode != null)
                {
                    return GetInstance(context.PaymentInfo.PaymentMode.ChargeProcessor);
                }
            }

            return null;
        }

        private static Charges GetInstance(string chargeProcessor)
        {
            if (!string.IsNullOrEmpty(chargeProcessor))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.CreateInstance(chargeProcessor) as Charges;
            }

            return null;
        }

        #endregion

        #region 获取一个支付上送请求数据

        /// <summary>
        /// 根据支付上下文获取一个支付上送请求数据
        /// </summary>
        /// <param name="context">支付上下文</param>
        /// <returns></returns>
        public string GetRequestContent(ChargeContext context)
        {
            if (context != null && context.PaymentModeId > 0)
            {
                if (context.RequestForm == null)
                {
                    context.RequestForm = new NameValueCollection();
                }
                SetRequestForm(context);

                return BuildRequestContent(context);
            }

            return string.Empty;
        }

        /// <summary>
        /// 构造上送请求数据
        /// </summary>
        /// <param name="context">支付上下文</param>
        /// <returns></returns>
        protected virtual string BuildRequestContent(ChargeContext context)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("<form id='chargeForm' name='chargeForm' method='{1}' action='{0}'>",
                     context.PaymentInfo.PaymentMode.PaymentUrl,
                     context.PaymentInfo.PaymentMode.RequestType);

            if (context.RequestForm != null && context.RequestForm.Count > 0)
            {
                foreach (string name in context.RequestForm)
                {
                    string value = context.RequestForm[name];
                    builder.AppendFormat("<input type='hidden' name='{0}' value='{1}' />"
                        , name, value);
                }
            }

            builder.Append("</form>");

            builder.Insert(0, string.Format(@"<html><head><meta http-equiv='pragma' content='no-cache'/><title>{0}</title></head><body>", "在线支付"));
            builder.AppendFormat("<center>正在提交支付数据，请稍候...</center>");
            builder.AppendFormat("<script language='javascript'>");
            builder.AppendFormat("var theForm = document.forms['chargeForm'];");
            builder.AppendFormat("if (!theForm) theForm = document.chargeForm;");
            builder.AppendFormat("theForm.submit();");
            builder.AppendFormat("</script>");
            builder.AppendFormat("</body></html>");

            return builder.ToString();
        }

        #endregion

        #region 从配置文件获取配置信息

        /// <summary>
        /// 根据id获取PaymentInfo
        /// </summary>
        /// <param name="paymentModeId">id</param>
        /// <returns></returns>
        public static PaymentSetting GetPaymentInfo(int paymentModeId)
        {
            return GetPaymentInfo(paymentModeId, null);
        }

        /// <summary>
        /// 根据商户编号获取PaymentInfo
        /// </summary>
        /// <param name="merchantNO">商户编号</param>
        /// <returns></returns>
        public static PaymentSetting GetPaymentInfoByMerchantNo(string merchantNO)
        {
            return GetPaymentInfo(0, merchantNO);
        }

        /// <summary>
        /// 根据id或者商品编号获取PaymentInfo
        /// </summary>
        /// <param name="paymentModeId">id</param>
        /// <param name="merchantNO">商户编号</param>
        /// <returns></returns>
        private static PaymentSetting GetPaymentInfo(int paymentModeId, string merchantNO)
        {
            PaymentSetting info = new PaymentSetting();
            info.PaymentBase = new PaymentBase();
            XDocument doc = XDocument.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/Payment.config"));
            if (null != doc)
            {
                XElement root = doc.Root;
                XElement paymentBase = root.Element("paymentBase");
                info.PaymentBase.BaseUrl = GetElementValue(paymentBase, "baseUrl");

                XElement paymentModes = root.Element("paymentModes");
                foreach (XElement item in paymentModes.Elements("paymentMode"))
                {
                    if (paymentModeId > 0)
                    {
                        if (item.Attribute("id").Value == paymentModeId.ToString())
                        {
                            info.PaymentMode = GetPaymentMode(item);
                            break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(merchantNO))
                    {
                        string merchantId = GetElementValue(item, "merchantNO");
                        if (merchantNO == merchantId)
                        {
                            info.PaymentMode = GetPaymentMode(item);
                            break;
                        }
                    }
                }
            }

            if (info.PaymentMode == null)
            {
                info.PaymentMode = new PaymentMode();
            }

            return info;
        }

        /// <summary>
        /// 根据节点获取节点配置信息
        /// </summary>
        /// <param name="item">节点</param>
        /// <returns></returns>
        private static PaymentMode GetPaymentMode(XElement item)
        {
            PaymentMode paymentMode = new PaymentMode();

            paymentMode.Id = item.Attribute("id").Value;
            paymentMode.Name = item.Attribute("name").Value;
            paymentMode.RequestType = GetElementValue(item, "requestType");
            paymentMode.ChargeProcessor = GetElementValue(item, "chargeProcessor");
            paymentMode.PaymentUrl = GetElementValue(item, "paymentUrl");
            paymentMode.PaymentBgCallbackUrl = GetElementValue(item, "paymentBgCallbackUrl");
            paymentMode.PaymentCallbackUrl = GetElementValue(item, "paymentCallbackUrl");
            paymentMode.RefundUrl = GetElementValue(item, "refundUrl");
            paymentMode.RefundCallbackUrl = GetElementValue(item, "refundCallbackUrl");
            paymentMode.BankCert = GetElementValue(item, "bankCert");
            paymentMode.BankCertKey = GetElementValue(item, "bankCertKey");

            paymentMode.CustomConfigs = new Dictionary<string, string>();
            foreach (XElement customNode in item.Element("customConfigs").Elements("customConfig"))
            {
                paymentMode.CustomConfigs[customNode.Attribute("name").Value] = customNode.Value;
            }

            paymentMode.Debug = GetElementValue(item, "debug");

            paymentMode.MerchantList = new List<PaymentModeMerchant>();
            foreach (XElement node in item.Element("merchants").Elements("merchant"))
            {
                PaymentModeMerchant merchant = new PaymentModeMerchant();
                merchant.MerchantSysNo = int.Parse(GetElementValue(node, "merchantSysNo"));
                merchant.MerchantNO = GetElementValue(node, "merchantNO");
                merchant.MerchantCert = GetElementValue(node, "merchantCert");
                merchant.MerchantCertKey = GetElementValue(node, "merchantCertKey");
                merchant.CurCode = GetElementValue(node, "curCode");
                merchant.Encoding = GetElementValue(node, "encoding");
                merchant.CustomConfigs = new Dictionary<string, string>();
                foreach (XElement customNode in node.Element("customConfigs").Elements("customConfig"))
                {
                    merchant.CustomConfigs[customNode.Attribute("name").Value] = customNode.Value;
                }
                paymentMode.MerchantList.Add(merchant);
            }

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
        /// 字符串转换为整型
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public int ConvertStringToInt(string value)
        {
            int num;
            if (int.TryParse(value, out num))
            {
                return num;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 字符串转换为小数
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public decimal ConvertStringToDecimal(string value)
        {
            decimal num;
            if (decimal.TryParse(value, out num))
            {
                return num;
            }
            else
            {
                return 0m;
            }
        }

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

            return result;
        }

        /// <summary>
        /// 进行base64编码
        /// </summary>
        /// <param name="data">被编码数据</param>
        /// <returns></returns>
        public static string Base64Encode(string data)
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
        public static string Base64Decode(string data)
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
