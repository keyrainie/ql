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

namespace ECCentral.Portal.UI.PO.Views
{
    public static class ConverterHelper
    {

        public static decimal? ToNullableDecimal(this string str)
        {
            return string.IsNullOrEmpty(str) ? (decimal?)null : Convert.ToDecimal(str);
        }

        public static decimal ToDecimal(this string str)
        {
            return string.IsNullOrEmpty(str) ? 0m : Convert.ToDecimal(str);
        }

        public static int? ToNullableToInteger(this string str)
        {
            return string.IsNullOrEmpty(str) ? (int?)null : Convert.ToInt32(str);
        }

        public static int ToInteger(this string str)
        {
            return string.IsNullOrEmpty(str) ? 0 : Convert.ToInt32(str);
        }

        public static string DecimailToString(this decimal? val)
        {
            return val.HasValue ? val.Value.ToString() : string.Empty;
        }

        public static string IntegerToString(this int? val)
        {
            return val.HasValue ? val.Value.ToString() : string.Empty;
        }
    }
}
