using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.MobileService.Models.App;
using Nesoft.Utility;
using Nesoft.ECWeb.MobileService.Models.Payment;

namespace Nesoft.ECWeb.MobileService.AppCode
{
    /// <summary>
    /// 系统相关配置，如Banner的PageType,Position编号等配置
    /// </summary>
    public class AppSettings
    {
        public static MobileAppConfig GetCachedConfig()
        {
            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/MobileApp.config");
            return CacheManager.ReadXmlFileWithLocalCache<MobileAppConfig>(configPath);
        }

        public static MobilePaySections GetMobilePaySections()
        {
            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/MobilePaySections.config");
            return CacheManager.ReadXmlFileWithLocalCache<MobilePaySections>(configPath);
        }
    }
}