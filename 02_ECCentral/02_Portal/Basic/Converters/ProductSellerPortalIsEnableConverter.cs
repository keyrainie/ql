using System;
using System.Globalization;
using System.Windows.Data;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.Basic.Converters
{
    public class ProductSellerPortalIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SellerProductRequestStatus)
            {
                switch ((SellerProductRequestStatus)value)
                {
                    case SellerProductRequestStatus.WaitApproval:
                        return false;
                    case SellerProductRequestStatus.Approved:
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

    public class ProductSellerPortalVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Visibility result = System.Windows.Visibility.Collapsed;
            if (value is SellerProductRequestStatus)
            {
                switch ((SellerProductRequestStatus)value)
                {
                    case SellerProductRequestStatus.WaitApproval:
                        if (parameter != null) result = System.Windows.Visibility.Visible;
                        break;
                    case SellerProductRequestStatus.Approved:
                        if (parameter != null && parameter.ToString().Equals("DenyOperate"))
                        {
                            result = System.Windows.Visibility.Visible;
                        }
                        else if (parameter != null && parameter.ToString().Equals("AuditOperate"))
                        {
                            result = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            result = System.Windows.Visibility.Visible;
                        }
                        break;   
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
