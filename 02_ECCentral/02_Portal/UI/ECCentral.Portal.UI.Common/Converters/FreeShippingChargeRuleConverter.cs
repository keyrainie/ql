using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Common.Models;

namespace ECCentral.Portal.UI.Common.Converters
{
    public class FreeShippingChargeRuleStutusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = value as Nullable<FreeShippingAmountSettingStatus>;
            var param = parameter.ToString();

            switch (param)
            {
                case "StatusEnableSet":
                    return !(status == FreeShippingAmountSettingStatus.Active);

                case "StatusVisibilitySet":
                    if (status == FreeShippingAmountSettingStatus.Active)
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
