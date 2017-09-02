using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class NetPayConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as NetPayVM;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "Edit_Audit":
                    case "Edit_Abandon":
                        return (data.Status == NetPayStatus.Origin) ?
                            Visibility.Visible :
                            Visibility.Collapsed;

                    case "Edit_Disabled":
                        return (data.Status != NetPayStatus.Origin) ?
                            Visibility.Visible :
                            Visibility.Collapsed;

                    case "Status":
                        return (data.Status == NetPayStatus.Approved) ?
                            new SolidColorBrush(Colors.Green) :
                            new SolidColorBrush(Colors.Black);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}