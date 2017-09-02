using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class AppSettingHelper
    {
        private static readonly string relativeUrl = "/AppSetting/GetAppSetting";
        private static readonly string relativeBatchUrl = "/AppSetting/GetAllAppSettings/{0}";

        public static void GetSetting(string domainName, string key, EventHandler<RestClientEventArgs<string>> callBack)
        {
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/UtilityService");
            AppSettingQueryFilter filter = new AppSettingQueryFilter()
            {
                DomainName = domainName,
                Key = key
            };
            restClient.Query<string>(relativeUrl, filter, callBack);
        }

        public static void GetAllSettings(string domainName, EventHandler<RestClientEventArgs<Dictionary<string, string>>> callBack)
        {
            RestClient restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl") + "/UtilityService");            
            string url = string.Format(relativeBatchUrl, domainName);
            restClient.Query<Dictionary<string, string>>(url, callBack);
        }
    }

    public class AppSettingQueryFilter
    {
        public string DomainName
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }
    }
}
