using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Windows;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductChannelPeriodPriceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductChannelPeriodPriceStatus)
            {
                switch ((ProductChannelPeriodPriceStatus)value)
                {
                    case ProductChannelPeriodPriceStatus.Init:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
