using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductStatusOnSaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductStatus)
            {
                if ((ProductStatus)value == ProductStatus.InActive_Show || (ProductStatus)value == ProductStatus.InActive_UnShow)
                {
                    return "Visible";//
                }
            }
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductStatusOnShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductStatus)
            {
                if ((ProductStatus)value == ProductStatus.Active || (ProductStatus)value == ProductStatus.InActive_UnShow)
                {
                    return "Visible";
                }
            }
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductStatusNoShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductStatus)
            {
                if ((ProductStatus)value != ProductStatus.InActive_UnShow)
                {
                    return "Visible";
                }
            }
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductStatusAbandonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductStatus)
            {
                if ((ProductStatus)value == ProductStatus.InActive_Show || (ProductStatus)value == ProductStatus.InActive_UnShow)
                {
                    return "Visible";
                }
            }
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductStatus)
            {
                return EnumConverter.GetDescription(value, typeof(ProductStatus));
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToWayConvert : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

         return !((bool)value);
            
           
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
