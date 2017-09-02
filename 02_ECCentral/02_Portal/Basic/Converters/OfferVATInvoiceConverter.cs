using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Converters
{
    public class OfferVATInvoiceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OfferVATInvoice)
            {
                if ((OfferVATInvoice)value == OfferVATInvoice.Yes)
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool)value;
            if (isTrue)
            {
                return OfferVATInvoice.No;
            }
            return OfferVATInvoice.Yes;
        }
    }
}
