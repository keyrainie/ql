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
using System.Windows.Data;

namespace ECCentral.Portal.Basic.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        private const string Long = "Long";
        private const string Short = "Short";
        private const string YearMonth = "YearMonth";

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                DateTime dt;
                if (DateTime.TryParse(value.ToString(), out dt))
                {
                    string format = (parameter ?? Short).ToString();
                    if (string.Compare(format, Long, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return dt.ToString(ResConverter.DateTime_LongFormat);
                    }
                    else if (string.Compare(format, Short, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return dt.ToString(ResConverter.DateTime_ShortFormat);
                    }
                    else if (string.Compare(format, YearMonth, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return dt.ToString("yyyy-MM");
                    }                 
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
