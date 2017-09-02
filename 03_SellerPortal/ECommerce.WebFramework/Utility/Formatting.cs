using System;

namespace ECommerce.WebFramework
{
    public enum MonyFormatPatterns
    {
        Default
    }

    public enum TimeFormatPatterns
    {
        Default,
        ShortDate,
        LongDate,
        LongTime
    }

    public class JavascriptFormatString
    {
        public const string MomentDate = "YYYY-MM-DD";
        public const string MomentDateTime = "YYYY-MM-DD HH:mm";
    }

    public class DotnetFormatString
    {
        public const string ShortDate = "MM-dd";
        public const string LongDate = "yyyy-MM-dd";
        public const string LongTime = "yyyy-MM-dd HH:mm";
    }

    public class Formatting
    {
        public static string FormatMoney(object amount)
        {
            return FormatMoney(amount, MonyFormatPatterns.Default);
        }

        public static string FormatMoney(object amount, MonyFormatPatterns pattern)
        {
            decimal val = Convert.ToDecimal(amount);
            return val.ToString("0.00");
        }

        public static string FormatDateTime(object datetime, TimeFormatPatterns pattern)
        {
            if (datetime == null)
                return string.Empty;

            DateTime typedDateTime;
            try
            {
                typedDateTime = Convert.ToDateTime(datetime);
            }
            catch
            {
                return datetime.ToString();
            }

            switch (pattern)
            {
                case TimeFormatPatterns.LongTime:
                    return typedDateTime.ToString(DotnetFormatString.LongTime);
                case TimeFormatPatterns.ShortDate:
                    return typedDateTime.ToString(DotnetFormatString.ShortDate);
                case TimeFormatPatterns.LongDate:
                    return typedDateTime.ToString(DotnetFormatString.LongDate);
                default:
                    return datetime.ToString();
            }
        }
    }
}