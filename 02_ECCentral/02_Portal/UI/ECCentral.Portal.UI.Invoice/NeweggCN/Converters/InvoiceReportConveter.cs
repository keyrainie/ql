using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Converters
{
    public class InvoiceReportConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var data = value as dynamic;
            if (data != null)
            {
                var par = parameter.ToString();
                switch (par)
                {
                    case "OrderID":
                        return (data.OrderType == "SO" || data.OrderType == "RO");

                    case "OrderID_Color":
                        if (data.OrderType == "SO" || data.OrderType == "RO")
                        {
                            return new SolidColorBrush(Colors.Purple);
                        }
                        else
                        {
                            return new SolidColorBrush(Colors.Black);
                        }
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