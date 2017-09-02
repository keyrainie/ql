using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ContentMgmt.GiftCardPoolInterface
{
    public static class ConstValues
    {
        private static readonly string companyCode = ConfigurationManager.AppSettings["companyCode"];

        public static string CompanyCode
        {
            get
            {
                return companyCode;
            }
        }

        private static readonly string languageCode = ConfigurationManager.AppSettings["languageCode"];

        public static string LanguageCode
        {
            get
            {
                return languageCode;
            }
        }

        private static readonly string toMailAddress = ConfigurationManager.AppSettings["toMailAddress"];

        public static string ToMailAddress
        {
            get
            {
                return toMailAddress;
            }
        }

        private static readonly string ccMailAddress = ConfigurationManager.AppSettings["ccMailAddress"];

        public static string CcMailAddress
        {
            get
            {
                return ccMailAddress;
            }
        }

        private static readonly string initialCode = ConfigurationManager.AppSettings["initialCode"];

        public static string InitialCode
        {
            get
            {
                return initialCode;
            }
        }


        private static readonly int availableCount = int.Parse(ConfigurationManager.AppSettings["availableCount"]);

        public static int AvailableCount
        {
            get
            {
                return availableCount;
            }
        }
        private static readonly string availableCode = ConfigurationManager.AppSettings["availableCode"];

        public static string AvailableCode
        {
            get
            {
                return availableCode;
            }
        }

        private static readonly int codeDimension = AvailableCode.Length;

        public static int CodeDimension
        {
            get
            {
                return codeDimension;
            }
        }

        private static readonly string availablePassword = ConfigurationManager.AppSettings["availablePassword"];

        public static string AvailablePassword
        {
            get
            {
                return availablePassword;
            }
        }

        private static readonly int passwordDimension = AvailablePassword.Length;

        public static int PasswordDimension
        {
            get
            {
                return passwordDimension;
            }
        }
    }
}
