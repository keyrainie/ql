using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class PropertyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PropertyType)
            {
                return EnumConverter.GetDescription(value, typeof(PropertyType));
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
