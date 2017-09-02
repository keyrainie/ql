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

namespace ECCentral.Portal.UI.PO.Converters
{
    public class BasketQueryConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            string par = parameter.ToString();
            switch (par)
            {
                case "IsTransfer":
                    if (data.ToString() == "0")
                        return "否";
                    else
                        return "是";
                default:
                    return string.Empty;
                        
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
