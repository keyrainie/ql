using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class IsAutoAdjustPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IsAutoAdjustPrice)
            {
                if ((IsAutoAdjustPrice)value == IsAutoAdjustPrice.Yes)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool)value;
            if (isTrue)
            {
                return IsAutoAdjustPrice.No;
            }
            return IsAutoAdjustPrice.Yes;
        }
    }
}
