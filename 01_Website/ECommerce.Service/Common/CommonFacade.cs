using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.Common;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using ECommerce.Entity.Payment;
using System.Xml.Linq;

namespace ECommerce.Facade
{
    public class CommonFacade
    {
        #region 区域相关

        /// <summary>
        /// 获取所有的省份
        /// </summary>
        /// <param name="regionSysNo"></param>
        /// <returns></returns>
        public static List<Area> GetAllProvince()
        {
            return CommonDA.GetAllProvince();
        }

        /// <summary>
        /// 获取省份下所有的城市
        /// </summary>
        /// <param name="proviceSysNo"></param>
        /// <returns></returns>
        public static List<Area> GetAllCity(int proviceSysNo)
        {
            return CommonDA.GetAllCity(proviceSysNo);
        }

        /// <summary>
        /// 获取城市下所有的地区
        /// </summary>
        /// <returns></returns>
        public static List<Area> GetAllDistrict(int citySysNo)
        {
            return CommonDA.GetAllDistrict(citySysNo);
        }

        public static Area GetArea(int district)
        {
            return CommonDA.GetAreaBySysNo(district);
        }
        #endregion

        /// <summary>
        /// 创建短信
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool InsertNewSMS(SMSInfo item, out int sysNo)
        {
            item.IPAddress = GetIP();
            return CommonDA.InsertNewSMS(item, out  sysNo);
        }

        public static bool InsertNewSMS(SMSInfo item)
        {
            item.IPAddress = GetIP();
            return CommonDA.InsertNewSMS(item);
        }

        public static string GetIP()
        {
            HttpContext context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!String.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }


        /// <summary>
        /// 构造缓存的Key
        /// </summary>
        /// <param name="baseKey"></param>
        /// <param name="paramlist"></param>
        /// <returns></returns>
        public static string GenerateKey(string baseKey, params string[] paramlist)
        {
            string key = baseKey;
            if (paramlist != null && paramlist.Length > 0)
            {
                foreach (string param in paramlist)
                {
                    key += "_" + param;
                }
            }
            return key;
        }

        /// <summary>
        /// 构造缓存的Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseKey"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GenerateKey<T>(string baseKey, T param) where T : class
        {
            string key = baseKey;
            if (param != null)
            {
                return key;
            }
            string strParam = ECommerce.Utility.SerializationUtility.BinarySerialize(param);
            key += "_" + strParam;
            return key;
        }

        /// <summary>
        /// 根据PageTypeID获取页面类型
        /// </summary>
        /// <param name="sysNo">编号</param>
        /// <returns></returns>
        public static PageType GetPageType(int pageTypeID)
        {
            return CommonDA.GetPageType(pageTypeID);
        }

        public static int AddPoint(Point point)
        {
            point.LanguageCode = ConstValue.LanguageCode;
            point.CurrencyCode = ConstValue.CurrencySysNo.ToString();
            point.CompanyCode = ConstValue.CompanyCode;
            point.StoreCompanyCode = ConstValue.StoreCompanyCode;
            return CommonDA.AddPoint(point);
        }

        /// <summary>
        /// 根据Key获取前台配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSysConfigByKey(string key)
        {
            string cacheKey = "GetSysConfigByKey" + key;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (string)HttpRuntime.Cache[cacheKey];
            }
            string value = CommonDA.GetSysConfigByKey(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                HttpRuntime.Cache.Insert(cacheKey, value, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);
            }
            return value;
        }

        /// <summary>
        /// 屏蔽不允许在前台显示的词并用*代替
        /// </summary>
        /// <param name="originalWords"></param>
        /// <returns></returns>
        public static string SetCannotOnlineWordsMask(string originalWords, char replacement = '*')
        {
            if (string.IsNullOrWhiteSpace(originalWords))
            {
                return originalWords;
            }
            string cannotOnlineWords = CommonFacade.GetSysConfigByKey("CannotOnlineWords");
            if (!string.IsNullOrWhiteSpace(cannotOnlineWords))
            {
                Regex regex = new Regex(cannotOnlineWords.Replace(',', '|'), RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                return regex.Replace(originalWords, match => string.Empty.PadLeft(match.Length, replacement));
            }
            return originalWords;
        }

        public static PaymentSetting GetPaymentSettingInfoByID(int payTypeID)
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
                    if (payTypeID > 0)
                    {
                        if (item.Attribute("id").Value == payTypeID.ToString())
                        {
                            info.PaymentMode = GetPaymentMode(item);
                            break;
                        }
                    }

                }
            }
            return info;
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
            //paymentMode.MerchantName = GetElementValue(item, "merchantName");
            //paymentMode.MerchantNO = GetElementValue(item, "merchantNO");
            //paymentMode.MerchantCert = GetElementValue(item, "merchantCert");
            //paymentMode.MerchantCertKey = GetElementValue(item, "merchantCertKey");
            //paymentMode.CurCode = GetElementValue(item, "curCode");
            //paymentMode.Encoding = GetElementValue(item, "encoding");
            paymentMode.Debug = GetElementValue(item, "debug");
            //paymentMode.CustomProperty1 = GetElementValue(item, "customProperty1");
            //paymentMode.CustomProperty2 = GetElementValue(item, "customProperty2");
            //paymentMode.CustomProperty3 = GetElementValue(item, "customProperty3");
            //paymentMode.CustomProperty4 = GetElementValue(item, "customProperty4");
            //paymentMode.CustomProperty5 = GetElementValue(item, "customProperty5");

            return paymentMode;
        }
    }
}
