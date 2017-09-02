using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using ECCentral.Portal.UI.ExternalSYS.Models;

namespace ECCentral.Portal.UI.ExternalSYS.Converters
{
    public class VendorUserConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as VendorUserVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "pass":
                        if (data.Status == ECCentral.BizEntity.ExternalSYS.ValidStatus.DeActive)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    case "Invalid":
                        if (data.Status == ECCentral.BizEntity.ExternalSYS.ValidStatus.Active)
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
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
