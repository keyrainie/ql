using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductRankPriceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductRankPriceStatus)
            {
                if ((ProductRankPriceStatus)value == ProductRankPriceStatus.Active)
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
                return ProductRankPriceStatus.Active;
            }
            return ProductRankPriceStatus.DeActive;
        }
    }
}
