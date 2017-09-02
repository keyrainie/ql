using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductResourceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductResourceStatus)
            {
                if ((ProductResourceStatus)value == ProductResourceStatus.Active)
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
                return ProductResourceStatus.Active;
            }
            return ProductResourceStatus.DeActive;
        }
    }


    public class ProductResourceIsShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductResourceIsShow)
            {
                if ((ProductResourceIsShow)value == ProductResourceIsShow.Yes)
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
                return ProductResourceIsShow.Yes;
            }
            return ProductResourceIsShow.No;
        }
    }





}
