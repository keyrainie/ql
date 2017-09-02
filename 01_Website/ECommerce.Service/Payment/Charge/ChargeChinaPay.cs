using ECommerce.Entity.Payment;
using SolrNet.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ECommerce.Facade.Payment.Charge
{
    public class ChargeChinaPay : Charges
    {
        public override void SetRequestForm(Entity.Payment.ChargeContext context)
        {
            //以下信息非特殊情况不需要改动
            context.RequestForm["version"] = context.PaymentInfoMerchant.CustomConfigs["version"];//版本号
            context.RequestForm["encoding"] = context.PaymentInfoMerchant.Encoding;//编码方式
            context.RequestForm["txnType"] = context.PaymentInfoMerchant.CustomConfigs["txnType"];//交易类型
            context.RequestForm["txnSubType"] = context.PaymentInfoMerchant.CustomConfigs["txnSubType"];//交易子类
            context.RequestForm["bizType"] = context.PaymentInfoMerchant.CustomConfigs["bizType"];//业务类型
            context.RequestForm["signMethod"] = context.PaymentInfoMerchant.CustomConfigs["signMethod"];//签名方法
            context.RequestForm["channelType"] = context.PaymentInfoMerchant.CustomConfigs["channelType"];//渠道类型
            context.RequestForm["accessType"] = context.PaymentInfoMerchant.CustomConfigs["accessType"];//接入类型
            context.RequestForm["frontUrl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentCallbackUrl);  //前台通知地址      
            context.RequestForm["backUrl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);  //后台通知地址
            context.RequestForm["currencyCode"] = context.PaymentInfoMerchant.CurCode;//交易币种
            context.RequestForm["merId"] = context.PaymentInfoMerchant.MerchantNO;//商户号，请改自己的测试商户号，此处默认取demo演示页面传递的参数
            context.RequestForm["orderId"] = context.SOInfo.SoSysNo.ToString();//商户订单号，8-32位数字字母，不能含“-”或“_”，此处默认取demo演示页面传递的参数，可以自行定制规则
            context.RequestForm["txnTime"] = context.SOInfo.OrderDate.ToString("yyyyMMddHHmmss");//订单发送时间，格式为YYYYMMDDhhmmss，取北京时间，此处默认取demo演示页面传递的参数，参考取法： DateTime.Now.ToString("yyyyMMddHHmmss")
            context.RequestForm["txnAmt"] = context.SOInfo.RealPayAmt.ToString("F0");//交易金额，单位分，此处默认取demo演示页面传递的参数
            
            SignData(context);

        }
        /// <summary>
        /// Signs the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private void SignData(ChargeContext context)
        {
            string certPath = context.PaymentInfoMerchant.MerchantCert;
            string certPwd = context.PaymentInfoMerchant.MerchantCertKey;
            
            context.RequestForm["certId"] = CertUtil.GetSignCertId(certPath, certPwd);

            //将Dictionary信息转换成key1=value1&key2=value2的形式
            string stringData = SDKUtil.CreateLinkString(context.RequestForm, true, false);
            //log.Info("待签名排序串：[" + stringData + "]");

            string stringSign = null;

            byte[] signDigest = SecurityUtil.Sha1X16(stringData, Encoding.UTF8);

            string stringSignDigest = BitConverter.ToString(signDigest).Replace("-", "").ToLower();
            //log.Info("sha1结果：[" + stringSignDigest + "]");

            byte[] byteSign = SecurityUtil.SignBySoft(CertUtil.GetSignProviderFromPfx(certPath, certPwd), Encoding.UTF8.GetBytes(stringSignDigest));

            stringSign = Convert.ToBase64String(byteSign);
            //log.Info("签名结果：[" + stringSign + "]");

            //设置签名域值
            context.RequestForm["signature"] = stringSign;
        }

        #region
        /// <summary>
        /// Verifies the sign.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool VerifySign(CallbackContext context)
        {
            //获取签名
            string signValue = context.ResponseForm["signature"];
            byte[] signByte = Convert.FromBase64String(signValue);
            
            //context.ResponseForm.Remove("signature");

            NameValueCollection collection = new NameValueCollection();

            foreach (string s in context.ResponseForm.AllKeys)
            {
                if (s != "signature")
                {
                    collection.Add(s,context.ResponseForm.Get(s));
                }
            }
            string stringData = SDKUtil.CreateLinkString(collection, true, false);
            byte[] signDigest = SecurityUtil.Sha1X16(stringData, Encoding.UTF8);
            string stringSignDigest = BitConverter.ToString(signDigest).Replace("-", "").ToLower();
            string encryptCertPath=context.PaymentInfoMerchant.CustomConfigs["encryptCert"];
            RSACryptoServiceProvider provider = CertUtil.GetValidateProviderFromPath(context.ResponseForm["certId"], encryptCertPath);
            if (null == provider)
            {
                return false;
            }
            bool result = SecurityUtil.ValidateBySoft(provider, signByte, Encoding.UTF8.GetBytes(stringSignDigest));
            return result;
        }

        /// <summary>
        /// Gets the pay result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool GetPayResult(CallbackContext context)
        {
            string respcode=context.ResponseForm["respcode"];
            if (respcode.Equals("00"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the so system no.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override int GetSOSysNo(CallbackContext context)
        {
            return int.Parse(context.ResponseForm["orderId"]);
        }

        /// <summary>
        /// Gets the pay amount.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override decimal GetPayAmount(CallbackContext context)
        {
            return decimal.Parse(context.ResponseForm["txnAmt"])/100;
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GetSerialNumber(CallbackContext context)
        {
            return context.ResponseForm["queryId"];
        }

        /// <summary>
        /// Gets the pay process time.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GetPayProcessTime(CallbackContext context)
        {
            return base.GetPayProcessTime(context);
        }
        #endregion
    }

    public class Cert
    {
        public X509Certificate2 cert;
        public string certId;
        public RSACryptoServiceProvider key;
    }

    //public class SDKConfig
    //{
    //    private static string validateCertDir = @"D:\\certs\\";//功能：读取配置文件获取验签目录
    //    public static string ValidateCertDir
    //    {
    //        get { return SDKConfig.validateCertDir; }
    //        set { SDKConfig.validateCertDir = value; }
    //    }
    //}

    public class CertUtil
    {
        private static Dictionary<string, Cert> signCerts = new Dictionary<string,Cert>();
        private static Dictionary<string, Cert> cerCerts = new Dictionary<string, Cert>();
        private static void initSignCert(string certPath, string certPwd)
        {

            Cert signCert = new Cert();
            signCert.cert = new X509Certificate2(certPath, certPwd, X509KeyStorageFlags.MachineKeySet);
            signCert.key = (RSACryptoServiceProvider)signCert.cert.PrivateKey;
            signCert.certId = BigInteger.Parse(signCert.cert.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
            //privateKeyCert.certId = BigNum.ToDecimalStr(BigNum.ConvertFromHex(privateKeyCert.cert.SerialNumber)); 低于4.0版本的.NET请使用此方法
            signCerts[certPath] = signCert;
        }
        private static void initCerCerts(string encryptCert)
        {
            DirectoryInfo directory = new DirectoryInfo(encryptCert);
            FileInfo[] files = directory.GetFiles("*.cer");
            if (null == files || 0 == files.Length)
            {
                return;
            }
            foreach (FileInfo file in files)
            {
                Cert cert = new Cert();
                cert.cert = new X509Certificate2(file.DirectoryName + "\\" + file.Name);
                //cert.certId = BigNum.ToDecimalStr(BigNum.ConvertFromHex(cert.cert.SerialNumber)); 低于4.0版本的.NET请使用此方法
                cert.certId = BigInteger.Parse(cert.cert.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                cert.key = (RSACryptoServiceProvider)cert.cert.PublicKey.Key;
                cerCerts[cert.certId] = cert;
            }
        }
        /// <summary>
        /// 获取签名证书的证书序列号
        /// </summary>
        /// <returns></returns>
        public static string GetSignCertId(string certPath, string certPwd)
        {
            if (!signCerts.ContainsKey(certPath))
            {
                initSignCert(certPath, certPwd);
            }
            return signCerts[certPath].certId;
        }
        /// <summary>
        /// 获取签名证书私钥
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetSignProviderFromPfx(string certPath, string certPwd)
        {
            if (!signCerts.ContainsKey(certPath))
            {
                initSignCert(certPath, certPwd);
            }
            return signCerts[certPath].key;
        }

        /// <summary>
        /// 通过证书id，获取验证签名的证书
        /// </summary>
        /// <param name="certId"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetValidateProviderFromPath(string certId, string encryptCert)
        {
            if (cerCerts == null || cerCerts.Count <= 0)
            {
                initCerCerts(encryptCert);
            }
            if (cerCerts == null || cerCerts.Count <= 0)
            {
                return null;
            }
            if (cerCerts.ContainsKey(certId))
            {
                return cerCerts[certId].key;
            }
            else
            {
                return null;
            }
        }
    }

    public class SDKUtil
    {
        /// <summary>
        /// 把请求要素按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="para">请求要素</param>
        /// <param name="sort">是否需要根据key值作升序排列</param>
        /// <param name="encode">是否需要URL编码</param>
        /// <returns>拼接成的字符串</returns>
        public static String CreateLinkString(NameValueCollection para, bool sort, bool encode)
        {
            if (para == null || para.Count == 0)
                return "";
            List<String> list = new List<String>(para.AllKeys);

            if (sort)
                list.Sort(StringComparer.Ordinal);

            StringBuilder sb = new StringBuilder();
            foreach (String key in list)
            {
                String value = para[key];
                if (encode && value != null)
                {
                    try
                    {
                        value = HttpUtility.UrlEncode(value);
                    }
                    catch (Exception ex)
                    {
                        //LogError(ex);
                        return "#ERROR: HttpUtility.UrlEncode Error!" + ex.Message;
                    }
                }

                sb.Append(key).Append("=").Append(value).Append("&");

            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }
    }

    public static class SecurityUtil
    {
        public static readonly string ALGORITHM_SHA1 = "SHA1";

        /// <summary>
        /// 摘要计算
        /// </summary>
        /// <param name="dataStr"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Sha1X16(string dataStr, Encoding encoding)
        {
            try
            {
                byte[] data = encoding.GetBytes(dataStr);
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] sha1Res = sha1.ComputeHash(data, 0, data.Length);
                return sha1Res;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// 软签名
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SignBySoft(RSACryptoServiceProvider provider, byte[] data)
        {
            byte[] res = null;
            try
            {
                HashAlgorithm hashalg = new SHA1CryptoServiceProvider();
                res = provider.SignData(data, hashalg);
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="base64DecodingSignStr"></param>
        /// <param name="srcByte"></param>
        /// <returns></returns>
        public static bool ValidateBySoft(RSACryptoServiceProvider provider, byte[] base64DecodingSignStr, byte[] srcByte)
        {
            HashAlgorithm hashalg = new SHA1CryptoServiceProvider();
            return provider.VerifyData(srcByte, hashalg, base64DecodingSignStr);
        }












    }
}
