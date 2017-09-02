using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using Nesoft.ECWeb.MobileService.Models.Common;

namespace Nesoft.ECWeb.MobileService.Models.AliPay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        #region 字段
        private static string partner = "";
        private static string private_key = "";
        private static string public_key = "";
        private static string input_charset = "";
        private static string sign_type = "";
        //本系统支付宝系统编号112不能修改
        private static int payTypeSysNo = 112;
        #endregion
        
        static Config()
        {
            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            //partner = "2088021873407824";
            partner = "";
            //商户的私钥
            //private_key = @"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAMki/YrufoakXeOJSHY+adnnkKqbEM5xYSqv5hSAxqOMl82y+XU8K0tFpqElgQyYMUph7dCojGifTfHbcaOh5fqpgb/ixE2EQjRCI+c6wAHE0Kso5w/pBPHGHLsDRcIFbldWpTu1vM9oSMO5Il8nXtmnvYl29syYc6TzD/Ltyb31AgMBAAECgYEAgZQePZwqPTm1FvqBiVPqQ6zQYWjm6ejXTXXmxzyzO/g+cBGmbmWmCqdDkzujKOb/kkkGuc9sLZa7012pl16c1rrOVpKdjWE4tQZ3B/Ff8xnxJCgD8fq9bAjeZsQo0sxth/19N2vxxdr6KwLrxlh+BH5xRET/BJtllyw7jMK322ECQQD8JOcoOrQiLAM3Evbxt8l4Ixuhr1jRlv2Wsymjwr65x95MXgl/21JIPzHb32dlYql+LM+i4NztiVPsoHwcJiYpAkEAzDZnG673rnW8cC8dBKHeMvSx8VZlTI9gVcpmyXJeiPd+nkhnPUn91UIWEAf7aY4lEE0GQuEVo8ibYw30Il5a7QJAD++NJH/BTr+VXG+4Z8KD2zHs1yUr6eMvF80u/oiaYUd+hzkSRKBp7OMBlbHi6Qd5St4uKU8o52JciYIDFgxbMQJAGcDupe4d7FGKzzcR2Mi1JgX3/vCfW9VlF8yrw9H5nBpiTIHjCzaunVt1PtZS3ZAAmpqsJHkQapRdDFPxKvhtTQJAcBEM7HPH+m4YFPEhU2TWotqRtEmmazJPiCcMdtIJB08gd/LkRxO6yog2QiQfa4gvk8K+g6c7UAJ9BhZQItZwTA==";
            private_key = @"";
            //支付宝的公钥，无需修改该值
            //public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";
            public_key = @"";
            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

            //字符编码格式 目前支持 gbk 或 utf-8
            input_charset = "utf-8";

            //签名方式，选择项：RSA、DSA、MD5
            sign_type = "RSA";
        }
        
        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return CommonManager.GetPaymentSectionInfo(payTypeSysNo).PartnerID; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public static string Private_key
        {
            get { return CommonManager.GetPaymentSectionInfo(payTypeSysNo).AsrPrivateKey; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public static string Public_key
        {
            get { return CommonManager.GetPaymentSectionInfo(payTypeSysNo).AsrPublicKey; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return sign_type; }
        }
        #endregion
    }
}