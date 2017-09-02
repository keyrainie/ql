using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ECCentral.Job.Inventory.ProductRing.Components
{
    /// <summary>
    /// Configuration Settings class for this App.
    /// </summary>
    public sealed class Settings
    {
        public static string LongDateFormat = "yyyy-MM-dd HH:mm:ss";

        public static string ShortDateFormat = "yyyy-MM-dd";

        public static string Decimal = "#0.00";

        public static string EmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailAddress"];
            }
        }

        public static string LogFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFileName"];
            }
        }

        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }

        public static string RestFulBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["RestFulBaseUrl"];
            }
        }

        public static string LanguageCode
        {
            get
            {
                return ConfigurationManager.AppSettings["LanguageCode"];
            }
        }

        public static string AllFlag
        {
            get
            {
                return ConfigurationManager.AppSettings["AllFlag"];
            }
        }
    }
}

