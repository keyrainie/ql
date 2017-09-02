using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class VisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as DynamicXml;
            if (data != null)
            {
                string para = parameter.ToString();
                switch (para)
                {
                    case "ProductGiftItem":
                        if ((int)data["GiftItemCount"] > 0 
                            || (int)data["ExtendWarrantyCount"] > 0)
                        {
                            return Visibility.Visible;
                        }
                        break;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BoolToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            if (culture == null) throw new ArgumentNullException("culture");
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
