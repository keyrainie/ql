using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Media;

namespace ECCentral.Portal.Basic.Converters
{
    public class NewsIsRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush();

            brush.Color = Colors.Black;

            if (value is int)
            {
                if (((int)value) == 1)
                {
                    brush.Color = Colors.Orange;
                }
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
