using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductInfoFinishStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductInfoFinishStatus)
            {
                if ((ProductInfoFinishStatus)value == ProductInfoFinishStatus.Yes)
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
                return ProductInfoFinishStatus.Yes;
            }
            return ProductInfoFinishStatus.No;
        }
    }
}
