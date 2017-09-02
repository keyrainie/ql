using System;
using System.Globalization;
using System.Windows.Data;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.Basic.Converters
{
    public class ToCashStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToCashStatus)
            {
                switch ((ToCashStatus)value)
                {
                    case ToCashStatus.Requested:
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
