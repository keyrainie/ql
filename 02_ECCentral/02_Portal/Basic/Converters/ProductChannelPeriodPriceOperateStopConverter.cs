using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductChannelPeriodPriceOperateStopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductChannelPeriodPriceStatus)
            {
                switch ((ProductChannelPeriodPriceStatus)value)
                {
                    case ProductChannelPeriodPriceStatus.Ready:
                    case ProductChannelPeriodPriceStatus.Running:
                        return "停止";
                     default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
