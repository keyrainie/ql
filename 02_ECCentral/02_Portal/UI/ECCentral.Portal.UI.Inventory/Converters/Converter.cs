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
using ECCentral.Portal.Basic.Converters;

namespace ECCentral.Portal.UI.Inventory.Converters
{
    public class ThisDateTimeConvert : DateTimeConverter
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object o = base.Convert(value, targetType, parameter, culture);
            if (o != null && o.ToString().Trim() != string.Empty)
            {
                return String.Format("[{0}]", o);
            }
            return string.Empty;
        }
    }
}
