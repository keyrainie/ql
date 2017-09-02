using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Windows;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductChannelPeriodPriceBtnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductChannelPeriodPriceStatus)
            {
                switch ((ProductChannelPeriodPriceStatus)value)
                {
                    case ProductChannelPeriodPriceStatus.WaitApproved:
                        return parameter.ToString().Equals("Approve") ? Visibility.Visible : Visibility.Collapsed;
                    case ProductChannelPeriodPriceStatus.Init:
                        return parameter.ToString().Equals("Edit") ? Visibility.Visible : Visibility.Collapsed;
                    default:
                        return Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
