using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductGroupPolymericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductGroupPolymeric)
            {
                if ((ProductGroupPolymeric)value == ProductGroupPolymeric.Yes)
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
                return ProductGroupPolymeric.Yes;
            }
            return ProductGroupPolymeric.No;
        }
    }
}
