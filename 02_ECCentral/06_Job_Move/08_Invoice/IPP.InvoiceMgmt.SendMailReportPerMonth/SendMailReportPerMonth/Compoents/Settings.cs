using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IPPOversea.InvoiceMgmt.PerMonthReport.Compoents
{
    public sealed class Settings
    {
        public static string LongDateFormat = "yyyy-MM-dd HH:mm:ss";

        public static string ShortDateFormat = "yyyy-MM-dd";

        public static string Decimal = "#0.00";

        public static string DecimalFormatWithGroup = "#,###,###,##0.00";

        public static decimal DecimalNull = -999999;

        public static int IntNull = -999999;

        public static string EmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailAddress"];
            }
        }

        public static string EmailSubjectError
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailSubjectError"];
            }
        }

        public static string EmailAddressError
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailAddressError"];
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
    }

}
