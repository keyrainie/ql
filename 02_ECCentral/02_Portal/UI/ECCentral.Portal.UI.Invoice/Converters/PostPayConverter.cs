using System;
using System.Windows.Data;
using System.Windows.Media;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Converters
{
    public class PostPayConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            if ((PostIncomeConfirmStatus?)value == PostIncomeConfirmStatus.Audit)
            {
                brush.Color = Colors.Green;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}