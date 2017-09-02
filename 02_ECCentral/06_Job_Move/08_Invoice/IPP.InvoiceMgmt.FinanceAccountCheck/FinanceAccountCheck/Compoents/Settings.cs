using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


    /// <summary>
    /// Configuration Settings class for this App.
    /// </summary>
    public sealed class Settings
    {
        #region Constructors

        private Settings()
        {
        }

        #endregion      

        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }

        /// <summary>
        /// LogFileName
        /// </summary>
        public static string LogFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFileName"];
            }
        }       

        /// <summary>
        /// 支付接口
        /// </summary>
        public static string GateWay
        {
            get
            {
                return ConfigurationManager.AppSettings["gateway"];
            }
        }

        /// <summary>
        /// 服务名称，这个是识别是何接口实现何功能的标识，请勿修改
        /// </summary>
        public static string Service
        {
            get
            {
                return ConfigurationManager.AppSettings["service"];
            }
        }

        /// <summary>
        /// 加密类型,签名方式“不用改”
        /// </summary>
        public static string SignType
        {
            get
            {
                return ConfigurationManager.AppSettings["sign_type"];
            }
        }

        /// <summary>
        /// 安全校验码，与partner是一组，获取方式是：用签约时支付宝帐号登陆支付宝网站www.alipay.com，在商家服务我的商家里即可查到。
        /// </summary>
        public static string Key
        {
            get
            {
                return ConfigurationManager.AppSettings["key"];
            }
        }

        /// <summary>
        /// 安全校验码，与partner是一组，获取方式是：用签约时支付宝帐号登陆支付宝网站www.alipay.com，在商家服务我的商家里即可查到。
        /// </summary>
        public static string KeyAdd
        {
            get
            {
                return ConfigurationManager.AppSettings["KeyAdd"];
            }
        }

        /// <summary>
        /// 商户ID，合作身份者ID，合作伙伴ID
        /// </summary>
        public static string Partner
        {
            get
            {
                return ConfigurationManager.AppSettings["partner"];
            }
        }

        /// <summary>
        /// 支持多合作伙伴
        /// </summary>
        public static string partnerAdd
        {
            get
            {
                return ConfigurationManager.AppSettings["partnerAdd"];
            }
        }

        /// <summary>
        /// 编码类型，完全根据客户自身的项目的编码格式而定，千万不要填错。否则极其容易造成MD5加密错误。
        /// </summary>
        public static string InputCharset
        {
            get
            {
                return ConfigurationManager.AppSettings["_input_charset"];
            }
        }

        /// <summary>
        /// 交易代码，可以传递多个交易代码，中间用逗号分开，可选填
        /// </summary>
        /*代码类型枚举
           3001    转账
           3011    转账
           3012    充值
           4002    提现
           4003    充值
           6001    在线支付
           6002    在线支付
           */
        public static string TransCode
        {
            get
            {
                return ConfigurationManager.AppSettings["trans_code"];
            }
        }

        /// <summary>
        /// 预设开始时间点
        /// </summary>
        public static DateTime DefaultDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(ConfigurationManager.AppSettings["DefaultDate"]);
                }
                catch
                {
                    throw new Exception("预设日期格式有误");
                }
            }
        }

        public static string EmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailAddress"];
            }
        }

        public static string LongOrderSysNoAlert
        {
            get
            {
                return ConfigurationManager.AppSettings["LongOrderSysNoAlert"];
            }
        }

        public static int TryPeriod
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["TryPeriod"]);
            }
        }
    }

