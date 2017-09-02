using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Media;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.Basic.Converters
{
    public class NewsEnableReplyRankConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CustomerRank)
            {
                if ((int)value > 0)
                {
                    return ((CustomerRank)value).ToDescription();
                }
                else
                {
                    return "所有客户";
                }
            }

            return "所有客户";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
