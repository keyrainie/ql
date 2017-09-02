using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class CustomerRankConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CustomerRank)
            {
                return EnumConverter.GetDescription(value, typeof(CustomerRank));
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
