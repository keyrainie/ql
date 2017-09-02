using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MerchantCommissionSettle.Components
{
    /// <summary>
    /// Configuration Settings class for this App.
    /// </summary>
    public static class GlobalSettings
    {
        public static string AlertMailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["AlertMailAddress"];
            }
        }

        public static string AlertMailSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["AlertMailSubject"];
            }
        }

        public static string MailSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["MailSubject"];
            }
        }

        public static string MailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["MailAddress"];
            }
        }

        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }

        public static string StoreCompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreCompanyCode"];
            }
        }

        public static string CurrencyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CurrencyCode"];
            }
        }

        public static string LanguageCode
        {
            get
            {
                return ConfigurationManager.AppSettings["LanguageCode"];
            }
        }

        public static string UserName
        {
            get
            {
                return ConfigurationManager.AppSettings["UserName"];
            }
        }

        public static int UserSysNo
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["UserSysNo"]);
            }
        }

        public static string SourceDirectoryKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SourceDirectoryKey"];
            }
        }

        public static string FromIP
        {
            get
            {
                return ConfigurationManager.AppSettings["FromIP"];
            }
        }

        public static string LogFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFileName"];
            }
        }

        public static string LongDateFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["LongDateFormat"];
            }
        }

        public static string ShortDateFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["ShortDateFormat"];
            }
        }

        public static string DecimalFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["DecimalFormat"];
            }
        }

        /// <summary>
        /// 只计算店租的佣金结算方式
        /// </summary>
        public static string OnlyCalcRentFeeAccType
        {
            get 
            {
                return ConfigurationManager.AppSettings["OnlyCalcRentFeeAccType"];
            }
        }
    }
}

