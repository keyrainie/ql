using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace IPP.EcommerceMgmt.SendCouponCode
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

        private static readonly string storeCompanyCode = ConfigurationManager.AppSettings["storeCompanyCode"];

        public static string StoreCompanyCode
        {
            get
            {
                return storeCompanyCode;
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

        private static readonly string editUser = ConfigurationManager.AppSettings["editUser"];

        public static string EditUser
        {
            get
            {
                return editUser;
            }
        }

        public static string MailTitlePrefix
        {
            get
            {
                return "(info)IPP-MktToolMgmt-CouponCustomerLogApp---";
            }
        }

        public static string LogCategory
        {
            get
            {
                return "CouponCustomerLogApp";
            }
        }

        private static string mailTemplate = null;

        public static string MailTemplate
        {
            get
            {
                if (mailTemplate == null)
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(
                        System.Windows.Forms.Application.StartupPath, "template\\EmailTemplate.txt")))
                    {
                        mailTemplate = sr.ReadToEnd();
                    }
                }
                return mailTemplate;
            }
        }

        private static string birthdaymailTemplate = null;

        public static string BirthdayMailTemplate
        {
            get
            {
                if (birthdaymailTemplate == null)
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(
                        System.Windows.Forms.Application.StartupPath, "template\\BirthDayEmailTemplate.txt")))
                    {
                        birthdaymailTemplate = sr.ReadToEnd();
                    }
                }
                return birthdaymailTemplate;
            }
        }
    }
}
