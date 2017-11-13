using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using ECommerce.Utility;

namespace ECommerce.Entity
{
    public class ConstValue
    {
        public static string CompanyCode = "8601";
        public static string LanguageCode = "zh-CN";
        public static string StoreCompanyCode = "8601";
        public static string FileBaseUrl = ConfigurationSettings.AppSettings["FileBaseUrl"];
        public static string FileBaseUrlSSL = ConfigurationSettings.AppSettings["FileBaseUrlSSL"];

        public static string FormatDateTimeLong = "yyyy-MM-dd hh:mm:ss";
        public static string FormatDateTimeShort = "yyyy-MM-dd";
        public static string ChannelID = ConfigurationSettings.AppSettings["ChannelID"];
        public static bool HaveSSLWebsite = ConfigurationSettings.AppSettings["HaveSSLWebsite"].ToString().ToLower() == "true" ? true : false;
        public static string SSLWebsiteHost = ConfigurationSettings.AppSettings["SSLWebsiteHost"];
        public static List<string> SSLControllers = ConfigurationSettings.AppSettings["SSLControllers"].ToString().ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();

        public static string WebDomain = ConfigurationSettings.AppSettings["WebDomain"];

        public static string WebDomainOnlyHost = ConfigurationSettings.AppSettings["WebDomainOnlyHost"];

        public static string CDNWebDomain = ConfigurationSettings.AppSettings["CDNWebDomain"];
        public static string ImageServerHost = ConfigurationSettings.AppSettings["ImageServerHost"];
        public static string ImageServerHostSSL = ConfigurationSettings.AppSettings["ImageServerHostSSL"];
        public static string Category1IconHost = ConfigurationSettings.AppSettings["Category1IconHost"];
        //积分兑换比例
        public static string PointExhangeRate = AppSettingManager.GetSetting("Product", "Product_PointExhangeRate");
        //网银积分兑换比例
        public static string BankPointExhangeRate = AppSettingManager.GetSetting("Product", "Product_BankPointExhangeRate");

        public static string UserSysNo = ConfigurationSettings.AppSettings["UserSysNo"];
        public static string UserAcct = ConfigurationSettings.AppSettings["UserAcct"];
        public static string ECCServiceBaseUrl = ConfigurationSettings.AppSettings["ECC_Service_Base_Url"];
        public static string CurrencySysNo = ConfigurationSettings.AppSettings["CurrencySysNo"];
        //拆单限制门槛金额配置
        public static string MaxPerOrderAmount = ConfigurationSettings.AppSettings["MaxPerOrderAmount"];
        
        //构建类别dimension nvalue 的基数
        public static int SINGLE_SUBCATEGORY_DMSID_SEED = Convert.ToInt32(AppSettingManager.GetSetting("Product", "Product_SINGLE_SUBCATEGORY_DMSID_SEED"));
        //构建brand nvalue的基数
        public static  int SINGLE_BRAND_STORE_DMSID_SEED = Convert.ToInt32(AppSettingManager.GetSetting("Product", "Product_SINGLE_BRAND_STORE_DMSID_SEED"));
        //构建country nvalue的基数
        public static  int SINGLE_COUNTYR_DMSID_SEED = Convert.ToInt32(AppSettingManager.GetSetting("Product", "Product_SINGLE_COUNTYR_DMSID_SEED"));
        //构建商家类别 nvalue的基数
        public static int Product_SINGLE_STORECATE_DMSID_SEED = Convert.ToInt32(AppSettingManager.GetSetting("Product", "Product_SINGLE_STORECATE_DMSID_SEED"));
        public static  int ProductWarnInventory = Convert.ToInt32(AppSettingManager.GetSetting("Product", "Product_ProductWarnInventory"));

        public static string Cookie_Name_VoidedOrder = "Voided_Order_SysNos";

        /// <summary>
        /// 关税阈值，小于此值，关税可省掉
        /// </summary>
        public static int TariffFreeLimit = 50;

        /// <summary>
        /// 验证手机送积分
        /// </summary>
        public static int GetPointByValidatePhone = 10;
        /// <summary>
        /// 验证邮箱送积分
        /// </summary>
        public static int GetPointByValidateEmail = 10;

        public static string PointAccountSysNo = ConfigurationSettings.AppSettings["PointAccountSysNo"];

        /// <summary>
        /// 以备货时间拆单，差多少天（含）以内不拆单
        /// </summary>
        public static string SplitLeadTimeDiff = AppSettingManager.GetSetting("Shopping", "Shopping_SplitLeadTimeDiff");

        /// <summary>
        /// 礼品卡C3
        /// </summary>
        public static int GiftCardCategory3 = int.Parse(ConfigurationSettings.AppSettings["GiftCardCategory3"]);

        /// <summary>
        /// 平台默认商家编号
        /// </summary>
        public static int PlatformMerchantSysNo = int.Parse(ConfigurationSettings.AppSettings["PlatformMerchantSysNo"]);
        /// <summary>
        /// 静态资源版本号
        /// </summary>
        public static string ResourcesVersion = ConfigurationSettings.AppSettings["ResourcesVersion"];

        /// <summary>
        /// 登录验证码类别(0:图形验证码 1:手机短信在线验证)
        /// </summary>
        public static string LoginValidCodeType = ConfigurationSettings.AppSettings["LoginValidCodeType"];

        /// <summary>
        /// 没有登陆用户查看商品默认显示的配送地址
        /// </summary>
        public static int DefaultRegion = int.Parse(ConfigurationSettings.AppSettings["DefaultRegion"]);

        /// <summary>
        /// 商品咨询直接展示开关【true或false】
        /// </summary>
        public static bool ProductConsultSwitch = ConfigurationSettings.AppSettings["ProductConsultSwitch"].ToString().ToLower() == "true" ? true : false;
        /// <summary>
        /// 撮合交易直接展示开关【true或false】
        /// </summary>
        public static bool ProductMatchedTradingSwitch = ConfigurationSettings.AppSettings["ProductMatchedTradingSwitch"].ToString().ToLower() == "true" ? true : false;
        /// <summary>
        /// 限时抢购提前显示时间设置【单位分钟】
        /// </summary>
        public static int LimitBuyEarlyShowTimeSetting = int.Parse(ConfigurationSettings.AppSettings["LimitBuyEarlyShowTimeSetting"]);

        /// <summary>
        /// 促销活动里的商品是否支持付款后扣减在线库存【支持：true或不支持：false】
        /// </summary>
        public static bool PaymentInventory = ConfigurationSettings.AppSettings["PaymentInventory"].ToString().ToLower() == "true" ? true : false;

    }

    public  struct CacheTime
    {
        /// <summary>
        /// 非常短的时间，60秒
        /// </summary>
        public static int Shortest = 60;

        /// <summary>
        /// 短时间，300秒
        /// </summary>
        public static int Short = 300;

        /// <summary>
        /// 中等时间，600秒
        /// </summary>
        public static int Middle = 600;

        /// <summary>
        /// 长时间，900秒
        /// </summary>
        public static int Long = 900;

        /// <summary>
        /// 更长时间，1800秒
        /// </summary>
        public static int Longer = 1800;

        /// <summary>
        /// 最长时间，3600秒
        /// </summary>
        public static int Longest = 3600;
    }


}
