using System;
using System.Globalization;
using System.Windows.Data;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductGroupMaintainCreateFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                if ((bool)value)
                {
                    return "Collapsed";
                }
                return "Visible";
            }
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
