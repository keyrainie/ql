using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer
{
    public static class AppSettingHelper
    {
        private const string DomainName = "ThirdPart";

        #region 联通相关配置
        public static string UnicomURL
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "UnicomURL");
            }
        }

        public static string UnicomAccountID
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "UnicomAccountID");
            }
        }
        public static string UnicomPassword
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "UnicomPassword");
            }
        }
        public static string UnicomAgentKey
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "UnicomAgentKey");
            }
        }
        #endregion

        #region 神州积分联盟相关配置
        public static string SZPointAlliance_RefundPrepaidCard
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_RefundPrepaidCard");
            }
        }

        public static string SZPointAlliance_MerchantID
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_MerchantID");
            }
        }

        public static string SZPointAlliance_Key
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_Key");
            }
        }

        public static string SZPointAlliance_ChannelType
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_ChannelType");
            }
        }

        public static bool SZPointAlliance_IsUseProxy
        {
            get
            {
                string value = AppSettingManager.GetSetting(DomainName, "SZPointAlliance_IsUseProxy");
                bool tmp = bool.TryParse(value, out tmp) ? tmp : false;
                return tmp;
            }
        }

        public static string SZPointAlliance_ProxyUrl
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_ProxyUrl");
            }
        }

        public static string SZPointAlliance_ProxyUserID
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_ProxyUserID");
            }
        }

        public static string SZPointAlliance_ProxyPassword
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "SZPointAlliance_ProxyPassword");
            }
        }

        #endregion

        #region
        public static bool IsUseProxy
        {
            get
            {
                string value = AppSettingManager.GetSetting(DomainName, "IsUseProxy");
                bool t = bool.TryParse(value, out t) ? t : false;
                return t;
            }
        }
        public static string ProxyUrl
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "ProxyUrl");
            }
        }
        public static string ProxyUserID
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "ProxyUserID");
            }
        }
        public static string ProxyPassword
        {
            get
            {
                return AppSettingManager.GetSetting(DomainName, "ProxyPassword");
            }
        }
        #endregion
    }
}