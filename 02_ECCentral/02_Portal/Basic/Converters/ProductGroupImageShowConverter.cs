using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductGroupImageShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductGroupImageShow)
            {
                if ((ProductGroupImageShow)value == ProductGroupImageShow.Yes)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool)value;
            if (isTrue)
            {
                return ProductGroupImageShow.Yes;
            }
            return ProductGroupImageShow.No;
        }
    }
}
